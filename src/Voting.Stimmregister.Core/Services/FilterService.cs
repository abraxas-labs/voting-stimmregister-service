// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Voting.Lib.Iam.Exceptions;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Core.Configuration;
using Voting.Stimmregister.Core.Diagnostics;
using Voting.Stimmregister.Core.Services.Supporting.Signing;
using Voting.Stimmregister.Domain.Cryptography;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Exceptions;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services;

/// <inheritdoc cref="IFilterService"/>
public class FilterService : IFilterService
{
    private readonly IPermissionService _permission;
    private readonly IFilterRepository _filterRepository;
    private readonly IFilterCriteriaRepository _filterCriteriaRepository;
    private readonly IFilterVersionRepository _filterVersionRepository;
    private readonly IFilterVersionPersonRepository _filterVersionPersonRepository;
    private readonly IPersonRepository _personRepository;
    private readonly ICreateSignatureService _createSignatureService;
    private readonly FilterConfig _filterConfig;
    private readonly PersonConfig _personConfig;
    private readonly IDataContext _dataContext;
    private readonly ActivityFactory<FilterService> _activityFactory;
    private readonly ILogger<FilterService> _logger;
    private readonly IBatchInserter _batchInserter;
    private readonly BfsIntegrityPersonsVerifier _bfsIntegrityPersonsVerifier;
    private readonly IVerifySigningService _verifySigningService;

    public FilterService(
        IFilterRepository filterRepository,
        IFilterCriteriaRepository filterCriteriaRepository,
        IFilterVersionRepository filterVersionRepository,
        IFilterVersionPersonRepository filterVersionPersonRepository,
        IPersonRepository personRepository,
        ICreateSignatureService createSignatureService,
        IPermissionService permission,
        FilterConfig filterConfig,
        PersonConfig personConfig,
        IDataContext dataContext,
        ActivityFactory<FilterService> activityFactory,
        ILogger<FilterService> logger,
        IBatchInserter batchInserter,
        BfsIntegrityPersonsVerifier bfsIntegrityPersonsVerifier,
        IVerifySigningService verifySigningService)
    {
        _filterRepository = filterRepository;
        _filterCriteriaRepository = filterCriteriaRepository;
        _filterVersionRepository = filterVersionRepository;
        _filterVersionPersonRepository = filterVersionPersonRepository;
        _personRepository = personRepository;
        _createSignatureService = createSignatureService;
        _permission = permission;
        _filterConfig = filterConfig;
        _personConfig = personConfig;
        _dataContext = dataContext;
        _activityFactory = activityFactory;
        _logger = logger;
        _batchInserter = batchInserter;
        _bfsIntegrityPersonsVerifier = bfsIntegrityPersonsVerifier;
        _verifySigningService = verifySigningService;
    }

    public async Task<List<FilterEntity>> GetAll(FilterSearchParametersModel searchParameters)
    {
        var queryable = _filterRepository.Query();
        var searchResult = await queryable
            .Include(filter => filter.FilterCriterias.OrderBy(fc => fc.SortIndex))
            .Include(filter => filter.FilterVersions
                .OrderByDescending(x => x.Deadline)
                .ThenByDescending(x => x.AuditInfo.CreatedAt)
                .Take(1))
            .OrderByDescending(filter => filter.AuditInfo.CreatedAt)
            .ToListAsync();

        foreach (var item in searchResult)
        {
            ApplyComputedProperties(item);

            // clear the collection since we only load the most recent version,
            // this could be confusing otherwise.
            item.FilterVersions = new HashSet<FilterVersionEntity>();
        }

        return searchResult;
    }

    public async Task<FilterEntity> GetSingleById(Guid id)
    {
        var filter = await GetSingleFilterByIdIncludingFilterCriteriasAndVersions(id)
            ?? throw new EntityNotFoundException(id);

        ApplyComputedProperties(filter);
        return filter;
    }

    public async Task<FilterMetadataModel> GetMetadata(Guid id, DateOnly deadline)
    {
        var filter = await _filterRepository.Query()
                .Include(x => x.FilterCriterias)
                .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new EntityNotFoundException(nameof(FilterEntity), id);

        var personCount = await _personRepository.GetCountsByFilter(filter.FilterCriterias.AsReadOnly(), deadline);
        var actuality = await GetActualityByFilter(filter.FilterCriterias.AsReadOnly(), deadline);

        return new(personCount.CountOfPersons, personCount.CountOfInvalidPersons, actuality.IsActual, actuality.ActualityDate);
    }

    public async Task<FilterVersionEntity> GetSingleVersionInclFilterByVersionId(Guid versionId)
    {
        var filterVersion = await _filterVersionRepository.Query()
                .Include(x => x.Filter)
                .SingleOrDefaultAsync(x => x.Id == versionId)
            ?? throw new EntityNotFoundException(nameof(FilterVersionEntity), versionId);

        // set filter versions to empty, otherwise only this version is included,
        // but not all.
        filterVersion.Filter!.FilterVersions = new HashSet<FilterVersionEntity>();

        return filterVersion;
    }

