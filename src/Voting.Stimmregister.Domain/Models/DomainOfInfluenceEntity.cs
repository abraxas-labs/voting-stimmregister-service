// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Domain.Models;

/// <summary>
/// Domain of influence database model.
/// </summary>
public class DomainOfInfluenceEntity : ImportedEntity
{
    /// <summary>
    /// Gets or sets the domain of influence id, i.e. '906'.
    /// This field is used to uniquely identity the data entry for a particular street and its associated circles.
    /// The identification is based on the subsystem's primary key.
    /// It is primarily used to find the associated circles for one or more persons and vice versa.
    /// </summary>
    public int DomainOfInfluenceId { get; set; }

    /// <summary>
    /// Gets or sets the street, i.e. 'Kirchlistr.'.
    /// </summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the house number, i.e. '25'.
    /// </summary>
    public string HouseNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the house number addition, i.e. 'a'.
    /// </summary>
    public string? HouseNumberAddition { get; set; }

    /// <summary>
    /// Gets or sets the zip code, i.e. '9000'.
    /// </summary>
    public int SwissZipCode { get; set; }

    /// <summary>
    /// Gets or sets the town, i.e. 'St. Gallen'.
    /// </summary>
    public string? Town { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the domain of influence is part of a political municipality, i.e. 'true'. Default is 'false'.
    /// </summary>
    public bool? IsPartOfPoliticalMunicipality { get; set; }

    /// <summary>
    /// Gets or sets the political circle id, i.e. 'W' for 'Westen'.
    /// </summary>
    public string? PoliticalCircleId { get; set; }

    /// <summary>
    /// Gets or sets the political circle name, i.e. 'Westen'.
    /// </summary>
    public string? PoliticalCircleName { get; set; }

    /// <summary>
    /// Gets or sets the catholic church circle id, i.e. '207'.
    /// </summary>
    public string? CatholicChurchCircleId { get; set; }

    /// <summary>
    /// Gets or sets the catholic church circle name, i.e. 'Katholischer Kirchkreis Heiligkreuz'.
    /// </summary>
    public string? CatholicChurchCircleName { get; set; }

    /// <summary>
    /// Gets or sets the evangelic church circle id, i.e. '221'.
    /// </summary>
    public string? EvangelicChurchCircleId { get; set; }

    /// <summary>
    /// Gets or sets the evangelic church circle name, i.e. 'Evang. Kirchkreis Rotmonten'.
    /// </summary>
    public string? EvangelicChurchCircleName { get; set; }

    /// <summary>
    /// Gets or sets the school circle id, i.e. '705'.
    /// </summary>
    public string? SchoolCircleId { get; set; }

    /// <summary>
    /// Gets or sets the school circle name, i.e. 'Schulzählkreis Rotmonten-Gerhalde'.
    /// </summary>
    public string? SchoolCircleName { get; set; }

    /// <summary>
    /// Gets or sets the traffic circle id, i.e. '119'.
    /// </summary>
    public string? TrafficCircleId { get; set; }

    /// <summary>
    /// Gets or sets the traffic circle name, i.e. '119'.
    /// </summary>
    public string? TrafficCircleName { get; set; }

    /// <summary>
    /// Gets or sets the residential district circle id, i.e. '306'.
    /// </summary>
    public string? ResidentialDistrictCircleId { get; set; }

    /// <summary>
    /// Gets or sets the residential district circle name, i.e. 'Notkersegg (Hub) Osten'.
    /// </summary>
    public string? ResidentialDistrictCircleName { get; set; }

    /// <summary>
    /// Gets or sets the people council circle id, i.e. '15'.
    /// </summary>
    public string? PeopleCouncilCircleId { get; set; }

    /// <summary>
    /// Gets or sets the people council circle name, i.e. 'Quartierverein Nordost-Heiligkreuz'.
    /// </summary>
    public string? PeopleCouncilCircleName { get; set; }

    // References

    /// <summary>
    /// Gets or sets the import id reference.
    /// </summary>
    public Guid? ImportStatisticId { get; set; }

    /// <summary>
    /// Gets or sets the import entity reference.
    /// </summary>
    public ImportStatisticEntity? ImportStatistic { get; set; }
}
