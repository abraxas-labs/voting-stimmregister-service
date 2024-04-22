// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Voting.Lib.Common;
using Voting.Lib.Iam.Models;
using Voting.Lib.Iam.Store;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Adapter.VotingIam.Configuration;
using Voting.Stimmregister.Domain.Authorization;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.VotingIam.Services;

/// <inheritdoc/>
public class PermissionService : IPermissionService
{
    private readonly IAuth _auth;
    private readonly IAuthStore _authStore;
    private readonly ILogger<PermissionService> _logger;
    private readonly VotingIamConfig _config;
    private readonly IClock _clock;

    public PermissionService(
        IAuth auth,
        IAuthStore authStore,
        ILogger<PermissionService> logger,
        VotingIamConfig config,
        IClock clock)
    {
        _auth = auth;
        _authStore = authStore;
        _logger = logger;
        _config = config;
        _clock = clock;
    }

    public string UserId => _auth.User.Loginid;

    public string TenantId => _auth.Tenant.Id;

    public string TenantName => _auth.Tenant.Name;

    public IReadOnlyCollection<string> BfsAccessControlList { get; private set; } = Array.Empty<string>();

    /// <inheritdoc/>
    public bool IsImportObserver()
        => _auth.HasRole(Roles.ImportObserver);

    public bool IsManualExporter()
        => _auth.HasRole(Roles.ManualExporter);

    public void SetImpersonatedIdentityPermissions(
        User user,
        Tenant tenant,
        IEnumerable<string>? roles)
    {
        _authStore.SetValues(string.Empty, user, tenant, roles);
    }

    /// <inheritdoc/>
    public void SetImpersonatedAccessControlPermissions(
        IReadOnlyCollection<string> doiAcl)
    {
        BfsAccessControlList = doiAcl;
    }

    public bool IsServiceUser()
        => _auth is { IsAuthenticated: true, User.Servicename: not null };

    public void SetAbraxasAuthIfNotAuthenticated()
    {
        if (!_auth.IsAuthenticated)
        {
            _logger.LogDebug(SecurityLogging.SecurityEventId, "Using Abraxas authentication values, since no user is authenticated");
            _authStore.SetValues(
                string.Empty,
                new User
                {
                    Loginid = _config.ServiceUserId,
                    Servicename = _config.ServiceAccount,
                },
                new Tenant
                {
                    Id = _config.AbraxasTenantId,
                },
                Enumerable.Empty<string>());
        }
    }

    public void SetCreated(IAuditedEntity entity)
    {
        entity.AuditInfo.CreatedAt = _clock.UtcNow;
        entity.AuditInfo.CreatedById = _auth.User.Loginid;
        entity.AuditInfo.CreatedByName = string.IsNullOrWhiteSpace(_auth.User.Username) ? _auth.User.Servicename ?? "unknown" : _auth.User.Username;
    }

    public void SetModified(IAuditedEntity entity)
    {
        entity.AuditInfo.ModifiedAt = _clock.UtcNow;
        entity.AuditInfo.ModifiedById = _auth.User.Loginid;
        entity.AuditInfo.ModifiedByName = string.IsNullOrWhiteSpace(_auth.User.Username) ? _auth.User.Servicename ?? "unknown" : _auth.User.Username;
    }

    public void ClearModified(IAuditedEntity entity)
    {
        entity.AuditInfo.ModifiedAt = null;
        entity.AuditInfo.ModifiedById = null;
        entity.AuditInfo.ModifiedByName = null;
    }
}