    public async Task DeleteSingleById(Guid id, int municipalityId)
    {
        var filter = await GetSingleFilter(id)
            ?? throw new EntityNotFoundException(id);

        if (filter.MunicipalityId != municipalityId)
        {
            throw new ForbiddenException("no municipality id found");
        }

        await _filterRepository.DeleteByKey(id);
    }

    public async Task<Guid> DuplicateSingleById(Guid id, int municipalityId)
    {
        var filter = await GetSingleFilterByIdIncludingFilterCriterias(id)
            ?? throw new EntityNotFoundException(id);

        if (filter.MunicipalityId != municipalityId)
        {
            throw new ForbiddenException("no municipality id found");
        }

        filter.Name += _filterConfig.DuplicateNameSuffix;
        var filterCriterias = filter.FilterCriterias;
        filter.FilterCriterias = new HashSet<FilterCriteriaEntity>();

        await using var transaction = await _dataContext.BeginTransaction();

        await CreateFilterEntity(filter, municipalityId);

        foreach (var criteria in filterCriterias)
        {
            await CreateFilterCriteriaEntity(filter, criteria);
        }

        await transaction.CommitAsync();
        return filter.Id;
    }

    public async Task<Guid> Save(FilterEntity filter, int municipalityId)
    {
        await using var transaction = await _dataContext.BeginTransaction();

        filter.FilterCriterias = filter.FilterCriterias.Where(x => !string.IsNullOrEmpty(x.FilterValue)).ToHashSet();

        if (filter.Id == Guid.Empty)
        {
            await CreateFilter(filter, municipalityId);
        }
        else
        {
            await UpdateFilter(filter);
        }

        await transaction.CommitAsync();
        return filter.Id;
    }

    public async Task<Guid> CreateVersion(FilterVersionEntity filterVersionEntity, CancellationToken ct)
    {
        await using var transaction = await _dataContext.BeginTransaction();
        var id = await CreateFilterVersion(filterVersionEntity, ct);
        await transaction.CommitAsync(ct);
        return id;
    }

    public async Task RenameVersion(Guid filterVersionId, string name, CancellationToken ct)
    {
        await using var transaction = await _dataContext.BeginTransaction();

        var filterVersionEntity = await GetSingleFilterVersionById(filterVersionId)
            ?? throw new EntityNotFoundException(filterVersionId);

        var verifier = _verifySigningService.CreateFilterVersionSignatureVerifier(filterVersionEntity);

        filterVersionEntity.Name = name;
        _permission.SetModified(filterVersionEntity);

        var signatureCreator = _createSignatureService.CreateFilterVersionSignatureCreator(filterVersionEntity);
        var persons = _personRepository.StreamPersonsByFilterVersion(filterVersionEntity.Id);
        await foreach (var person in persons.WithCancellation(ct))
        {
            verifier.Append(person);
            signatureCreator.Append(person);
        }

        verifier.EnsureValid();
        signatureCreator.Sign();

        await _filterVersionRepository.Update(filterVersionEntity);
        await transaction.CommitAsync(ct);
    }

    public async Task DeleteVersionById(Guid id)
    {
        var version = await GetSingleFilterVersionById(id)
            ?? throw new EntityNotFoundException(id);

        await _filterVersionRepository.DeleteByKey(version.Id);
    }

    private async Task<FilterActualityModel> GetActualityByFilter(IReadOnlyCollection<FilterCriteriaEntity> filterCriteria, DateOnly deadline)
    {
        var actualityDate = await _personRepository.GetActualityDateByFilter(filterCriteria, deadline, ImportType.Person);
        var isActual = actualityDate.HasValue && (DateTime.UtcNow - actualityDate.Value) < _personConfig.ActualityTimeSpan;

        return new FilterActualityModel(isActual, actualityDate);
    }

    private void ApplyComputedProperties(FilterEntity filter)
    {
        filter.LatestVersion = filter.FilterVersions
            .OrderByDescending(x => x.Deadline)
            .ThenByDescending(x => x.AuditInfo.CreatedAt)
            .FirstOrDefault();
    }

    private async Task CreateFilter(FilterEntity filter, int municipalityId)
    {
        var filterCriteriaEntities = filter.FilterCriterias;
        await CreateFilterEntity(filter, municipalityId);
        await CreateFilterCriterias(filterCriteriaEntities, filter);
    }

