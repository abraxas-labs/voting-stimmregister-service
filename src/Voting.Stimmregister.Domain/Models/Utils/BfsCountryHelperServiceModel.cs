// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Xml.Serialization;

namespace Voting.Stimmregister.Domain.Models.Utils;

/// <summary>
/// The BFS country helper service file model.
/// </summary>
[Serializable]
public class BfsCountryHelperServiceModel
{
    /// <summary>
    /// Gets or sets the id (BFS Number) i.e. '8100'.
    /// </summary>
    [XmlElement("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the inId i.e. '756'.
    /// </summary>
    [XmlElement("unId")]
    public int UnId { get; set; }

    /// <summary>
    /// Gets or sets the iso2Id i.e. 'CH'.
    /// </summary>
    [XmlElement("iso2Id")]
    public string Iso2Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the iso3Id i.e. 'CHE'.
    /// </summary>
    [XmlElement("iso3Id")]
    public string Iso3Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the shortNameDe i.e. 'Schweiz'.
    /// </summary>
    [XmlElement("shortNameDe")]
    public string ShortNameDe { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the shortNameEn i.e. 'Switzerland'.
    /// </summary>
    [XmlElement("shortNameEn")]
    public string ShortNameEn { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the officialNameDe i.e. 'Schweizerische Eidgenossenschaft'.
    /// </summary>
    [XmlElement("officialNameDe")]
    public string OfficialNameDe { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets the entryValid i.e. 'treu'.
    /// </summary>
    [XmlElement("entryValid")]
    public bool EntryValid { get; set; }
}
