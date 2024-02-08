// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Abraxas.Voting.Basis.Services.V1;
using Abraxas.Voting.Basis.Services.V1.Models;
using Abraxas.Voting.Basis.Shared.V1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.VotingBasis;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Adapter.VotingBasis.Validators;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Exceptions;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.VotingBasis.Services;

/// <summary>
/// Domain of influence access control list import service from VOTING Basis.
/// </summary>
public class AccessControlListImportService : IAccessControlListImportService
{
    private readonly AccessControlListEntityValidator _aclEntityValidator = new();

    private readonly IClock _clock;
    private readonly AdminManagementService.AdminManagementServiceClient _votingBasisServiceClient;
    private readonly IAccessControlListDoiRepository _aclDoiRepo;
    private readonly IImportStatisticRepository _importStatisticRepo;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<AccessControlListImportService> _logger;
    private readonly IDataContext _dataContext;
    private readonly ImportsConfig _config;
    private readonly List<Guid> _entityIdsWithValidationErrors = new();

    private readonly List<AccessControlListDoiEntity> _allAclImportList = new();
    private readonly List<AccessControlListDoiEntity> _newAclEntityList = new();
    private readonly List<AccessControlListDoiEntity> _updateAclEntityList = new();
    private readonly List<AccessControlListDoiEntity> _deleteAclEntityList = new();

    private readonly Stopwatch _stopwatch = new();
    private Guid _importStatisticId;

    public AccessControlListImportService(
        IClock clock,
        IAccessControlListDoiRepository aclDoiRepo,
        IImportStatisticRepository importStatisticRepo,
        AdminManagementService.AdminManagementServiceClient votingBasisServiceClient,
        ILogger<AccessControlListImportService> logger,
        IPermissionService permissionService,
        IDataContext dataContext,
        ImportsConfig config)
    {
        _clock = clock;
        _aclDoiRepo = aclDoiRepo;
        _importStatisticRepo = importStatisticRepo;
        _votingBasisServiceClient = votingBasisServiceClient;
        _logger = logger;
        _permissionService = permissionService;
        _dataContext = dataContext;
        _config = config;
    }

    /// <inheritdoc/>
    public async Task ImportAcl()
    {
        _stopwatch.Start();
        _logger.LogDebug("Start importing domain of influence based access control list from VOTING Basis.");

        try
        {
            _importStatisticId = (await CreateImportStatistics()).Id;

            _allAclImportList.AddRange(await GetFlattenAccessControlListFromVotingBasis());

            var aclsFromRepo = await _aclDoiRepo.Query().AsNoTracking().ToListAsync();
            _deleteAclEntityList.AddRange(aclsFromRepo);

            foreach (var aclFromImport in _allAclImportList)
            {
                var existingAclFromRepo = aclsFromRepo.Find(x => x.Id.Equals(aclFromImport.Id));

                if (existingAclFromRepo != null)
                {
                    _deleteAclEntityList.Remove(existingAclFromRepo);

                    if (!IsEntityUpdateRequired(aclFromImport, existingAclFromRepo))
                    {
                        continue;
                    }

                    UpdateEntity(aclFromImport, existingAclFromRepo);
                }
                else
                {
                    CreateEntity(aclFromImport);
                }
            }

            await UpdateDatabase();
            await UpdateImportStatistics();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while importing domain of influence based access control list from VOTING Basis.");

            if (_importStatisticId != Guid.Empty)
            {
                await _importStatisticRepo.UpdateFinishedWithProcessingErrors(
                    _importStatisticId,
                    ex.Message,
                    new(),
                    _entityIdsWithValidationErrors,
                    null,
                    _clock.UtcNow);
            }
        }

        _logger.LogDebug("Finished importing domain of influence based access control list from VOTING Basis");
    }

    private async Task<ImportStatisticEntity> CreateImportStatistics()
    {
        var entity = new ImportStatisticEntity
        {
            ImportStatus = ImportStatus.Running,
            ImportType = Domain.Enums.ImportType.Acl,
            SourceSystem = ImportSourceSystem.VotingBasis,
            WorkerName = _config.WorkerName,
        };

        _permissionService.SetCreated(entity);
        entity.StartedDate = entity.AuditInfo.CreatedAt;
        await _importStatisticRepo.CreateAndUpdateIsLatest(entity);
        return entity;
    }

