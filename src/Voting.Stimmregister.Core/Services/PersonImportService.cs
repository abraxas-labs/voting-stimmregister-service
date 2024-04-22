// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.VotingBasis;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Abstractions.Core.Import.Mapping;
using Voting.Stimmregister.Abstractions.Core.Import.Models;
using Voting.Stimmregister.Abstractions.Core.Import.Services;
using Voting.Stimmregister.Core.Exceptions;
using Voting.Stimmregister.Core.Utils;
using Voting.Stimmregister.Domain.Cache;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Cryptography;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services;

/// <summary>
/// Import service base implementation.
/// </summary>
/// <typeparam name="TRecord">The csv record model type.</typeparam>
public abstract class PersonImportService<TRecord> : BaseImportService<PersonImportStateModel, TRecord, PersonEntity>, IPersonImportService<TRecord>
{
    private readonly IPersonRepository _personRepository;
    private readonly IDomainOfInfluenceRepository _domainOfInfluenceRepository;
    private readonly IEVotersCache _eVotersCache;
    private readonly IClock _clock;
    private readonly ICreateSignatureService _createSignatureService;
    private readonly IMapper _personMapper;
    private readonly IBfsStatisticService _bfsStatisticService;
    private readonly ILogger _logger;

    protected PersonImportService(
        IClock clock,
        IImportStatisticRepository importStatisticRepository,
        IIntegrityRepository integrityRepository,
        ILogger<PersonImportService<TRecord>> logger,
        IValidator<TRecord>? recordValidator,
        IValidator<PersonEntity> entityValidator,
        IPermissionService permissionService,
        IAccessControlListDoiService aclDoiService,
        IDataContext dataContext,
        IPersonRecordEntityMapper<TRecord> mapper,
        ImportsConfig importConfig,
        IPersonRepository personRepository,
        IDomainOfInfluenceRepository domainOfInfluenceRepository,
        IEVotersCache eVotersCache,
        ICreateSignatureService createSignatureService,
        IMapper personMapper,
        IMunicipalityIdCantonCache municipalityIdCantonCache,
        ICantonBfsCache cantonBfsCache,
        IBfsStatisticService bfsStatisticService)
        : base(
            ImportType.Person,
            clock,
            personRepository,
            importStatisticRepository,
            integrityRepository,
            logger,
            recordValidator,
            entityValidator,
            permissionService,
            aclDoiService,
            dataContext,
            mapper,
            importConfig,
            municipalityIdCantonCache,
            cantonBfsCache)
    {
        _personRepository = personRepository;
        _domainOfInfluenceRepository = domainOfInfluenceRepository;
        _eVotersCache = eVotersCache;
        _clock = clock;
        _createSignatureService = createSignatureService;
        _personMapper = personMapper;
        _bfsStatisticService = bfsStatisticService;
        _logger = logger;
    }

    protected override void SignIntegrity(PersonImportStateModel state, BfsIntegrityEntity integrity)
    {
        var entities = state.EntitiesUnchanged
            .Concat(state.EntitiesToCreate.Where(x => x is { IsDeleted: false, IsLatest: true }))
            .Concat(state.EntitiesToUpdate.Where(x => x is { IsDeleted: false, IsLatest: true }))
            .OrderBy(x => x.Id)
            .ToList();

        _createSignatureService.SignIntegrity(integrity, entities);
    }

    protected override async Task InitializeState(PersonImportStateModel state)
    {
        if (!state.MunicipalityId.HasValue)
        {
            return;
        }

        EnsurePersonImportSourceSystemIsAllowed(state.MunicipalityId.Value, state.ImportSourceSystem);

        state.PersonDoisFromAclByBfs = await FetchPersonDoisFromAclByBfs(state.MunicipalityId.Value);

        state.InitializeExistingPersons(
            await _personRepository.GetLatestPersonsByBfsNumberIgnoreAcl(state.MunicipalityId.Value),
            await _domainOfInfluenceRepository.GetDomainOfInfluencesByIdForBfsNumber(state.MunicipalityId.Value),
            _personMapper);
        state.EVotingEnabledVns = await _eVotersCache.GetEnabledAhvN13ForCantonWithMunicipalityId(state.MunicipalityId.Value);
    }

    protected override void MarkUnprocessedAsDeleted(PersonImportStateModel state)
        => state.SoftDeleteUnprocessed(_clock.UtcNow);

    protected override bool IsImportedDataEqual(PersonEntity updatedEntity, PersonEntity existingEntity)
        => PersonEntityImportedDataComparer.Instance.Equals(updatedEntity, existingEntity);

    protected override Task PostImportProcessingAssertion(PersonImportStateModel importState)
    {
        AssertForDuplicatedSourceId(importState);
        AssertForDuplicatedAhvn13(importState);
        return Task.CompletedTask;
    }

    protected override async Task PostProcessImport(PersonImportStateModel state)
    {
        if (state.ImportSourceSystem == ImportSourceSystem.Cobra)
        {
            return;
        }

        await _bfsStatisticService.CreateOrUpdateStatistics(state);
    }

