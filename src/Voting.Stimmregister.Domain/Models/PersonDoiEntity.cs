// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Lib.Database.Models;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Domain.Models;

/// <summary>
/// Person DOI database model.
/// </summary>
public class PersonDoiEntity : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the person DOI, i.e. 'Westen'.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the person DOI, i.e. 'W'.
    /// </summary>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the canton abbreviation of the person DOI, i.e. 'SG'.
    /// </summary>
    public string Canton { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the DOI type of the person DOI.
    /// </summary>
    public DomainOfInfluenceType DomainOfInfluenceType { get; set; }

    // References
    public Guid PersonId { get; set; }

    public PersonEntity? Person { get; set; }
}
