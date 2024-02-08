// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;

namespace Voting.Stimmregister.Domain.Configuration;

public class LogantoPersonImportConfig
{
    /// <summary>
    /// Gets or sets a set of bfs numbers for which the voting cards are still sent to the voter
    /// for people with away addresses (no domain of influence id).
    /// See rule 4 of the specification "nicht zustellen".
    /// </summary>
    public HashSet<int> BfsThatAllowSendingVotingCardForPeopleWithAwayAddresses { get; set; } = new();

    /// <summary>
    /// Gets or sets a set of bfs numbers for which the voting cards are still sent to the voter
    /// for people with unknown main residence address but an existing residence address (with domain of influence id).
    /// See rule 3 of the specification "nicht zustellen".
    /// </summary>
    public HashSet<int> BfsThatAllowSendingVotingCardForPeopleWithUnknownMainResidenceAddresses { get; set; } = new();
}
