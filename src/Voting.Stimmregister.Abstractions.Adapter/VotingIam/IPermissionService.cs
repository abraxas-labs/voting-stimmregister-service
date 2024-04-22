// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using Voting.Lib.Iam.Models;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Adapter.VotingIam;

/// <summary>
/// Permission service to check users permissions and roles.
/// </summary>
public interface IPermissionService
{
    string UserId { get; }

    string TenantId { get; }

    string TenantName { get; }

    IReadOnlyCollection<string> BfsAccessControlList { get; }

    /// <summary>
    /// Checks whether the current user has import observer role or not.
    /// </summary>
    /// <returns>True if user has import observer role.</returns>
    bool IsImportObserver();

    /// <summary>
    /// Checks whether the current user has manual exporter role or not.
    /// </summary>
    /// <returns>True if user has manual exporter role.</returns>
    bool IsManualExporter();

    /// <summary>
    /// Overrides impersonated identity information for the user and tenant.
    /// </summary>
    /// <param name="user">The user information.</param>
    /// <param name="tenant">The tenant information.</param>
    /// <param name="roles">The authentication roles.</param>
    void SetImpersonatedIdentityPermissions(
        User user,
        Tenant tenant,
        IEnumerable<string>? roles);

    /// <summary>
    /// Overrides impersonated access control level infomration for an identity.
    /// </summary>
    /// <param name="doiAcl">The access control list.</param>
    void SetImpersonatedAccessControlPermissions(
        IReadOnlyCollection<string> doiAcl);

    /// <summary>
    /// Returns whether a user is an authenticated service user.
    /// </summary>
    /// <returns><c>true</c> if a user is authenticated and is a service user.</returns>
    bool IsServiceUser();

    /// <summary>
    /// Sets the configured Abraxas service user authentication if no authentication is currently provided.
    /// This should only be used for background jobs or similar things.
    /// </summary>
    void SetAbraxasAuthIfNotAuthenticated();

    /// <summary>
    /// Marks an entity as created as of now by the current user.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void SetCreated(IAuditedEntity entity);

    /// <summary>
    /// Marks an entity as modified as of now by the current user.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void SetModified(IAuditedEntity entity);

    /// <summary>
    /// Clears the modified audit info.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void ClearModified(IAuditedEntity entity);
}
