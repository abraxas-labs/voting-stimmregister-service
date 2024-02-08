// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using CsvHelper.Configuration.Attributes;

namespace Voting.Stimmregister.Domain.Models.Import;

/// <summary>
/// CSV import model for domain of influence records.
/// </summary>
public class LogantoDomainOfInfluenceCsvRecord : IMunicipalityCsvRecord
{
    /// <summary>
    /// Gets or sets the municipality id (BFS number), i.e. '3214'.
    /// The municipality id from Bundesamt for Statistics is used during the import to differentiate individual DOI imports on a municipality base.
    /// </summary>
    [Name("KUNDENNUMMER")]
    public int MunicipalityId { get; set; }

    /// <summary>
    /// Gets or sets the domain of influence id, i.e. '906'.
    /// This field is used to uniquely identity the data entry for a particular street and its associated circles.
    /// The identification is based on the subsystem's primary key.
    /// It is primarily used to find the associated circles for one or more persons and vice versa.
    /// </summary>
    [Name("ADABAS_ISN")]
    public int DomainOfInfluenceId { get; set; }

    /// <summary>
    /// Gets or sets the street, i.e. 'Kirchlistr.'.
    /// </summary>
    [Name("STRASSENBEZEICHNUNG")]
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the house number, i.e. '25'.
    /// </summary>
    [Name("HAUSNUMMER")]
    public string HouseNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the house number addition, i.e. 'a'.
    /// </summary>
    [Name("HAUSNUMMER_ZUSATZ")]
    public string? HouseNumberAddition { get; set; }

    /// <summary>
    /// Gets or sets the zip code, i.e. '9000'.
    /// </summary>
    [Name("O-PLZ")]
    public int SwissZipCode { get; set; }

    /// <summary>
    /// Gets or sets the town, i.e. 'St. Gallen'.
    /// </summary>
    [Name("O-ORT")]
    public string? Town { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the domain of influence is part of a political municipality, i.e. 'true'. Default is 'false'.
    /// </summary>
    [BooleanTrueValues("1")]
    [BooleanFalseValues("0")]
    [Name("IN_POLITISCHER_GEMEINDE")]
    public bool? IsPartOfPoliticalMunicipality { get; set; }

    /// <summary>
    /// Gets or sets the political circle id, i.e. 'W' for 'Westen'.
    /// </summary>
    [Name("POLITISCHER_KREIS")]
    public string? PoliticalCircleId { get; set; }

    /// <summary>
    /// Gets or sets the political circle name, i.e. 'Westen'.
    /// </summary>
    [Name("POLITISCHER_KREIS_TXT")]
    public string? PoliticalCircleName { get; set; }

    /// <summary>
    /// Gets or sets the catholic church circle id, i.e. '207'.
    /// </summary>
    [Name("KATHOLISCHER_KIRCHKREIS")]
    public string? CatholicChurchCircleId { get; set; }

    /// <summary>
    /// Gets or sets the catholic church circle name, i.e. 'Katholischer Kirchkreis Heiligkreuz'.
    /// </summary>
    [Name("KATHOLISCHER_KIRCHKREIS_TXT")]
    public string? CatholicChurchCircleName { get; set; }

    /// <summary>
    /// Gets or sets the evangelic church circle id, i.e. '221'.
    /// </summary>
    [Name("EVANGELISCHER_KIRCHKREIS")]
    public string? EvangelicChurchCircleId { get; set; }

    /// <summary>
    /// Gets or sets the evangelic church circle name, i.e. 'Evang. Kirchkreis Rotmonten'.
    /// </summary>
    [Name("EVANGELISCHER_KIRCHKREIS_TXT")]
    public string? EvangelicChurchCircleName { get; set; }

    /// <summary>
    /// Gets or sets the school circle id, i.e. '705'.
    /// </summary>
    [Name("SCHULKREIS")]
    public string? SchoolCircleId { get; set; }

    /// <summary>
    /// Gets or sets the school circle name, i.e. 'Schulzählkreis Rotmonten-Gerhalde'.
    /// </summary>
    [Name("SCHULKREIS_TXT")]
    public string? SchoolCircleName { get; set; }

    /// <summary>
    /// Gets or sets the traffic circle id, i.e. '119'.
    /// </summary>
    [Name("VERKEHRSKREIS")]
    public string? TrafficCircleId { get; set; }

    /// <summary>
    /// Gets or sets the traffic circle name, i.e. '119'.
    /// </summary>
    [Name("VERKEHRSKREIS_TXT")]
    public string? TrafficCircleName { get; set; }

    /// <summary>
    /// Gets or sets the residential district circle id, i.e. '306'.
    /// </summary>
    [Name("WOHNVIERTEL")]
    public string? ResidentialDistrictCircleId { get; set; }

    /// <summary>
    /// Gets or sets the residential district circle name, i.e. 'Notkersegg (Hub) Osten'.
    /// </summary>
    [Name("WOHNVIERTEL_TXT")]
    public string? ResidentialDistrictCircleName { get; set; }

    /// <summary>
    /// Gets or sets the people council circle id, i.e. '15'.
    /// </summary>
    [Name("VOLKSZAEHLKREIS")]
    public string? PeopleCouncilCircleId { get; set; }

    /// <summary>
    /// Gets or sets the people council circle name, i.e. 'Quartierverein Nordost-Heiligkreuz'.
    /// </summary>
    [Name("VOLKSZAEHLKREIS_TXT")]
    public string? PeopleCouncilCircleName { get; set; }

    public string BuildRecordIdentifier()
        => $"{DomainOfInfluenceId} {SwissZipCode} {Town}";
}
