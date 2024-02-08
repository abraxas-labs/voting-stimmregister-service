// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Core.Configuration;

public class FilterConfig
{
    /// <summary>
    /// Gets or sets the suffix which is appended to filter duplicates.
    /// </summary>
    public string DuplicateNameSuffix { get; set; } = " (Kopie)";
}