    /// <summary>
    /// Processes the validation of the acl entity. Potential validation errors are stored on the entity for reporting purposes.
    /// </summary>
    /// <param name="entity">The ACL entity to validate.</param>
    private void ValidateEntity(AccessControlListDoiEntity entity)
    {
        var validationResult = _aclEntityValidator.Validate(entity);
        if (validationResult.IsValid)
        {
            entity.IsValid = true;
            return;
        }

        _logger.LogWarning("Validation failed for access control list entity with id '{Id}'", entity.Id);

        var validationErrors = validationResult.ToDictionary();
        entity.IsValid = false;
        entity.ValidationErrors = JsonSerializer.Serialize(validationErrors);
        _entityIdsWithValidationErrors.Add(entity.Id);
    }

    private async Task UpdateImportStatistics()
    {
        var importEntity = await _importStatisticRepo.GetByKey(_importStatisticId)
                           ?? throw new EntityNotFoundException(typeof(ImportStatisticEntity), _importStatisticId);

        importEntity.ImportStatus = _entityIdsWithValidationErrors.Count > 0 ?
            ImportStatus.FinishedWithErrors :
            ImportStatus.FinishedSuccessfully;

        importEntity.ImportRecordsCountTotal = _allAclImportList.Count;
        importEntity.DatasetsCountCreated = _newAclEntityList.Count;
        importEntity.DatasetsCountUpdated = _updateAclEntityList.Count;
        importEntity.DatasetsCountDeleted = _deleteAclEntityList.Count;
        importEntity.FinishedDate = _clock.UtcNow;
        importEntity.HasValidationErrors = _entityIdsWithValidationErrors.Count > 0;
        importEntity.EntitiesWithValidationErrors = _entityIdsWithValidationErrors;
        importEntity.TotalElapsedMilliseconds = _stopwatch.ElapsedMilliseconds;

        _permissionService.SetModified(importEntity);

        await _importStatisticRepo.Update(importEntity);
    }

    private async Task<IEnumerable<AccessControlListDoiEntity>> GetFlattenAccessControlListFromVotingBasis()
    {
        var result = await _votingBasisServiceClient.GetPoliticalDomainOfInfluenceHierarchyAsync(new());

        return result.PoliticalDomainOfInfluences
            .SelectMany(GetFlattenChildrenInclSelf)
            .Select(doi => MapToAclEntity(doi, new()));
    }

    private IEnumerable<PoliticalDomainOfInfluence> GetFlattenChildrenInclSelf(PoliticalDomainOfInfluence doi)
    {
        yield return doi;
        foreach (var childDoi in doi.Children.SelectMany(GetFlattenChildrenInclSelf))
        {
            yield return childDoi;
        }
    }

    private AccessControlListDoiEntity MapToAclEntity(
        PoliticalDomainOfInfluence serviceModel,
        AccessControlListDoiEntity entity)
    {
        entity.Id = Guid.Parse(serviceModel.Id);
        entity.Name = serviceModel.Name;
        entity.Bfs = string.IsNullOrWhiteSpace(serviceModel.Bfs) ? null : serviceModel.Bfs;
        entity.TenantName = serviceModel.TenantName;
        entity.TenantId = serviceModel.TenantId;
        entity.Type = (Domain.Enums.DomainOfInfluenceType)serviceModel.Type;
        entity.Canton = serviceModel.Canton == DomainOfInfluenceCanton.Unspecified ?
            Canton.Unknown :
            Enum.Parse<Canton>(serviceModel.Canton.ToString(), ignoreCase: true);
        entity.ParentId = string.IsNullOrWhiteSpace(serviceModel.ParentId) ? null : Guid.Parse(serviceModel.ParentId);
        entity.ImportStatisticId = _importStatisticId;

        return entity;
    }

