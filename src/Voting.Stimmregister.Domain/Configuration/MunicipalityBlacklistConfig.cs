// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Domain.Configuration;

/// <summary>
/// Configuration item for the municipality blacklist.
/// </summary>
public class MunicipalityBlacklistConfig
{
    /// <summary>
    /// Gets or sets the municipality id.
    /// </summary>
    public int MunicipalityId { get; set; }

    /// <summary>
    /// Gets or sets the starting date from where on the municipality id is blacklisted.
    /// </summary>
    public DateTime StartingDate { get; set; }
}
