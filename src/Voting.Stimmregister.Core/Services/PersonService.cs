// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Voting.Lib.Common;
using Voting.Lib.Database.Models;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Core.Configuration;
using Voting.Stimmregister.Domain.Constants;
using Voting.Stimmregister.Domain.Diagnostics;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Exceptions;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Utils;

namespace Voting.Stimmregister.Core.Services;

/// <inheritdoc cref="IPersonService"/>
public class PersonService : IPersonService
{
    private readonly ILogger<IPersonService> _logger;
    private readonly IMapper _mapper;
    private readonly IClock _clock;
    private readonly IPersonRepository _personRepository;
    private readonly IFilterRepository _filterRepository;
    private readonly IBfsIntegrityRepository _bfsIntegrityRepository;
    private readonly IFilterVersionRepository _filterVersionRepository;
    private readonly ILastSearchParameterService _lastSearchParameterService;
    private readonly PersonConfig _config;

    public PersonService(
        ILogger<IPersonService> logger,
        IMapper mapper,
        IClock clock,
        IPersonRepository personRepository,
        IFilterRepository filterRepository,
        IFilterVersionRepository filterVersionRepository,
        IBfsIntegrityRepository bfsIntegrityRepository,
        PersonConfig config,
        ILastSearchParameterService lastSearchParameterService)
    {
        _logger = logger;
        _mapper = mapper;
        _clock = clock;
        _personRepository = personRepository;
        _filterRepository = filterRepository;
        _filterVersionRepository = filterVersionRepository;
        _bfsIntegrityRepository = bfsIntegrityRepository;
        _config = config;
        _lastSearchParameterService = lastSearchParameterService;
    }

    public Task<LastSearchParameterEntity> GetLastUsedParameters(PersonSearchType searchType)
        => _lastSearchParameterService.GetForCurrentUserAndTenant(searchType);

    /// <inheritdoc />
    public async Task<PersonSearchResultPage<PersonEntityModel>> GetAll(PersonSearchParametersModel searchParameters, bool requiredValidPageSize = true, bool includeDois = false)
    {
        var stopWatch = Stopwatch.StartNew();
        var referenceKeyDate = _clock.Today;

        try
        {
            ValidatePagingParameters(requiredValidPageSize, searchParameters.Paging);
            var criteria = _mapper.Map<List<FilterCriteriaEntity>>(searchParameters.Criteria);
            var searchResult = await _personRepository.GetPersonByFilter(criteria, referenceKeyDate, includeDois, searchParameters.Paging);
            await _lastSearchParameterService.Store(searchParameters.SearchType, searchParameters.Paging, criteria);
            stopWatch.Stop();
            RegisterStatistics(searchParameters, stopWatch, searchResult.TotalItemsCount);
            var personEntitySearchResult = _mapper.Map<PersonSearchResultPage<PersonEntityModel>>(
                searchResult,
                opt => opt.Items[MappingConstants.ReferenceKeyDate] = referenceKeyDate);

            DeriveComputedInfos(personEntitySearchResult.Items, referenceKeyDate);
            await DeriveActuality(personEntitySearchResult.Items);
            return personEntitySearchResult;
        }
        finally
        {
            stopWatch.Stop();
        }
    }

    public async Task<PersonEntityModel> GetSingleIncludingDoIs(Guid personRegisterId)
    {
        if (personRegisterId == Guid.Empty)
        {
            throw new InvalidSearchFilterCriteriaException("Register Id search parameter must not be null or Guid.Empty.");
        }

        var personEntity = await _personRepository.GetPersonByRegisterIdIncludingDoIs(personRegisterId);
        var person = _mapper.Map<PersonEntityModel>(personEntity, opt => opt.Items[MappingConstants.ReferenceKeyDate] = _clock.Today);
        DeriveComputedInfos(_clock.Today, person);
        await DeriveActuality(person);
        return person;
    }

