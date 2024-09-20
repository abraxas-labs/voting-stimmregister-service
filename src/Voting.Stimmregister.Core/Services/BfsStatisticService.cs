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

namespace Voting.Stimmregister.Core.Services;

public class BfsStatisticService : IBfsStatisticService
{
    private readonly IEVoterAuditRepository _evoterAuditRepo;
    private readonly IBfsStatisticRepository _bfsStatisticRepository;
    private readonly IPermissionService _permissionService;
    private readonly IPersonService _personService;

    public BfsStatisticService(
        IEVoterAuditRepository evoterAuditRepo,
        IBfsStatisticRepository bfsStatisticRepository,
        IPermissionService permissionService,
        IPersonService personService)
    {
        _evoterAuditRepo = evoterAuditRepo;
        _bfsStatisticRepository = bfsStatisticRepository;
        _permissionService = permissionService;
        _personService = personService;
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
            EVoterRegistrationCount = evoterRegistrationCount,
            EVoterDeregistrationCount = evoterDeregistrationCount,
        };

        _permissionService.SetCreated(entity);

        await _bfsStatisticRepository.CreateOrUpdate(entity);
    }
}
