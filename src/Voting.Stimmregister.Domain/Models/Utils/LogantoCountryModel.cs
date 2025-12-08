// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Xml.Serialization;

namespace Voting.Stimmregister.Domain.Models.Utils;

/// <summary>
/// The loganto country helper service file model.
/// </summary>
[Serializable]
public class LogantoCountryModel
{
    /// <summary>
    /// Gets or sets the id (BFS Number) i.e. '8207'.
    /// </summary>
    [XmlElement("id")]
    public string IdString { get; set; } = string.Empty;

    public ushort? Id => !string.IsNullOrWhiteSpace(IdString) && ushort.TryParse(IdString, out var retval) ? retval : null;

    /// <summary>
    /// Gets or sets the logaId i.e. 'D'.
    /// </summary>
    [XmlElement("logaId")]
    public string LogaId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the iso2Id i.e. 'DE'.
    /// </summary>
    [XmlElement("iso2Id")]
    public string? Iso2Id { get; set; }

    /// <summary>
    /// Gets or sets the iso3Id i.e. 'DEU'.
    /// </summary>
    [XmlElement("iso3Id")]
    public string? Iso3Id { get; set; }

    /// <summary>
    /// Gets or sets the shortNameDe i.e. 'Deutschland'.
    /// </summary>
    [XmlElement("shortNameDe")]
    public string ShortNameDe { get; set; } = string.Empty;
}