    private async Task CreateFilterEntity(FilterEntity entity, int municipalityId)
    {
        entity.Id = Guid.Empty;
        entity.MunicipalityId = municipalityId;
        entity.TenantId = _permission.TenantId;
        entity.TenantName = _permission.TenantName;
        entity.FilterCriterias = new HashSet<FilterCriteriaEntity>();
        _permission.ClearModified(entity);
        _permission.SetCreated(entity);
        await _filterRepository.Create(entity);
    }

    private async Task CreateFilterCriteriaEntity(FilterEntity entity, FilterCriteriaEntity criteriaEntity)
    {
        criteriaEntity.Id = Guid.Empty;
        criteriaEntity.FilterId = entity.Id;
        _permission.ClearModified(criteriaEntity);
        _permission.SetCreated(criteriaEntity);
        await _filterCriteriaRepository.Create(criteriaEntity);
    }

    private async Task<Guid> CreateFilterVersion(FilterVersionEntity filterVersion, CancellationToken ct)
    {
        using var activity = _activityFactory.Start("create-filter-version");

        var filter = await _filterRepository.Query()
            .Include(x => x.FilterCriterias.OrderBy(y => y.SortIndex))
            .FirstOrDefaultAsync(x => x.Id == filterVersion.FilterId, ct)
            ?? throw new EntityNotFoundException(filterVersion.FilterId);

        var filterVersionEntity = await CreateFilterVersionEntity(filterVersion, filter.FilterCriterias.AsReadOnly());

        var filterIncludedPersonIds = await CreateFilterVersionPersons(filterVersionEntity.Id, filter.FilterCriterias.AsReadOnly(), DateOnly.FromDateTime(filterVersion.Deadline), ct);

        (filterVersionEntity.Count, filterVersionEntity.CountOfInvalidPersons) = await _filterVersionPersonRepository.CountIgnoreAcl(filterVersionEntity.Id);
        await VerifyBfsIntegrityAndSignFilterVersion(filterVersionEntity, filterIncludedPersonIds, ct);
        await _filterVersionRepository.UpdateIgnoreRelations(filterVersionEntity);

        if (filterVersionEntity.CountOfInvalidPersons > 0)
        {
            _logger.LogWarning(
                "Created filter version {FilterVersionId} with {Count} invalid persons out of {TotalCount}",
                filterVersionEntity.Id,
                filterVersionEntity.CountOfInvalidPersons,
                filterVersionEntity.Count);
        }

        return filterVersionEntity.Id;
    }

    private async Task<IReadOnlySet<Guid>> CreateFilterVersionPersons(
        Guid filterVersionId,
        IReadOnlyCollection<FilterCriteriaEntity> criteria,
        DateOnly deadline,
        CancellationToken ct)
    {
        using var activity = _activityFactory.Start("filter-version-create-persons");
        var personIds = await _personRepository.ListPersonIdsInclInvalid(criteria, deadline);
        var filterVersionPersons = personIds.Select(personId =>
        {
            var filterVersionPerson = new FilterVersionPersonEntity
            {
                PersonId = personId,
                FilterVersionId = filterVersionId,
            };
            _permission.SetCreated(filterVersionPerson);
            return filterVersionPerson;
        });

        await _batchInserter.InsertBatched(filterVersionPersons, ct);
        return personIds;
    }

    private async Task VerifyBfsIntegrityAndSignFilterVersion(
        FilterVersionEntity filterVersion,
        IReadOnlySet<Guid> filterIncludedPersonIds,
        CancellationToken ct)
    {
        using var activity = _activityFactory.Start("sign-filter-version");

        var bfsNumbers = await _filterVersionPersonRepository.ListBfsIgnoreAcl(filterVersion.Id);
        var filterVersionSignatureCreator = _createSignatureService.CreateFilterVersionSignatureCreator(filterVersion);

        void ProcessPerson(PersonEntity p)
        {
            if (filterIncludedPersonIds.Contains(p.Id))
            {
                filterVersionSignatureCreator.Append(p);
            }
        }

        await _bfsIntegrityPersonsVerifier.Verify(bfsNumbers, ProcessPerson, ct);
        filterVersionSignatureCreator.Sign();
    }

    private async Task<FilterVersionEntity> CreateFilterVersionEntity(FilterVersionEntity filterVersion, IReadOnlyCollection<FilterCriteriaEntity> criteria)
    {
        using var activity = _activityFactory.Start("create-filter-version-entity");

        foreach (var criteriaEntity in criteria)
        {
            criteriaEntity.Id = Guid.Empty;
            criteriaEntity.FilterId = null;
            criteriaEntity.Filter = null;
            filterVersion.FilterCriterias.Add(criteriaEntity);
        }

        _permission.SetCreated(filterVersion);
        await _filterVersionRepository.Create(filterVersion);
        return filterVersion;
    }

