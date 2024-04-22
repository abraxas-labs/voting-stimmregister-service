// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Abstractions.Core.Import.Models;
using Voting.Stimmregister.Abstractions.Core.Import.Services;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services;

public class BfsStatisticService : IBfsStatisticService
{
    private readonly IEVoterAuditRepository _evoterAuditRepo;
    private readonly IBfsStatisticRepository _bfsStatisticRepository;
    private readonly IPermissionService _permissionService;

    public BfsStatisticService(
        IEVoterAuditRepository evoterAuditRepo,
        IBfsStatisticRepository bfsStatisticRepository,
        IPermissionService permissionService)
    {
        _evoterAuditRepo = evoterAuditRepo;
        _bfsStatisticRepository = bfsStatisticRepository;
        _permissionService = permissionService;
    }

    public async Task CreateOrUpdateStatistics(PersonImportStateModel state)
    {
        var evoterRegistrationCount = await _evoterAuditRepo
            .Query()
            .CountAsync(v => v.BfsMunicipality == state.MunicipalityId && v.EVoterFlag == true);

        var evoterDeregistrationCount = await _evoterAuditRepo
            .Query()
            .CountAsync(v => v.BfsMunicipality == state.MunicipalityId && v.EVoterFlag == false);

        var evoterTotalCount =
            state.EntitiesToCreate.Count(e => e.EVoting)
            + state.EntitiesToUpdate.Count(e => e.IsLatest && e.EVoting)
            + state.EntitiesUnchanged.Count(e => e.EVoting);

        var voterTotalCount =
            state.CreateCount
            + state.UpdateCount
            + state.EntitiesUnchanged.Count;

        var entity = new BfsStatisticEntity
        {
            Bfs = state.MunicipalityId.ToString()!,
            BfsName = state.MunicipalityName!,
            VoterTotalCount = voterTotalCount,
            EVoterTotalCount = evoterTotalCount,
            EVoterRegistrationCount = evoterRegistrationCount,
            EVoterDeregistrationCount = evoterDeregistrationCount,
        };

        _permissionService.SetCreated(entity);

        await _bfsStatisticRepository.CreateOrUpdate(entity);
    }
}
