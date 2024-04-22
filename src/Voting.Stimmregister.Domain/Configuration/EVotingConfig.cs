// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using Voting.Lib.Iam.ServiceTokenHandling;

namespace Voting.Stimmregister.Domain.Configuration;

/// <summary>
/// Configuration model for E-Voting.
/// </summary>
public class EVotingConfig
{
    public const string SecureConnectSharedEVotingOptionKey = "SecureConnectSharedEVoting";

    public string KewrServiceUrl { get; set; } = string.Empty;

    public string LogantoServiceUrl { get; set; } = string.Empty;

    public SecureConnectServiceAccountOptions SecureConnectSharedEVoting { get; set; } = new();

    public Dictionary<string, int> LogantoOeidToOrgUnitMapping { get; set; } = new();

    public bool EnableKewrAndLoganto { get; set; } = true;

    /// <summary>
    /// Gets or sets a list of municipality ids to skip forwarding of the e-voter flag to loganto.
    /// </summary>
    public HashSet<short> SkipForwardingEVoterFlag { get; set; } = new();
}
