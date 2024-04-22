// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.VotingBasis;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.RegistrationStatistic;

namespace Voting.Stimmregister.Core.Services;

/// <inheritdoc cref="IRegistrationStatisticService"/>
public class RegistrationStatisticService : IRegistrationStatisticService
{
    private readonly IBfsStatisticRepository _bfsStatisticRepository;
    private readonly IPermissionService _permissionService;
    private readonly IAccessControlListDoiService _aclDoiService;

    public RegistrationStatisticService(
        IBfsStatisticRepository bfsStatisticRepository,
        IPermissionService permissionService,
        IAccessControlListDoiService aclDoiService)
    {
        _bfsStatisticRepository = bfsStatisticRepository;
        _permissionService = permissionService;
        _aclDoiService = aclDoiService;
    }

    public async Task<RegistrationStatisticResponseModel> List()
    {
        var municipalityStatistics = await _bfsStatisticRepository.Query().OrderBy(b => b.BfsName).ToListAsync();

        var totalStatistic = new BfsStatisticEntity();

        foreach (var municipalityStatistic in municipalityStatistics)
        {
            totalStatistic.EVoterRegistrationCount += municipalityStatistic.EVoterRegistrationCount;
            totalStatistic.EVoterDeregistrationCount += municipalityStatistic.EVoterDeregistrationCount;
            totalStatistic.EVoterTotalCount += municipalityStatistic.EVoterTotalCount;
            totalStatistic.VoterTotalCount += municipalityStatistic.VoterTotalCount;
        }

        var doiAccessControlList = await _aclDoiService.GetDoiAccessControlListByTenantId(_permissionService.TenantId);

        return new RegistrationStatisticResponseModel
        {
            MunicipalityStatistics = municipalityStatistics,
            TotalStatistic = totalStatistic,
            IsTopLevelAuthority = doiAccessControlList.Any(d => d.Parent == null),
        };
    }
}
