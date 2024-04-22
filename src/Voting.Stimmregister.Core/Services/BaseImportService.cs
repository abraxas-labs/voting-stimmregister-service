// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Voting.Lib.Common;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Models;
using Voting.Stimmregister.Abstractions.Adapter.VotingBasis;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Abstractions.Core.Import.Mapping;
using Voting.Stimmregister.Abstractions.Core.Import.Models;
using Voting.Stimmregister.Abstractions.Core.Import.Services;
using Voting.Stimmregister.Core.Exceptions;
using Voting.Stimmregister.Core.Extensions;
using Voting.Stimmregister.Domain.Cache;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Diagnostics;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Exceptions;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services;

/// <summary>
/// Import service base implementation.
/// </summary>
/// <typeparam name="TState">The import state type.</typeparam>
/// <typeparam name="TRecord">The csv record model type.</typeparam>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public abstract class BaseImportService<TState, TRecord, TEntity> : IImportService
    where TState : ImportStateModel<TEntity>, new()
    where TEntity : ImportedEntity, new()
{
    private readonly IClock _clock;
    private readonly IDbRepository<DbContext, TEntity> _entityRepository;
    private readonly IImportStatisticRepository _importStatisticRepository;
    private readonly IIntegrityRepository _integrityRepository;
    private readonly ILogger<BaseImportService<TState, TRecord, TEntity>> _logger;
    private readonly IValidator<TRecord>? _recordValidator;
    private readonly IValidator<TEntity> _entityValidator;
    private readonly IPermissionService _permissionService;
    private readonly IDataContext _dataContext;
    private readonly IRecordEntityMapper<TState, TRecord, TEntity> _mapper;
    private readonly ImportType _importType;
    private readonly BaseImportConfig _config;
    private readonly IMunicipalityIdCantonCache _municipalityIdCantonCache;
    private readonly ICantonBfsCache _cantonBfsCache;

    protected BaseImportService(
        ImportType importType,
        IClock clock,
        IDbRepository<DbContext, TEntity> entityRepository,
        IImportStatisticRepository importStatisticRepository,
        IIntegrityRepository integrityRepository,
        ILogger<BaseImportService<TState, TRecord, TEntity>> logger,
        IValidator<TRecord>? recordValidator,
        IValidator<TEntity> entityValidator,
        IPermissionService permissionService,
        IAccessControlListDoiService aclDoiService,
        IDataContext dataContext,
        IRecordEntityMapper<TState, TRecord, TEntity> mapper,
        ImportsConfig importConfig,
        IMunicipalityIdCantonCache municipalityIdCantonCache,
        ICantonBfsCache cantonBfsCache)
    {
        _clock = clock;
        _entityRepository = entityRepository;
        _importStatisticRepository = importStatisticRepository;
        _integrityRepository = integrityRepository;
        _logger = logger;
        _recordValidator = recordValidator;
        _entityValidator = entityValidator;
        _permissionService = permissionService;
        AclDoiService = aclDoiService;
        _dataContext = dataContext;
        _mapper = mapper;
        ImportConfig = importConfig;
        _importType = importType;
        _config = ImportConfig.GetImportConfig(importType);
        _municipalityIdCantonCache = municipalityIdCantonCache;
        _cantonBfsCache = cantonBfsCache;
    }

    protected IAccessControlListDoiService AclDoiService { get; }

    protected ImportsConfig ImportConfig { get; }

    /// <inheritdoc/>
    public async Task RunImport(ImportDataModel data)
    {
        _logger.LogInformation("Start importing file '{Name}' with id {Id}", data.Name, data.Id);

        var importState = new TState { ImportStatisticId = data.Id, DataReceivedTimestamp = data.ReceivedTimestamp, ImportSourceSystem = data.SourceSystem };

        try
        {
            await ProcessRecords(importState, data.Content);
            await PostImportProcessingBaseAssertion(importState);
            MarkUnprocessedAsDeleted(importState);

            await using var transaction = await _dataContext.BeginTransaction();
            await CreateIntegrity(importState);
            await UpdateDatabase(importState);
            await UpdateImportStatistics(importState);
            await PostProcessImport(importState);
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occured during import processing of import with id '{ImportId}'",
                importState.ImportStatisticId);

            await _importStatisticRepository.UpdateFinishedWithProcessingErrors(
                importState.ImportStatisticId,
                ex.Message,
                importState.RecordValidationErrors,
                importState.EntityIdsWithValidationErrors,
                importState.MunicipalityId,
                _clock.UtcNow);
        }

        _logger.LogInformation(
            "Finished importing file '{Name}' with id '{Id}'",
            data.Name,
            data.Id);
    }

    public async Task<int?> PeekMunicipalityIdFromStream(Stream content)
    {
        await using var reader = CreateReader(content);
        var record = await reader.ReadRecords().FirstOrDefaultAsync();
        return record == null
            ? null
            : GetMunicipalityId(record);
    }

    protected abstract void SignIntegrity(TState state, BfsIntegrityEntity integrity);

    protected abstract Task InitializeState(TState state);

    protected abstract IImportRecordReader<TRecord> CreateReader(Stream content);

    protected abstract TEntity? FindExistingEntity(TState state, TRecord record);

    protected abstract void MarkUnprocessedAsDeleted(TState state);

    protected abstract bool IsImportedDataEqual(TEntity updatedEntity, TEntity existingEntity);

    protected abstract int GetMunicipalityId(TRecord record);

    protected abstract string BuildHumanReadableIdentifier(TRecord record);

    /// <summary>
    /// Enables deriving import classes to extend the import process with additional assertion logic.
    /// </summary>
    /// <param name="importState">The import state model.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected virtual Task PostImportProcessingAssertion(TState importState)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Enables deriving import classes to extend the import process with additional post processing logic.
    /// </summary>
    /// <param name="state">The import state model.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected virtual Task PostProcessImport(TState state)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Initializes the state on first iteration with existing data from database,
    /// with the municipality id from first record,
    /// Note: The MunicipalityId must be unique across all records within the import file.
    /// </summary>
    /// <param name="state">The import state model.</param>
    /// <param name="firstRecord">The first record.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task InitializeState(TState state, TRecord firstRecord)
    {
        var municipalityId = GetMunicipalityId(firstRecord);
        if (municipalityId <= 0)
        {
            throw new ImportException("MunicipalityId could not be set from first record.");
        }

        EnsureMunicipalityIdNotBlacklisted(municipalityId);

        var aclEntry = await AclDoiService.GetValidEntryForBfs(municipalityId.ToString())
            ?? throw new ImportException($"No valid acl entry found for {municipalityId}");

        state.MunicipalityId = municipalityId;
        state.MunicipalityName = aclEntry.Name;
        state.MunicipalityAddress = aclEntry.ReturnAddress;

        var canton = await _municipalityIdCantonCache.GetCantonByMunicipalityId(municipalityId);
        var cantonBfs = await _cantonBfsCache.GetBfsOfCanton(canton) ?? throw new ImportException($"No canton acl entry found for {municipalityId}");
        state.CantonBfs = short.Parse(cantonBfs);

        _permissionService.SetAbraxasAuthIfNotAuthenticated();
        _permissionService.SetImpersonatedAccessControlPermissions(new[] { municipalityId.ToString() });

        await _importStatisticRepository.UpdateMunicipalityId(state.ImportStatisticId, state.MunicipalityId, aclEntry.Name);
        await InitializeState(state);
        state.Initialized = true;
    }

    private void EnsureMunicipalityIdNotBlacklisted(int municipalityId)
    {
        if (ImportConfig.MunicipalityIdBlacklistById.TryGetValue(municipalityId, out var startingDate) && startingDate < _clock.UtcNow)
        {
            throw new ImportException(
                $"MunicipalityId '{municipalityId}' is blacklisted. Matches item with starting date '{startingDate.ToUniversalTime():dd.MM.yyyy HH:mm:ss}'");
        }
    }

    private async Task ProcessRecords(TState importState, Stream content)
    {
        await using var reader = CreateReader(content);
        await foreach (var record in reader.ReadRecords())
        {
            importState.RecordsCount++;

            if (!importState.Initialized)
            {
                await InitializeState(importState, record);
            }

            if (!IsRecordValid(importState, record))
            {
                continue;
            }

            AssertMunicipalityIdUnique(importState, record);
            ProcessRecord(importState, record);
        }
    }

    /// <summary>
    /// Processes an import record and updates the import state model accordingly.
    /// </summary>
    /// <param name="state">The import state model.</param>
    /// <param name="record">The csv record.</param>
    private void ProcessRecord(TState state, TRecord record)
    {
        var existingEntity = FindExistingEntity(state, record);

        var entity = (TEntity)Activator.CreateInstance(typeof(TEntity))!;
        _mapper.MapEntityLifecycleMetadata(entity, existingEntity);
        _mapper.MapRecordToEntity(entity, state, record);

        ValidateEntity(state, entity);

        if (existingEntity == null)
        {
            state.Create(entity);
            return;
        }

        if (IsImportedDataEqual(entity, existingEntity))
        {
            state.SetUnchanged(existingEntity);
        }
        else
        {
            state.Update(entity, existingEntity);
        }

        state.SetProcessed(existingEntity);
    }

    /// <summary>
    /// Checks for each record for its valid state.
    /// Throws a <see cref="ImportException"/> if MaxRecordValidationErrorsThreshold was reached.
    /// </summary>
    /// <param name="importState">The import state model.</param>
    /// <param name="record">The csv record to check.</param>
    private bool IsRecordValid(TState importState, TRecord record)
    {
        var validationResult = _recordValidator?.Validate(record);
        if (validationResult?.IsValid != false)
        {
            return true;
        }

        importState.HasRecordValidationErrors = true;
        importState.RecordIdsWithValidationErrors.Add(importState.RecordsCount);

        var validationErrorsModel = validationResult.ToRecordValidationErrorModel(importState.RecordsCount, BuildHumanReadableIdentifier(record));
        importState.RecordValidationErrors.Add(validationErrorsModel);

        _logger.LogError(
            "Validation failed for import '{ImportId}' and record #{CsvRecordsCounter}. Errors: {ValidationErrors}",
            importState.ImportStatisticId,
            importState.RecordsCount,
            JsonSerializer.Serialize(validationErrorsModel));

        if (importState.RecordIdsWithValidationErrors.Count < _config.MaxRecordValidationErrorsThreshold)
        {
            return false;
        }

        throw new ImportException(
            $"Maximum record validation errors exceeded: {importState.RecordIdsWithValidationErrors.Count}/{_config.MaxRecordValidationErrorsThreshold}");
    }

    /// <summary>
    /// If municipality id is not unique within the import file for all records, the import must be aborted.
    /// Throws a <see cref="ImportException"/> if the record municipality id is the same as the initial record's municipality.
    /// </summary>
    /// <param name="state">The import state model.</param>
    /// <param name="record">The csv record to check.</param>
    private void AssertMunicipalityIdUnique(TState state, TRecord record)
    {
        var recordMunicipalityId = GetMunicipalityId(record);
        if (recordMunicipalityId.Equals(state.MunicipalityId))
        {
            return;
        }

        _logger.LogError(
                "MunicipalityId is not unique within file. Expected: '{ImportMunicipalityId}', Received: '{RecordMunicipalityId}', Record: #{RecordsCounter}!",
                state.MunicipalityId,
                state.RecordsCount,
                recordMunicipalityId);

        throw new ImportException(
            $"MunicipalityId is not unique within file. Expected: '{state.MunicipalityId}', Received: '{recordMunicipalityId}' Record: #{state.RecordsCount}!");
    }

    /// <summary>
    /// Processes the validation of the passed entity. Potential validation errors are stored on the entity for reporting purposes.
    /// </summary>
    /// <param name="state">The import state model.</param>
    /// <param name="entity">The entity to validate and update.</param>
    private void ValidateEntity(
        TState state,
        TEntity entity)
    {
        var validationResult = _entityValidator.Validate(entity);
        if (validationResult.IsValid)
        {
            entity.IsValid = true;
            return;
        }

        _logger.LogDebug("Validation failed for entity with id '{Id}'", entity.Id);

        var validationErrors = validationResult.ToDictionary();
        entity.IsValid = false;
        entity.ValidationErrors = JsonSerializer.Serialize(validationErrors);
        state.EntityIdsWithValidationErrors.Add(entity.Id);
    }

    private Task PostImportProcessingBaseAssertion(TState importState)
    {
        AssertImportStateIsValid(importState);
        return PostImportProcessingAssertion(importState);
    }

    /// <summary>
    /// Validates the import state.
    /// </summary>
    /// <param name="importState">The import state to validate.</param>
    /// <exception cref="ImportException">Thrown if state is not valid.</exception>
    private void AssertImportStateIsValid(TState importState)
    {
        if (importState.RecordsCount == 0)
        {
            throw new ImportException("No CSV records processed which indicates input file has no records.");
        }
    }

    private async Task UpdateDatabase(TState importState)
    {
        if (importState.EntitiesToUpdate.Count > 0)
        {
            await _entityRepository.UpdateRange(importState.EntitiesToUpdate);
        }

        if (importState.EntitiesToDelete.Count > 0)
        {
            await _entityRepository.DeleteRangeByKey(importState.EntitiesToDelete.Select(x => x.Id));
        }

        if (importState.EntitiesToCreate.Count > 0)
        {
            await _entityRepository.CreateRange(importState.EntitiesToCreate);
        }
    }

    private async Task CreateIntegrity(TState state)
    {
        var integrityEntity = new BfsIntegrityEntity
        {
            Bfs = state.MunicipalityId?.ToString() ?? string.Empty,
            ImportType = _importType,
            LastUpdated = state.DataReceivedTimestamp,
        };

        _permissionService.SetCreated(integrityEntity);
        SignIntegrity(state, integrityEntity);
        await _integrityRepository.CreateOrUpdate(integrityEntity);
    }

    private async Task UpdateImportStatistics(TState state)
    {
        var importEntity = await _importStatisticRepository.GetByKey(state.ImportStatisticId)
            ?? throw new EntityNotFoundException(typeof(ImportStatisticEntity), state.ImportStatisticId);

        importEntity.ImportStatus = state.HasRecordValidationErrors ?
            ImportStatus.FinishedWithErrors :
            ImportStatus.FinishedSuccessfully;

        importEntity.ImportRecordsCountTotal = state.RecordsCount;
        importEntity.DatasetsCountCreated = state.CreateCount;
        importEntity.DatasetsCountUpdated = state.UpdateCount;
        importEntity.DatasetsCountDeleted = state.DeleteCount;
        importEntity.FinishedDate = _clock.UtcNow;
        importEntity.HasValidationErrors = state.EntityIdsWithValidationErrors.Count > 0;
        importEntity.EntitiesWithValidationErrors = state.EntityIdsWithValidationErrors;
        importEntity.RecordNumbersWithValidationErrors = state.RecordIdsWithValidationErrors;
        importEntity.RecordValidationErrors = state.RecordValidationErrors.Count > 0
            ? JsonSerializer.Serialize(state.RecordValidationErrors)
            : null;

        await _importStatisticRepository.Update(importEntity);

        DiagnosticsConfig.IncreaseProcessedImportJobs(
            importEntity.ImportType.ToString(),
            importEntity.ImportStatus.ToString());

        DiagnosticsConfig.SetImportDatasetsMutated(
            importEntity.ImportType.ToString(),
            state.MunicipalityId,
            state.CreateCount + state.UpdateCount + state.DeleteCount);
    }
}
