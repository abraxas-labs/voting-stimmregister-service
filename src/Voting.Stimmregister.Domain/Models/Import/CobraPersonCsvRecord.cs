// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using CsvHelper.Configuration.Attributes;

namespace Voting.Stimmregister.Domain.Models.Import;

/// <summary>
/// CSV import model for person records from COBRA.
/// </summary>
public class CobraPersonCsvRecord : IPersonCsvRecord
{
    /// <summary>
    /// Gets or sets the identifier of the person entry as provided by the source system, i.e. '123456789'.
    /// </summary>
    [Name("eVotingID")]
    public string SourceSystemId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the 'eCH-0044:vnType' AHV number, i.e. '7560000111122' for '756.0000.1111.22'.
    /// A person is uniquely identified by the AHV number (AHVN). The AHVN can exist several times for the same person if several residential status exist (HWS, NWS).
    /// </summary>
    [Name("AHVN13")]
    public long? Vn { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0011:officialName, i.e. 'Fisher'.
    /// </summary>
    [Name("Nachname")]
    public string OfficialName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the eCH-0011:firstName, i.e. 'Jade Jamila'.
    /// </summary>
    [Name("Vorname")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Salutation10, i.e. '1'.
    /// Not required by VOTING Stimmregister.
    /// </summary>
    [Name("Anrede10")]
    public string? Salutation10 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the eCH-0011:birthDataType, i.e. '21.09.1987'.
    /// </summary>
    [Name("Geburtsdatum")]
    public string DateOfBirth { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Heimatgemeinden, i.e. ''.
    /// Not required by VOTING Stimmregister.
    /// </summary>
    [Name("Heimatgemeinden")]
    public string? Heimatgemeinden { get; set; }

    /// <summary>
    /// Gets or sets the eCH0045:person (Extension) contact address line, i.e. 'Apartment 86'.
    /// Part of the addresses for Swiss abroad do not use the common street fields, instead address line 1-7 are filled.
    /// </summary>
    [Name("Adresszeile 1")]
    public string? ContactAddressLine1 { get; set; }

    /// <summary>
    /// Gets or sets the eCH0045:person (Extension) contact address line, i.e. 'Molinaccio'.
    /// refer to: <see cref="ContactAddressLine1"/>.
    /// </summary>
    [Name("Adresszeile 2")]
    public string? ContactAddressLine2 { get; set; }

    /// <summary>
    /// Gets or sets the eCH0045:person (Extension) contact address line, i.e. 'Playa'.
    /// refer to: <see cref="ContactAddressLine1"/>.
    /// </summary>
    [Name("Adresszeile 3")]
    public string? ContactAddressLine3 { get; set; }

    /// <summary>
    /// Gets or sets the eCH0045:person (Extension) contact address line, i.e. 'P.O. Box 4'.
    /// refer to: <see cref="ContactAddressLine1"/>.
    /// </summary>
    [Name("Adresszeile 4")]
    public string? ContactAddressLine4 { get; set; }

    /// <summary>
    /// Gets or sets the eCH0045:person (Extension) contact address line, i.e. '74160 Beaumont'.
    /// refer to: <see cref="ContactAddressLine1"/>.
    /// </summary>
    [Name("Adresszeile 5")]
    public string? ContactAddressLine5 { get; set; }

    /// <summary>
    /// Gets or sets the eCH0045:person (Extension) contact address line, i.e. 'Dorking'.
    /// refer to: <see cref="ContactAddressLine1"/>.
    /// </summary>
    [Name("Adresszeile 6")]
    public string? ContactAddressLine6 { get; set; }

    /// <summary>
    /// Gets or sets the eCH0045:person (Extension) contact address line, i.e. 'AL1 3GW'.
    /// refer to: <see cref="ContactAddressLine1"/>.
    /// </summary>
    [Name("Adresszeile 7")]
    public string? ContactAddressLine7 { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0045:residenceCountry, i.e. 'DE'.
    /// Data Conversion: Convert from long name to standardized ISO 3166-1 (alpha-2) code.
    /// The COBRA field 'Bfs-Ländernummer' can be used to get a better result in the data conversion.
    /// </summary>
    [Name("Land")]
    public string? ResidenceCountry { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0007:swissMunicipalityType, i.e. '9170' for Swiss abroad.
    /// The municipality id (BFS) is used during the import to differentiate individual DOI imports on a municipality base.
    /// Further, it is required for filtering to improve the query performance.
    /// </summary>
    [Name("Nr der politischen Gemeinde")]
    public int MunicipalityId { get; set; }

    /// <summary>
    /// Gets or sets the distribution key, i.e. ''.
    /// Not required by SSR.
    /// </summary>
    [Name("Verteilschlüssel der Ergebnisse")]
    public string? DistributionKey { get; set; }

    /// <summary>
    /// Gets or sets the federal voting right, i.e. '1'.
    /// Not required by SSR.
    /// </summary>
    [Name("Eidgenössisches Stimmrecht")]
    public string? FederalVotingRight { get; set; }

    /// <summary>
    /// Gets or sets the cantonal voting right, i.e. '0'.
    /// Not required by SSR.
    /// </summary>
    [Name("Kantonales Stimmrecht")]
    public string? CantonalVotingRight { get; set; }

    /// <summary>
    /// Gets or sets the communal voting right, i.e. '1'.
    /// Not required by SSR.
    /// </summary>
    [Name("Kommunales Stimmrecht")]
    public string? CommunalVotingRight { get; set; }

    /// <summary>
    /// Gets or sets the entry validity end date, i.e. ''.
    /// Not required by SSR.
    /// </summary>
    [Name("Gültigkeit der Eintragung endet")]
    public string? EntryValidityEndDate { get; set; }

    /// <summary>
    /// Gets or sets the franking code, i.e. '4'.
    /// Not required by SSR.
    /// </summary>
    [Name("Frankierungscode")]
    public string? FrankingCode { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0045:languageOfCorrespondence code, i.e. 'de'.
    /// </summary>
    [Name("Code der Kommunikationssprache")]
    public string? LanguageOfCorrespondence { get; set; }

    /// <summary>
    /// Gets or sets the communication language, i.e. 'Deutsch'.
    /// Not required by SSR.
    /// </summary>
    [Name("Sprache")]
    public string? Language { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0045:residenceCountry, i.e. 'DE'.
    /// Convert from long name to standardized ISO 3166-1 (alpha-2) code.
    /// The COBRA field 'Bfs-Ländernummer' can be used to get a better result in the data conversion.
    /// </summary>
    [Name("Aufenthaltsland")]
    public string? Aufenthaltsland { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0044:sexType, i.e. 'weiblich' for 'female'.
    /// <list type="bullet">
    ///     <item>männlich: male</item>
    ///     <item>weiblich: female</item>
    ///     <item>all others: undefined</item>
    /// </list>
    /// </summary>
    [Name("Geschlecht")]
    public string? Sex { get; set; }

    /// <summary>
    /// Gets or sets the salutation, i.e. 'Ms'.
    /// Not required by VOTING Stimmregister.
    /// </summary>
    [Name("Anrede")]
    public string? Salutation { get; set; }

    /// <summary>
    /// Gets or sets the title, i.e. ''.
    /// Not required by VOTING Stimmregister.
    /// </summary>
    [Name("Titel")]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the salutation used in letters, i.e. 'Madame,'.
    /// Not required by VOTING Stimmregister.
    /// </summary>
    [Name("Briefanrede")]
    public string? LetterSalutation { get; set; }

    /// <summary>
    /// Gets or sets the place of birth, i.e. 'St.Gallen'.
    /// Not required by VOTING Stimmregister.
    /// </summary>
    [Name("Geburtsort")]
    public string? PlaceOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:streetType, i.e. 'St. Leonhardstrasse'.
    /// </summary>
    [Name("Strasse")]
    public string? ContactAddressStreet { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:houseNumberType, i.e. '5'.
    /// </summary>
    [Name("Hausnummer")]
    public string? ContactAddressHouseNumber { get; set; }

    /// <summary>
    /// Gets or sets the house number addition, i.e. 'b'.
    /// </summary>
    [Name("Hausnummer Zusatz")]
    public string? ContactAddressHouseNumberAddition { get; set; }

    /// <summary>
    /// Gets or sets the eCH0010:postOfficeBoxTextType, i.e. 'SA-19'.
    /// </summary>
    [Name("Postfachnr")]
    public string? ContactAddressPostOfficeBoxText { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:swissZipCodeType, i.e. '9000'.
    /// </summary>
    [Name("PLZ")]
    public string? ContactAddressZipCode { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:townType, i.e. 'Hermanus'.
    /// </summary>
    [Name("Ort")]
    public string? ContactAddressTown { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:locality, i.e. 'Western Cape'.
    /// </summary>
    [Name("Ortsbereich")]
    public string? ContactAddressLocality { get; set; }

    /// <summary>
    /// Gets or sets the voting municipality, i.e. '9170'.
    /// </summary>
    [Name("Stimmgemeinde")]
    public string? VotingMunicipality { get; set; }

    /// <summary>
    /// Gets or sets the bfs country number which can be used for more specific country data converion, i.e. 'PU'.
    /// </summary>
    [Name("BfS-Ländernummer")]
    public string? BfsCountry { get; set; }

    /// <summary>
    /// Gets or sets the bfs number or municipality id, i.e. '9170' for Swiss abroad.
    /// </summary>
    [Name("BfS-Gemeindenummer")]
    public string? BfsNumber { get; set; }

    /// <summary>
    /// Gets or sets the mode of shipment, i.e. '4'.
    /// </summary>
    [Name("Versandart")]
    public string? ModeOfShipment { get; set; }

    /// <summary>
    /// Gets or sets the native place according to eCH-0011:originName, i.e. 'Lauterbrunnen'.
    /// </summary>
    [Name("Heimatort 1")]
    public string? OriginName1 { get; set; }

    /// <summary>
    /// Gets or sets the canton of the native place, i.e. 'SG'.
    /// </summary>
    [Name("Heimatkanton 1")]
    public string? OnCanton1 { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0020:comesFrom, i.e. 'St. Gallen'.
    /// </summary>
    [Name("Letzte Wohnsitzgemeinde")]
    public string? MoveInComesFrom { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the person is registered for e-voting or not.
    /// Specific eVoting flag for Swiss abroad. Not to be confused with the VOTING Stimmregister eServices eVoting Flag.
    /// </summary>
    [Name("E-Voting")]
    [BooleanTrueValues("1")]
    [BooleanFalseValues("0")]
    public bool? SwissAbroadEvotingFlag { get; set; }

    /// <summary>
    /// Gets a record identifier for troubleshooting if a record couldn't be imported due to record validation errors.
    /// </summary>
    /// <returns>A record identifier literal.</returns>
    public string BuildRecordIdentifier()
        => $"{FirstName} {OfficialName} ({DateOfBirth}, {SourceSystemId})";
}
