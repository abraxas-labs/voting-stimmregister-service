// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Linq;
using System.Threading.Tasks;
using Voting.Stimmregister.Abstractions.Adapter.VotingBasis;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Abstractions.Core.Import.Services;
using Voting.Stimmregister.Abstractions.Core.Services;
using Voting.Stimmregister.Domain.Models.BfsStatistic;

namespace Voting.Stimmregister.Core.Services;

/// <inheritdoc cref="IRegistrationStatisticService"/>
public class RegistrationStatisticService : IRegistrationStatisticService
{
    private readonly IBfsStatisticService _bfsStatisticService;
    private readonly IPermissionService _permissionService;
    private readonly IAccessControlListDoiService _aclDoiService;

    public RegistrationStatisticService(
        IBfsStatisticService bfsStatisticService,
        IPermissionService permissionService,
        IAccessControlListDoiService aclDoiService)
    {
        _bfsStatisticService = bfsStatisticService;
        _permissionService = permissionService;
        _aclDoiService = aclDoiService;
    }

    public async Task<RegistrationStatisticResponseModel> List()
    {
        var bfsStatistics = await _bfsStatisticService.GetStatistics();
        var doiAccessControlList = await _aclDoiService.GetDoiAccessControlListByTenantId(_permissionService.TenantId);

        return new RegistrationStatisticResponseModel
        {
            MunicipalityStatistics = bfsStatistics.MunicipalityStatistics,
            TotalStatistic = bfsStatistics.TotalStatistic,
            IsTopLevelAuthority = doiAccessControlList.Any(d => d.Parent == null),
        };
    }
}
