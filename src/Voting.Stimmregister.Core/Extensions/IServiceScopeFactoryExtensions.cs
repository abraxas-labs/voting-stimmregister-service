// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;

namespace Voting.Stimmregister.Core.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceScopeFactoryExtensions"/>.
/// </summary>
public static class IServiceScopeFactoryExtensions
{
    /// <summary>
    /// Creates a new impersonated service scope that can be individually configured to impersonate
    /// a specific identity and access control level.
    /// </summary>
    /// <param name="factory">The service scope factory.</param>
    /// <param name="user">The user information.</param>
    /// <param name="tenant">The tenant infomration.</param>
    /// <param name="roles">The roles.</param>
    /// <param name="doiAcl">The access control list.</param>
    /// <returns>A service scope which is configured to use the ImpersonationSecurityContext.</returns>
    public static IServiceScope CreateImpersonationScope(
        this IServiceScopeFactory factory,
        Lib.Iam.Models.User user,
        Lib.Iam.Models.Tenant tenant,
        IReadOnlyCollection<string>? roles = null,
        IReadOnlyCollection<string>? doiAcl = null)
    {
        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        var scope = factory.CreateScope();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

        if (user != null && tenant != null)
        {
            permissionService.SetImpersonatedIdentityPermissions(user, tenant, roles);
        }

        if (doiAcl != null)
        {
            permissionService.SetImpersonatedAccessControlPermissions(doiAcl);
        }

        return scope;
    }
}
