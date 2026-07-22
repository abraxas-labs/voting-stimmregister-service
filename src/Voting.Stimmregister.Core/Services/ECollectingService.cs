// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Voting.Lib.Common;
using Voting.Lib.Database.Models;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Core.Services.Supporting.Voting;
using Voting.Stimmregister.Domain.Exceptions;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Utils;

namespace Voting.Stimmregister.Core.Services;

public class ECollectingService : IECollectingService
{
    private const int MaxPageSize = 10;

    private readonly IPersonRepository _personRepository;
    private readonly IVotingDerivedInfos _votingDerivedInfos;
    private readonly IClock _clock;
    private readonly IMapper _mapper;

    public ECollectingService(
        IPersonRepository personRepository,
        IVotingDerivedInfos votingDerivedInfos,
        IClock clock,
        IMapper mapper)
    {
        _personRepository = personRepository;
        _votingDerivedInfos = votingDerivedInfos;
        _clock = clock;
        _mapper = mapper;
    }

    public async Task<ECollectingPersonEntityModel> GetPersonWithVotingRightByVn(ECollectingPersonSearchByVnParametersModel searchModel)
    {
        // The same person with the same social security number
        // could be present in the reigster in multiple municipalities (e.g. Haupt- und Nebenwohnsitz).
        // As we may need to know whether a person has the voting right anywhere in the canton
        // we load all matching persons, determine the voting right and return the first
        // person with voting right or 404 if none has a voting right.
        var persons = await _personRepository.GetLatestByVnAndBfsIgnoreAcl(searchModel);
        if (persons.Count == 0)
        {
            throw new EntityNotFoundException(nameof(PersonEntity), searchModel);
        }

        var personModels = _mapper.Map<List<ECollectingPersonEntityModel>>(persons);
        foreach (var person in personModels)
        {
            ComputeInfos(person);

            if (person.IsVotingAllowed)
            {
                return person;
            }
        }

        throw new EntityNotFoundException(nameof(PersonEntity), searchModel);
    }

    public async Task<Page<ECollectingPersonEntityModel>> GetPeopleByName(ECollectingPeopleSearchByNameParametersModel searchModel)
    {
        if (string.IsNullOrWhiteSpace(searchModel.FirstNameOrCallName)
            && string.IsNullOrWhiteSpace(searchModel.OfficialName)
            && string.IsNullOrWhiteSpace(searchModel.AddressStreet)
            && string.IsNullOrWhiteSpace(searchModel.AddressHouseNumber))
        {
            throw new ValidationException($"At least one field out of [{nameof(searchModel.FirstNameOrCallName)}, {nameof(searchModel.OfficialName)}, {nameof(searchModel.AddressStreet)}, {nameof(searchModel.AddressHouseNumber)}] is required");
        }

        if (searchModel.Pageable?.PageSize > MaxPageSize)
        {
            throw new ValidationException($"Page size too large, maximum is {MaxPageSize}");
        }

        var people = await _personRepository.GetByName(searchModel);
        var personEntityModels = _mapper.Map<Page<ECollectingPersonEntityModel>>(people);
        foreach (var personEntityModel in personEntityModels.Items)
        {
            ComputeInfos(personEntityModel, searchModel.ActualityDate);

            if (personEntityModel.IsVotingAllowed)
            {
                personEntityModel.Age = PersonUtil.GetAgeFrom(personEntityModel.DateOfBirth, DateOnly.FromDateTime(_clock.UtcNow));
            }
        }

        return personEntityModels;
    }

    public async Task<List<ECollectingPersonEntityModel>> GetPeopleByIds(ECollectingPeopleSearchByIdsParametersModel searchModel)
    {
        var people = await _personRepository.GetByIds(searchModel);
        var personEntityModels = _mapper.Map<List<ECollectingPersonEntityModel>>(people);
        foreach (var personEntityModel in personEntityModels)
        {
            ComputeInfos(personEntityModel);
        }

        return personEntityModels;
    }

    private void ComputeInfos(ECollectingPersonEntityModel person, DateOnly? referenceDate = null)
    {
        _votingDerivedInfos.ComputeInfos(person, referenceDate ?? _clock.Today);

        if (person.IsVotingAllowed)
        {
            person.Age = PersonUtil.GetAgeFrom(person.DateOfBirth, DateOnly.FromDateTime(_clock.UtcNow));
        }
    }
}