    private async Task UpdateFilter(FilterEntity updateFilter)
    {
        var existingFilter = await GetSingleFilterByIdIncludingFilterCriterias(updateFilter.Id)
            ?? throw new EntityNotFoundException(updateFilter.Id);

        existingFilter.Name = updateFilter.Name;
        existingFilter.Description = updateFilter.Description;
        _permission.SetModified(existingFilter);

        await _filterRepository.Update(existingFilter);
        await SyncFilterCriterias(updateFilter, existingFilter);
    }

    private async Task SyncFilterCriterias(FilterEntity updateFilter, FilterEntity existingFilter)
    {
        var existingFilterCriteriaByReferenceId = existingFilter.FilterCriterias.ToDictionary(fc => fc.ReferenceId);
        var updatedFilterCirteriaByReferenceId = updateFilter.FilterCriterias.ToDictionary(fc => fc.ReferenceId);

        foreach (var referenceId in updatedFilterCirteriaByReferenceId.Keys)
        {
            var updateFilterCriteria = updatedFilterCirteriaByReferenceId[referenceId];

            if (!existingFilterCriteriaByReferenceId.TryGetValue(referenceId, out var existingFilterCriteria))
            {
                await CreateFilterCriteriaEntity(existingFilter, updateFilterCriteria);
                continue;
            }

            if (IsFilterCriteriaUpdateRequired(existingFilterCriteria, updateFilterCriteria))
            {
                await UpdateFilterCriteriaEntity(existingFilterCriteria, updateFilterCriteria);
            }

            existingFilterCriteriaByReferenceId.Remove(referenceId);
        }

        await DeleteFilterCriterias(existingFilterCriteriaByReferenceId.Values);
    }

    private async Task UpdateFilterCriteriaEntity(FilterCriteriaEntity criteriaToUpdate, FilterCriteriaEntity updateCriteria)
    {
        criteriaToUpdate.FilterValue = updateCriteria.FilterValue;
        criteriaToUpdate.FilterOperator = updateCriteria.FilterOperator;
        criteriaToUpdate.FilterType = updateCriteria.FilterType;
        _permission.SetModified(criteriaToUpdate);
        await _filterCriteriaRepository.Update(criteriaToUpdate);
    }

    private bool IsFilterCriteriaUpdateRequired(FilterCriteriaEntity existingFilterCriteria, FilterCriteriaEntity updatedFilterCriteria)
    {
        return !(string.Equals(existingFilterCriteria.FilterValue, updatedFilterCriteria.FilterValue, StringComparison.Ordinal)
            && Equals(existingFilterCriteria.FilterOperator, updatedFilterCriteria.FilterOperator)
            && Equals(existingFilterCriteria.FilterType, updatedFilterCriteria.FilterType));
    }

    private async Task CreateFilterCriterias(IEnumerable<FilterCriteriaEntity> filterCriterias, FilterEntity filter)
    {
        var i = 0;
        foreach (var criteria in filterCriterias)
        {
            criteria.SortIndex = i++;
            await CreateFilterCriteriaEntity(filter, criteria);
        }
    }

    private async Task DeleteFilterCriterias(IReadOnlyCollection<FilterCriteriaEntity> filterCriterias)
    {
        if (filterCriterias.Count == 0)
        {
            return;
        }

        // The repository method DeleteRangeByKey cannot be used for deletion, because filter criteria use query filters
        // targeting relational entities which is not supported by the use of ExecuteDeleteAsync. Therefore, we have to ignore
        // the query filters for deletion.
        var ids = filterCriterias.Select(e => e.Id).ToList();
        await _filterCriteriaRepository.Query().IgnoreQueryFilters().Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync();
    }

    private async Task<FilterEntity?> GetSingleFilter(Guid id)
    {
        return await _filterRepository.Query()
            .SingleOrDefaultAsync(filter => filter.Id == id);
    }

    private async Task<FilterEntity?> GetSingleFilterByIdIncludingFilterCriterias(Guid id)
    {
        return await _filterRepository.Query()
            .Include(filter => filter.FilterCriterias.OrderBy(fc => fc.SortIndex))
            .SingleOrDefaultAsync(filter => filter.Id == id);
    }

    private async Task<FilterEntity?> GetSingleFilterByIdIncludingFilterCriteriasAndVersions(Guid id)
    {
        return await _filterRepository.Query()
            .Include(filter => filter.FilterVersions
                .OrderByDescending(x => x.Deadline)
                .ThenByDescending(x => x.AuditInfo.CreatedAt))
            .ThenInclude(version => version.FilterCriterias.OrderBy(fc => fc.SortIndex))
            .Include(filter => filter.FilterCriterias.OrderBy(fc => fc.SortIndex))
            .SingleOrDefaultAsync(filter => filter.Id == id);
    }

    private async Task<FilterVersionEntity?> GetSingleFilterVersionById(Guid id)
    {
        return await _filterVersionRepository.Query()
            .SingleOrDefaultAsync(version => version.Id == id);
    }
}