    private AccessControlListDoiEntity MapToAclEntity(
        AccessControlListDoiEntity source,
        AccessControlListDoiEntity target)
    {
        target.Name = source.Name;
        target.Bfs = string.IsNullOrWhiteSpace(source.Bfs) ? null : source.Bfs;
        target.TenantName = source.TenantName;
        target.TenantId = source.TenantId;
        target.Type = source.Type;
        target.Canton = source.Canton;
        target.ParentId = source.ParentId;
        target.ImportStatisticId = _importStatisticId;

        return target;
    }

    /// <summary>
    /// Compares all relevant fields from the imported entity with the database entity and
    /// indicates whether the entity should be updated or not.
    /// </summary>
    /// <param name="aclFromImport">The imported ACL entity.</param>
    /// <param name="aclFromRepo">The existing ACL entity.</param>
    /// <returns>True if the entity should be updated.</returns>
    private bool IsEntityUpdateRequired(AccessControlListDoiEntity aclFromImport, AccessControlListDoiEntity aclFromRepo)
    {
        var compareFunctions = new List<Func<int>>
        {
            () => Comparer<string>.Default.Compare(aclFromImport.Name, aclFromRepo.Name),
            () => Comparer<string?>.Default.Compare(aclFromImport.Bfs, aclFromRepo.Bfs),
            () => Comparer<string>.Default.Compare(aclFromImport.TenantName, aclFromRepo.TenantName),
            () => Comparer<string>.Default.Compare(aclFromImport.TenantId, aclFromRepo.TenantId),
            () => Comparer<Domain.Enums.DomainOfInfluenceType>.Default.Compare(aclFromImport.Type, aclFromRepo.Type),
            () => Comparer<Canton>.Default.Compare(aclFromImport.Canton, aclFromRepo.Canton),
        };

        return compareFunctions.Any(compareFunction => compareFunction() != 0);
    }

    /// <summary>
    /// Updates an existing entity.
    /// Which means:
    /// <list type="bullet">
    ///     <item>Adding the entity to the update list.</item>
    ///     <item>Removing the entity from the deletion list.</item>
    /// </list>
    /// </summary>
    /// <param name="source">The source to update from.</param>
    /// <param name="target">The target to be updated from the source.</param>
    private void UpdateEntity(AccessControlListDoiEntity source, AccessControlListDoiEntity target)
    {
        var updatedEntity = MapToAclEntity(source, target);
        ValidateEntity(updatedEntity);
        _updateAclEntityList.Add(updatedEntity);
        _deleteAclEntityList.Remove(updatedEntity);
    }

    /// <summary>
    /// Creates a new entity from the import.
    /// </summary>
    /// <param name="entity">The new entity to be added.</param>
    private void CreateEntity(AccessControlListDoiEntity entity)
    {
        ValidateEntity(entity);
        _newAclEntityList.Add(entity);
    }

    private async Task UpdateDatabase()
    {
        await using var transaction = await _dataContext.BeginTransaction();

        if (_updateAclEntityList.Count > 0)
        {
            LogDatabaseUpdates("Update", _updateAclEntityList);
            await _aclDoiRepo.UpdateRange(_updateAclEntityList);
        }

        if (_deleteAclEntityList.Count > 0)
        {
            LogDatabaseUpdates("Delete", _deleteAclEntityList);
            foreach (var id in _deleteAclEntityList.Select(acl => acl.Id))
            {
                await _aclDoiRepo.DeleteByKeyIfExists(id);
            }
        }

        if (_newAclEntityList.Count > 0)
        {
            LogDatabaseUpdates("Create", _newAclEntityList);
            await _aclDoiRepo.CreateRange(_newAclEntityList);
        }

        await transaction.CommitAsync();
    }

    private void LogDatabaseUpdates(string operation, IEnumerable<AccessControlListDoiEntity> entities)
    {
        if (!entities.Any())
        {
            return;
        }

        _logger.LogInformation("{operation} access control list domain of influence summary:", operation);
        foreach (var entity in entities)
        {
            _logger.LogInformation(
                " > {operation} ACL-DOI '{name}' with id {id} of type {type} (valid={isValid})",
                operation,
                entity.Name,
                entity.Id,
                entity.Type,
                entity.IsValid);
        }
    }
}