    public async Task<PersonEntityModel?> GetSingleOrDefaultWithVotingRightsByVnAndCantonBfsIgnoreAcl(long vn, short cantonBfs)
    {
        var personEntity = await _personRepository.GetSingleOrDefaultWithVotingRightsByVnAndCantonBfsIgnoreAcl(vn, cantonBfs);
        if (personEntity == null)
        {
            return null;
        }

        var person = _mapper.Map<PersonEntityModel>(personEntity, opt => opt.Items[MappingConstants.ReferenceKeyDate] = _clock.Today);
        DeriveComputedInfos(_clock.Today, person);
        await DeriveActuality(person);
        return person;
    }

    public async Task<PersonSearchResultPage<PersonEntityModel>> GetByFilterVersionId(PersonSearchFilterIdParametersModel searchParameters, bool requiredValidPageSize = true, bool includeDois = false)
    {
        var referenceKeyDate = _clock.Today;

        ValidatePagingParameters(requiredValidPageSize, searchParameters.Paging);
        PersonSearchResultPage<PersonEntity> searchResult;
        if (searchParameters.VersionId != null && searchParameters.VersionId != Guid.Empty)
        {
            var versionId = searchParameters.VersionId.Value;
            var filter = await _filterVersionRepository.Query().FirstOrDefaultAsync(r => r.Id == versionId);
            if (filter == null)
            {
                throw new EntityNotFoundException(typeof(FilterVersionEntity), versionId);
            }

            referenceKeyDate = DateOnly.FromDateTime(filter.Deadline);
            searchResult = await _personRepository.GetPersonsByFilterVersionId(versionId, searchParameters.Paging);
        }
        else if (searchParameters.FilterId != Guid.Empty)
        {
            var criteriaEntities = await _filterRepository.GetCriteriasByFilterId(searchParameters.FilterId);
            searchResult = await _personRepository.GetPersonByFilter(criteriaEntities.ToList(), referenceKeyDate, includeDois, searchParameters.Paging);
        }
        else
        {
            throw new InvalidSearchFilterCriteriaException($"Filter Version Id '{searchParameters.VersionId}' or Filter Id '{searchParameters.FilterId}' must be set.");
        }

        var personEntitySearchResult = _mapper.Map<PersonSearchResultPage<PersonEntityModel>>(searchResult);
        DeriveComputedInfos(personEntitySearchResult.Items, referenceKeyDate);
        await DeriveActuality(personEntitySearchResult.Items);
        return personEntitySearchResult;
    }

    public IAsyncEnumerable<PersonEntity> StreamAll(IReadOnlyCollection<PersonSearchFilterCriteriaModel> criteria)
        => StreamAll(_mapper.Map<List<FilterCriteriaEntity>>(criteria));

    public IAsyncEnumerable<PersonEntity> StreamAll(IReadOnlyCollection<FilterCriteriaEntity> criteria)
    {
        var referenceKeyDate = _clock.Today;
        return _personRepository.StreamPersons(criteria, referenceKeyDate);
    }

    public Task<PersonSearchStreamedResultModel<PersonEntity>> StreamAllWithCounts(IReadOnlyCollection<PersonSearchFilterCriteriaModel> criteria)
        => StreamAllWithCount(_mapper.Map<List<FilterCriteriaEntity>>(criteria));

    public async Task<PersonSearchStreamedResultModel<PersonEntity>> StreamAllWithCount(IReadOnlyCollection<FilterCriteriaEntity> criteria)
    {
        var referenceKeyDate = _clock.Today;
        return await _personRepository.StreamPersonsWithCounts(criteria, referenceKeyDate);
    }

    public async Task<IAsyncEnumerable<PersonEntity>> StreamAllByFilter(Guid filterId)
    {
        var criteriaEntities = await _filterRepository.GetCriteriasByFilterId(filterId);
        return StreamAll(criteriaEntities);
    }

    public async Task<PersonSearchStreamedResultModel<PersonEntity>> StreamAllWithCountsByFilter(Guid filterId)
    {
        var criteriaEntities = await _filterRepository.GetCriteriasByFilterId(filterId);
        return await StreamAllWithCount(criteriaEntities);
    }

    public IAsyncEnumerable<PersonEntity> StreamAllByFilterVersion(Guid filterVersionId)
        => _personRepository.StreamPersonsByFilterVersion(filterVersionId);

