// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Models.Utils;

/// <summary>
/// The country model.
/// </summary>
public class CountryModel
{
    /// <summary>
    /// Gets or sets the country identification e.g. BFS Number '8100'.
    /// </summary>
    public ushort BfsId { get; set; }

    /// <summary>
    /// Gets or sets the id as defiend by the source system e.g. Loganto Id 'S'.
    /// </summary>
    public string? SystemId { get; set; }

    /// <summary>
    /// Gets or sets the iso2.
    /// </summary>
    public string? Iso2 { get; set; }

    /// <summary>
    /// Gets or sets the shortNameDe e.g. 'Schweiz'.
    /// </summary>
    public string? ShortNameDe { get; set; }

    /// <summary>
    /// Gets or sets the shortNameEn e.g. 'Switzerland'.
    /// </summary>
    public string? ShortNameEn { get; set; }
}
