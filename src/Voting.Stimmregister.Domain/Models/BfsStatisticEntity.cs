// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Models;

public class BfsStatisticEntity : AuditedEntity
{
    /// <summary>
    /// Gets or sets the bfs / municipality id.
    /// </summary>
    public string Bfs { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bfs / municipality name.
    /// </summary>
    public string BfsName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total amount of voters.
    /// </summary>
    public int VoterTotalCount { get; set; }

    /// <summary>
    /// Gets or sets the total amount of e-voters.
    /// </summary>
    public int EVoterTotalCount { get; set; }

    /// <summary>
    /// Gets or sets the canton bfs number, i.e. '17'.
    /// </summary>
    public short CantonBfs { get; set; }
}