    public async Task<PersonSearchStreamedResultModel<PersonEntity>> StreamAllWithCountsByFilterVersion(Guid filterVersionId)
    {
        return await _personRepository.StreamPersonsByFilterVersionWithCount(filterVersionId);
    }

    private static void ValidatePagingParameters(bool requiredValidPageSize, Pageable? paging)
    {
        if (requiredValidPageSize && paging?.PageSize is <= 0 or > 100)
        {
            // never retrieve all records from the user interface:
            throw new InvalidSearchFilterCriteriaException($"Page size '{paging?.PageSize}' out of range (1 - 100)");
        }
    }

    private static bool IsBirthDateValidForVotingRights(DateOnly referenceKeyDate, DateOnly dateOfBirth)
    {
        const int ageOfMajority = 18;
        return referenceKeyDate.AddYears(-ageOfMajority).CompareTo(dateOfBirth) >= 0 && dateOfBirth > DateOnly.MinValue;
    }

    private static bool IsReferenceKeyDateAfterOrEqualMoveInArrivalDate(DateOnly referenceKeyDate, DateOnly? moveInArrivalDate)
    {
        // An empty moveInArrivalDate will result in true, as this is valid from a business point of view.
        return referenceKeyDate.CompareTo(moveInArrivalDate) >= 0;
    }

    private static bool IsVotingAllowed(PersonEntityModel person, DateOnly referenceKeyDate)
    {
        return IsBirthDateValidForVotingRights(referenceKeyDate, person.DateOfBirth) &&
               IsReferenceKeyDateAfterOrEqualMoveInArrivalDate(referenceKeyDate, person.MoveInArrivalDate) &&
            person is
            {
                RestrictedVotingAndElectionRightFederation: false,
                IsNationalityValidForVotingRights: true,
            };
    }

    private void DeriveComputedInfos(IReadOnlyCollection<PersonEntityModel> persons, DateOnly referenceKeyDate)
    {
        foreach (var person in persons)
        {
            DeriveComputedInfos(referenceKeyDate, person);
        }
    }

    private void DeriveComputedInfos(DateOnly referenceKeyDate, PersonEntityModel person)
    {
        person.IsNationalityValidForVotingRights = IsNationalityValidForVotingRights(person.Country);
        person.IsBirthDateValidForVotingRights = IsBirthDateValidForVotingRights(referenceKeyDate, person.DateOfBirth);
        person.IsVotingAllowed = IsVotingAllowed(person, referenceKeyDate);
    }

    private async Task DeriveActuality(IReadOnlyCollection<PersonEntityModel> persons)
    {
        var bfs = persons.Select(p => p.MunicipalityId.ToString()).ToHashSet();
        var integrityByBfs = await _bfsIntegrityRepository.ListForBfs(ImportType.Person, bfs);
        foreach (var person in persons)
        {
            if (!integrityByBfs.TryGetValue(person.MunicipalityId.ToString(), out var integrity))
            {
                continue;
            }

            person.ActualityDate = integrity.LastUpdated;
            person.Actuality = _clock.UtcNow - integrity.LastUpdated < _config.ActualityTimeSpan;
        }
    }

    private async Task DeriveActuality(PersonEntityModel person)
    {
        var integrity = await _bfsIntegrityRepository.Get(ImportType.Person, person.MunicipalityId.ToString());
        if (integrity != null)
        {
            person.ActualityDate = integrity.LastUpdated;
            person.Actuality = _clock.UtcNow - integrity.LastUpdated < _config.ActualityTimeSpan;
        }
    }

    private bool IsNationalityValidForVotingRights(string? country)
        => country?.Equals(Countries.Switzerland, StringComparison.OrdinalIgnoreCase) == true;

    private void RegisterStatistics(PersonSearchParametersModel searchParameters, Stopwatch stopWatch, int resultsCount)
    {
        _logger.LogDebug("Person search result processed within '{ElapsedMs}' ms", stopWatch.Elapsed.TotalMilliseconds);
        DiagnosticsConfig.RegisterPersonSearchQueryingTime(resultsCount, searchParameters.Criteria.Count, stopWatch.Elapsed);
    }
}
