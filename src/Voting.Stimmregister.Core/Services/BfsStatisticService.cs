// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Abstractions.Core.Import.Models;
using Voting.Stimmregister.Abstractions.Core.Import.Services;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.BfsStatistic;

namespace Voting.Stimmregister.Core.Services;

public class BfsStatisticService : IBfsStatisticService
{
    private readonly IBfsStatisticRepository _bfsStatisticRepository;
    private readonly IPermissionService _permissionService;
    private readonly IPersonService _personService;

    public BfsStatisticService(
        IBfsStatisticRepository bfsStatisticRepository,
        IPermissionService permissionService,
        IPersonService personService)
    {
        _bfsStatisticRepository = bfsStatisticRepository;
        _permissionService = permissionService;
        _personService = personService;
    }

    public async Task<BfsStatisticModel> GetStatistics(bool disableQueryFilter = false, short? cantonBfs = null)
    {
        var queryable = _bfsStatisticRepository.Query();
        var totalStatistic = new BfsStatisticEntity();

        if (disableQueryFilter)
        {
            queryable = queryable.IgnoreQueryFilters();
        }

        if (cantonBfs != null)
        {
            queryable = queryable.Where(b => b.CantonBfs.Equals(cantonBfs));
            totalStatistic.Bfs = cantonBfs.Value.ToString();
        }

        var municipalityStatistics = await queryable.OrderBy(b => b.BfsName).ToListAsync();

        foreach (var municipalityStatistic in municipalityStatistics)
        {
            totalStatistic.EVoterTotalCount += municipalityStatistic.EVoterTotalCount;
            totalStatistic.VoterTotalCount += municipalityStatistic.VoterTotalCount;
        }

        return new BfsStatisticModel
        {
            MunicipalityStatistics = municipalityStatistics,
            TotalStatistic = totalStatistic,
        };
    }

    public async Task CreateOrUpdateStatistics(PersonImportStateModel state)
    {
        var evoterTotalCount =
            state.EntitiesToCreate.Count(e => !e.IsDeleted && e.EVoting)
            + state.EntitiesToUpdate.Count(e => e.IsLatest && e.EVoting)
            + state.EntitiesUnchanged.Count(e => e.EVoting);

        var createdTotalCount = await state.EntitiesToCreate
            .ToAsyncEnumerable()
            .SelectAwait(async e => await _personService.GetPersonModelFromEntity(e, true, false))
            .CountAsync(e => !e.IsDeleted && e.IsVotingAllowed);

        var updatedTotalCount = await state.EntitiesToUpdate
            .ToAsyncEnumerable()
            .SelectAwait(async e => await _personService.GetPersonModelFromEntity(e, true, false))
            .CountAsync(e => e.IsLatest && e.IsVotingAllowed);

        var unchangedTotalCount = await state.EntitiesUnchanged
            .ToAsyncEnumerable()
            .SelectAwait(async e => await _personService.GetPersonModelFromEntity(e, true, false))
            .CountAsync(e => e.IsVotingAllowed);

        var voterTotalCount = createdTotalCount + updatedTotalCount + unchangedTotalCount;

        var entity = new BfsStatisticEntity
        {
            Bfs = state.MunicipalityId.ToString()!,
            BfsName = state.MunicipalityName!,
            VoterTotalCount = voterTotalCount,
            EVoterTotalCount = evoterTotalCount,
            CantonBfs = state.CantonBfs!.Value,
        };

        _permissionService.SetCreated(entity);

        await _bfsStatisticRepository.CreateOrUpdate(entity);
    }
}
