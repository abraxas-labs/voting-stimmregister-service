// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Lib.Iam.ServiceTokenHandling;

namespace Voting.Stimmregister.Domain.Configuration;

/// <summary>
/// Configuration model for VOTING Basis settings mapped from appsettings.
/// </summary>
public class VotingBasisConfig
{
    /// <summary>
    /// Gets or sets the cron schedule expression for DOI-ACL synchronization.
    /// </summary>
    public string CronScheduleDoiAclSync { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the api endpoint for VOTING Basis.
    /// </summary>
    public Uri? ApiEndpoint { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to enable or disable GRPC web.
    /// </summary>
    public bool EnableGrpcWeb { get; set; }

    /// <summary>
    /// Gets or sets the tenant id of the api admin.
    /// </summary>
    public string ApiAdminTenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the application id of VOTING Basis.
    /// </summary>
    public string ApiAdminAppId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identity provider configuration.
    /// </summary>
    public SecureConnectServiceAccountOptions IdpServiceAccount { get; set; } = new();
}
