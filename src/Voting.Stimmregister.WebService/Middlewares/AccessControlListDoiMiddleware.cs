// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Voting.Stimmregister.Abstractions.Adapter.VotingBasis;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.WebService.Configuration;

namespace Voting.Stimmregister.WebService.Middlewares;

public class AccessControlListDoiMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppConfig _config;

    public AccessControlListDoiMiddleware(RequestDelegate next, AppConfig config)
    {
        _next = next;
        _config = config;
    }

    public async Task Invoke(
        HttpContext context,
        IPermissionService permissionService,
        IAccessControlListDoiService aclService)
    {
        if (context.Request.Path.Value == null ||
            _config.AccessControlListEvaluationIgnoredPaths.Contains(context.Request.Path.Value))
        {
            await _next(context);
            return;
        }

        var bfsAcls = await aclService.GetBfsNumberAccessControlListByTenantId(
            permissionService.TenantId,
            DomainOfInfluenceType.Mu);

        permissionService.SetImpersonatedAccessControlPermissions(bfsAcls);

        await _next(context).ConfigureAwait(false);
    }
}
