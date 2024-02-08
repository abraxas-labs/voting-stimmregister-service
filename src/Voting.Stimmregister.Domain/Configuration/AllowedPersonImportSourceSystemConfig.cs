// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Domain.Configuration;

/// <summary>
/// Configuration item for the allowed person import source system.
/// </summary>
public class AllowedPersonImportSourceSystemConfig
{
    /// <summary>
    /// Gets or sets the municipality id.
    /// </summary>
    public int MunicipalityId { get; set; }

    /// <summary>
    /// Gets or sets the import source system.
    /// </summary>
    public ImportSourceSystem ImportSourceSystem { get; set; }

    /// <summary>
    /// Gets or sets the starting date from where on the import source system is allowed for the municipality id.
    /// </summary>
    public DateTime StartingDate { get; set; }
}