    private static bool IsDoiTypeImportedViaAcl(DomainOfInfluenceType type)
    {
        return type
            is DomainOfInfluenceType.Ch
            or DomainOfInfluenceType.Ct
            or DomainOfInfluenceType.Bz
            or DomainOfInfluenceType.Mu;
    }

    private async Task<IReadOnlySet<PersonDoiEntity>> FetchPersonDoisFromAclByBfs(int bfs)
    {
        var aclEntries = await AclDoiService.GetEntriesForBfsIncludingParents(bfs.ToString());

        // only a pre-defined set of doi levels is synchronized from the access control list.
        var filteredEntries = aclEntries.Where(personDoi => IsDoiTypeImportedViaAcl(personDoi.Type)).ToList();

        if (filteredEntries.Count == 0)
        {
            throw new ImportException($"No acl entry of type MU or CH, CT AND BZ found for BFS number {bfs}");
        }

        return filteredEntries.Select(a =>
            new PersonDoiEntity
            {
                DomainOfInfluenceType = a.Type,
                Canton = a.Canton.ToString(),
                Name = a.Name,
                Identifier = a.Bfs ?? string.Empty,
                PersonId = Guid.Empty,
                Id = Guid.Empty,
            })
            .ToHashSet();
    }

    /// <summary>
    /// Ensures that the person import source system is allowed.
    /// If no configuration is specified for a municipality id or the starting date is in the future
    /// the import source system must be <see cref="ImportSourceSystem.Loganto"/>.
    /// </summary>
    private void EnsurePersonImportSourceSystemIsAllowed(int municipalityId, ImportSourceSystem importSourceSystem)
    {
        if (importSourceSystem == ImportSourceSystem.Cobra &&
            ImportConfig.SwissAbroadMunicipalityIdWhitelist.Contains(municipalityId.ToString()))
        {
            return;
        }

        if (!ImportConfig.AllowedPersonImportSourceSystemByMunicipalityId.ContainsKey(municipalityId) &&
            importSourceSystem != ImportSourceSystem.Loganto)
        {
            throw new ImportException(
                $"No allowed person import source system configuration is specified for the MunicipalityId '{municipalityId}'. Import source '{importSourceSystem}' is not allowed.");
        }

        if (ImportConfig.AllowedPersonImportSourceSystemByMunicipalityId.TryGetValue(municipalityId, out var config) &&
            ((config.StartingDate < _clock.UtcNow && config.ImportSourceSystem != importSourceSystem) || (config.StartingDate > _clock.UtcNow && importSourceSystem != ImportSourceSystem.Loganto)))
        {
            throw new ImportException(
                $"MunicipalityId '{municipalityId}' does not allow person imports from import source '{importSourceSystem}'. Matches item with starting date '{config.StartingDate.ToUniversalTime():dd.MM.yyyy HH:mm:ss}'");
        }
    }

    /// <summary>
    /// If a duplicated SourceSystemId exists within the import file, the import must be aborted.
    /// Throws a <see cref="ImportException"/> if the SourceSystemId is not unique in the import file.
    /// </summary>
    /// <param name="state">The import state type.</param>
    private void AssertForDuplicatedSourceId(PersonImportStateModel state)
    {
        var duplicates = state.EntitiesUnchanged
            .Concat(state.EntitiesToCreate.Where(x => !x.IsDeleted && x.IsLatest))
            .Concat(state.EntitiesToUpdate.Where(x => !x.IsDeleted && x.IsLatest))
            .Where(p => p.SourceSystemId != null)
            .GroupBy(p => p.SourceSystemId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Any())
        {
            var duplicateIdentifiers = string.Join(", ", duplicates);
            _logger.LogError("Not allowed person import containing duplicated SourceId ({sourceSystemIds}). Import from source system '{importSourceSystem}' rejected.", duplicateIdentifiers, state.ImportSourceSystem);
            throw new ImportException($"Not allowed person import containing duplicated SourceId ({duplicateIdentifiers}). Import from source system '{state.ImportSourceSystem}' rejected.");
        }
    }

    /// <summary>
    /// If a duplicate person with the same AHVN13 exists within the import file, the import must be aborted.
    /// Throws a <see cref="ImportException"/> if a duplicated AHVN13 is detected.
    /// </summary>
    /// <param name="state">The import state type.</param>
    private void AssertForDuplicatedAhvn13(PersonImportStateModel state)
    {
        var duplicates = state.EntitiesUnchanged
            .Concat(state.EntitiesToCreate.Where(x => !x.IsDeleted && x.IsLatest))
            .Concat(state.EntitiesToUpdate.Where(x => !x.IsDeleted && x.IsLatest))
            .Where(p => p.Vn != null)
            .GroupBy(p => p.Vn)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Any())
        {
            var duplicateIdentifiers = string.Join(", ", duplicates);
            _logger.LogError("Not allowed person import containing duplicated AHVN13 ({ahnv13}). Import from source system '{importSourceSystem}' rejected.", duplicateIdentifiers, state.ImportSourceSystem);
            throw new ImportException($"Not allowed person import containing duplicated AHVN13 ({duplicateIdentifiers}). Import from source system '{state.ImportSourceSystem}' rejected.");
        }
    }
}
