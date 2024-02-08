// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Threading.Tasks;
using Voting.Lib.Database.Models;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services;

public class LastSearchParameterService : ILastSearchParameterService
{
    private readonly IPermissionService _permissionService;
    private readonly ILastSearchParameterRepository _lastSearchParameterRepository;

    public LastSearchParameterService(IPermissionService permissionService, ILastSearchParameterRepository lastSearchParameterRepository)
    {
        _permissionService = permissionService;
        _lastSearchParameterRepository = lastSearchParameterRepository;
    }

    public async Task<LastSearchParameterEntity> GetForCurrentUserAndTenant(PersonSearchType searchType)
    {
        return await _lastSearchParameterRepository.Fetch(searchType, _permissionService.UserId, _permissionService.TenantId)
            ?? new LastSearchParameterEntity { SearchType = searchType };
    }

    public async Task Store(PersonSearchType searchType, Pageable? pageable, List<FilterCriteriaEntity> criteria)
    {
        var entity = new LastSearchParameterEntity
        {
            SearchType = searchType,
            TenantId = _permissionService.TenantId,
            UserId = _permissionService.UserId,
            FilterCriteria = criteria,
            PageInfo = pageable ?? new Pageable(1, 20),
        };

        var i = 0;
        foreach (var criteriaEntity in criteria)
        {
            criteriaEntity.SortIndex = i++;
            _permissionService.SetCreated(criteriaEntity);
        }

        await _lastSearchParameterRepository.CreateOrReplace(entity);
    }
}
