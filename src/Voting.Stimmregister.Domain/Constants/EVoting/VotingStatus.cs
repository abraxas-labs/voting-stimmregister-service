﻿// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Constants.EVoting;

/// <summary>
/// Enumeration defining the voters e-voting registration status.
/// </summary>
public enum VotingStatus
{
    /// <summary>
    /// Voter e-voting status is unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Voter has registered for e-voting.
    /// </summary>
    Registered = 1,

    /// <summary>
    /// Voter has unregistered for e-voting.
    /// </summary>
    Unregistered = 2,
}
