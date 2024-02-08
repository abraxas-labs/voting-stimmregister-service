// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Models.Utils;

/// <summary>
/// The country helper service search result model.
/// </summary>
public class CountryHelperServiceResultModel
{
    /// <summary>
    /// Gets or sets the iso2Id code results.
    /// </summary>
    public string? Iso2Id { get; set; }

    /// <summary>
    /// Gets or sets the shortNameDe results.
    /// </summary>
    public string ShortNameDe { get; set; } = string.Empty;
}
