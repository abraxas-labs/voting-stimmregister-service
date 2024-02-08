// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Adapter.EVoting.Kewr.Models;

public class GenderModel
{
    /// <summary>
    /// Gets or sets the gender code eCH, for example "1" for male, "2" for female and "3" for unknown.
    /// </summary>
    public string CodeECH { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the gender code ID, for example "m" for male and "f" for female.
    /// </summary>
    public string CodeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the gender as localized text, for example "männlich".
    /// </summary>
    public string CodeText { get; set; } = string.Empty;
}
