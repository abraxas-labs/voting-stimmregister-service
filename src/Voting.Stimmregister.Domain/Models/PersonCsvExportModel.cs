// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using CsvHelper.Configuration.Attributes;

namespace Voting.Stimmregister.Domain.Models;

/// <summary>
/// The sanitized person domain model.
/// </summary>
public class PersonCsvExportModel
{
    /// <summary>
    /// Gets or sets the AHV number, i.e. '7560000111122' for '756.0000.1111.22'.
    /// </summary>
    [Name("AHV-Versichertennummer")]
    public string? Vn { get; set; }

    /// <summary>
    /// Gets or sets the official name, i.e. 'Muster'.
    /// </summary>
    [Name("Amtlicher Name")]
    public string OfficialName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the first name, i.e. 'Max'.
    /// </summary>
    [Name("Vorname")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sex type, i.e. '2'.
    /// <list type="bullet">
    ///     <item>1: male</item>
    ///     <item>2: female</item>
    ///     <item>3: undefined</item>
    /// </list>
    /// </summary>
    [Name("Geschlecht-Code")]
    public int SexCode { get; set; }

    /// <summary>
    /// Gets or sets the sex type, i.e. '2' for 'female'.
    /// <list type="bullet">
    ///     <item>1: male</item>
    ///     <item>2: female</item>
    ///     <item>3: undefined</item>
    /// </list>
    /// </summary>
    [Name("Geschlecht")]
    public string? Sex { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '15.05.1988'.
    /// </summary>
    [Name("Geburtsdatum")]
    [Format("dd.MM.yyyy")]
    public DateOnly? DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets the , i.e. ''.
    /// </summary>
    [Name("Geburtsdatum (angepasst)")]
    public string? DateOfBirthAdjusted { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. ' Ja' oder 'Nein'.
    /// </summary>
    [Name("Schweizer Staatsbürgerschaft")]
    public string SwissCitizenship { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Lediger Name")]
    public string OriginalName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Allianz-Name / Partnerschaftsname")]
    public string AlianceName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Aliasname")]
    public string AliasName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Anderer Name")]
    public string OtherName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Rufname")]
    public string CallName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Nationalität")]
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Zustelladresse - Adresszusatzzeile 1")]
    public string ContactAddressAddressLineExtension1 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Zustelladresse - Adresszusatzzeile 2")]
    public string ContactAddressAddressLineExtension2 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Zustelladresse - Strassenbezeichnung")]
    public string ContactAddressStreet { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Zustelladresse - Hausnummer")]
    public string ContactAddressHouseNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Zustelladresse - Wohnungsnummer")]
    public string ContactAddressDwellingNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Zustelladresse - Postfachbezeichnung")]
    public string ContactAddressPostOfficeBoxText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Zustelladresse - Postfachnummer")]
    public int? ContactAddressPostOfficeBoxNumber { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Zustelladresse - Ort")]
    public string ContactAddressTown { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Zustelladresse - Postleitzahl")]
    public string ContactAddressZipCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Zustelladresse - Gebiet")]
    public string ContactAddressLocality { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Zustelladresse - Adresszeile 1")]
    public string ContactAddressLine1 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Zustelladresse - Adresszeile 2")]
    public string ContactAddressLine2 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Zustelladresse - Adresszeile 3")]
    public string ContactAddressLine3 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Zustelladresse - Adresszeile 4")]
    public string ContactAddressLine4 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Zustelladresse - Adresszeile 5")]
    public string ContactAddressLine5 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Zustelladresse - Adresszeile 6")]
    public string ContactAddressLine6 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Zustelladresse - Adresszeile 7")]
    public string ContactAddressLine7 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Korrespondenzsprache")]
    public string LanguageOfCorrespondence { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the religion type, i.e. '111'.
    /// <list type="bullet">
    ///     <item>000: undefined</item>
    ///     <item>111: Evangelisch</item>
    ///     <item>121: Katholisch</item>
    ///     <item>122: Christkatholisch</item>
    ///     <item>211: Jüdisch</item>
    /// </list>
    /// </summary>
    [Name("Religion bzw. Konfession")]
    public int ReligionCode { get; set; }

    /// <summary>
    /// Gets or sets the religion type, i.e. '111' for 'Evangelisch'.
    /// <list type="bullet">
    ///     <item>000: undefined</item>
    ///     <item>111: Evangelisch</item>
    ///     <item>121: Katholisch</item>
    ///     <item>122: Christkatholisch</item>
    ///     <item>211: Jüdisch</item>
    /// </list>
    /// </summary>
    [Name("Religion bzw. Konfession")]
    public string? Religion { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. 'St. Gallen'.
    /// </summary>
    [Name("Heimatort 1")]
    public string OriginName1 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'Arbon'.
    /// </summary>
    [Name("Heimatort 2")]
    public string OriginName2 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'St. Gallen'.
    /// </summary>
    [Name("Heimatort 3")]
    public string OriginName3 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'St. Gallen'.
    /// </summary>
    [Name("Heimatort 4")]
    public string OriginName4 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'St. Gallen'.
    /// </summary>
    [Name("Heimatort 5")]
    public string OriginName5 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'St. Gallen'.
    /// </summary>
    [Name("Heimatort 6")]
    public string OriginName6 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'St. Gallen'.
    /// </summary>
    [Name("Heimatort 7")]
    public string OriginName7 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'SG'.
    /// </summary>
    [Name("Heimatortkanton 1")]
    public string OriginCanton1 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'TG'.
    /// </summary>
    [Name("Heimatortkanton 2")]
    public string OriginCanton2 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'SG'.
    /// </summary>
    [Name("Heimatortkanton 3")]
    public string OriginCanton3 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'SG'.
    /// </summary>
    [Name("Heimatortkanton 4")]
    public string OriginCanton4 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'SG'.
    /// </summary>
    [Name("Heimatortkanton 5")]
    public string OriginCanton5 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'SG'.
    /// </summary>
    [Name("Heimatortkanton 6")]
    public string OriginCanton6 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'SG'.
    /// </summary>
    [Name("Heimatortkanton 7")]
    public string OriginCanton7 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. '0201'.
    /// </summary>
    [Name("Ausländerkategorie-Code")]
    public string ResidencePermitCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'Aufenthalterin / Aufenthalter nach EU/EFTA-Abkommen'.
    /// </summary>
    [Name("Ausländerkategorie")]
    public string ResidencePermit { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. '01.01.2022'.
    /// </summary>
    [Name("Ausländerkategorie gültig ab")]
    [Format("dd.MM.yyyy")]
    public DateOnly? ResidencePermitValidFrom { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '01.01.2022'.
    /// </summary>
    [Name("Ausländerkategorie gültig bis")]
    [Format("dd.MM.yyyy")]
    public DateOnly? ResidencePermitValidTill { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '01.01.2022'.
    /// </summary>
    [Name("Einreisedatum")]
    [Format("dd.MM.yyyy")]
    public DateOnly? EntryDate { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. 'St. Gallen'.
    /// </summary>
    [Name("Amtlicher Gemeindename")]
    public string MunicipalityName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. '9001'.
    /// </summary>
    [Name("BFS-Gemeindenummer")]
    public int MunicipalityId { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '01.01.2022'.
    /// </summary>
    [Name("Zuzugsdatum")]
    [Format("dd.MM.yyyy")]
    public DateOnly? MoveInArrivalDate { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. 'St. Gallen'.
    /// </summary>
    [Name("Zuzugsort - Amtlicher Gemeindename")]
    public string MoveInMunicipalityName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'SG'.
    /// </summary>
    [Name("Zuzugsort - Kantonskürzel")]
    public string MoveInCantonAbbreviation { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'Bad Sachsen'.
    /// </summary>
    [Name("Zuzugsort - Herkunftsort")]
    public string MoveInComesFrom { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'Deutschland'.
    /// </summary>
    [Name("Zuzugsort - Ländername Kurzform")]
    public string MoveInCountryNameShort { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets the , i.e. ''.
    /// </summary>
    [Name("Zuzugsort - Unbekannt")]
    public string? MoveInUnknown { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. 'c/o La Vita Seniorenzentrum'.
    /// </summary>
    [Name("Wohnadresse - Adresszusatzzeile 1")]
    public string ResidenceAddressLineExtension1 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. ''.
    /// </summary>
    [Name("Wohnadresse - Adresszusatzzeile 2")]
    public string ResidenceAddressLineExtension2 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'Kirchstrasse'.
    /// </summary>
    [Name("Wohnadresse - Strassenbezeichnung")]
    public string ResidenceAddressStreet { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. '10'.
    /// </summary>
    [Name("Wohnadresse - Hausnummer")]
    public string ResidenceAddressHouseNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'Postfach 2000'.
    /// </summary>
    [Name("Wohnadresse - Postfachnummer")]
    public string ResidenceAddressPostOfficeBoxText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. 'St. Gallen'.
    /// </summary>
    [Name("Wohnadresse - Ort")]
    public string ResidenceAddressTown { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. '9001'.
    /// </summary>
    [Name("Wohnadresse - Postleitzahl")]
    public string ResidenceAddressZipCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the , i.e. '1'.
    /// </summary>
    [Name("Bund (Id)")]
    public string? ConfederationCircleId { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. 'Kanton St.Gallen eidgenössisch'.
    /// </summary>
    [Name("Bund (Name)")]
    public string? ConfederationCircleName { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '17'.
    /// </summary>
    [Name("Kanton 1 (Id)")]
    public string? CantonCircleId1 { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. 'Kanton St.Gallen kantonal'.
    /// </summary>
    [Name("Kanton 1 (Name)")]
    public string? CantonCircleName1 { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '19'.
    /// </summary>
    [Name("Kanton 2 (Id)")]
    public string? CantonCircleId2 { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. 'Kanton St.Gallen kantonal zusatz'.
    /// </summary>
    [Name("Kanton 2 (Name)")]
    public string? CantonCircleName2 { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '220'.
    /// </summary>
    [Name("Bezirk 1 (Id)")]
    public string? DistrictCircleId1 { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. 'Gerichtskreis St.Gallen'.
    /// </summary>
    [Name("Bezirk 1 (Name)")]
    public string? DistrictCircleName1 { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '229'.
    /// </summary>
    [Name("Bezirk 2 (Id)")]
    public string? DistrictCircleId2 { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. 'Gerichtskreis Rebstein'.
    /// </summary>
    [Name("Bezirk 2 (Name)")]
    public string? DistrictCircleName2 { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '3203'.
    /// </summary>
    [Name("Gemeinde (Id)")]
    public string? MunicipalityCircleId { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. 'St.Gallen'.
    /// </summary>
    [Name("Gemeinde (Name)")]
    public string? MunicipalityCircleName { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. 'C'.
    /// </summary>
    [Name("Politischer Kreis (Id)")]
    public string? PoliticalCircleId { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. 'Centrum'.
    /// </summary>
    [Name("Politischer Kreis (Name)")]
    public string? PoliticalCircleName { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '100'.
    /// </summary>
    [Name("Katholischer Kirchkreis (Id)")]
    public string? CatholicCircleId { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. 'Kath. Kirchgemeinde Region St. Gallen'.
    /// </summary>
    [Name("Katholischer Kirchkreis (Name)")]
    public string? CatholicCircleName { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '100'.
    /// </summary>
    [Name("Evangelischer Kirchkreis (Id)")]
    public string? EvangelicCircleId { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. 'Evang. Kirchgemeinde St. Gallen'.
    /// </summary>
    [Name("Evangelischer Kirchkreis (Name)")]
    public string? EvangelicCircleName { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '100'.
    /// </summary>
    [Name("Schulkreis (Id)")]
    public string? SchoolCircleId { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. 'Schulgemeinde St. Gallen'.
    /// </summary>
    [Name("Schulkreis (Name)")]
    public string? SchoolCircleName { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '0'.
    /// </summary>
    [Name("Verkehrskreis (Id)")]
    public string? TrafficCircleId { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. 'kein'.
    /// </summary>
    [Name("Verkehrskreis (Name)")]
    public string? TrafficCircleName { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '39'.
    /// </summary>
    [Name("Volkszählkreis (Id)")]
    public string? PeopleCircleId { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. 'kein'.
    /// </summary>
    [Name("Volkszählkreis (Name)")]
    public string? PeopleCircleName { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '0'.
    /// </summary>
    [Name("Wohnviertel (Id)")]
    public string? ResidentialDistrictCircleId { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. 'Kreis 39'.
    /// </summary>
    [Name("Wohnviertel (Name)")]
    public string? ResidentialDistrictCircleName { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '356'.
    /// </summary>
    [Name("Korporationen (Id)")]
    public string? CorporationsCircleId { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. 'Korporation Ost/West'.
    /// </summary>
    [Name("Korporationen (Name)")]
    public string? CorporationsCircleName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets the , i.e. 'Ja' / 'Nein'.
    /// </summary>
    [Name("E-Voting")]
    public string EVoting { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the residence of type, i.e. '1'.
    /// <list type="bullet">
    ///     <item>1: HWS</item>
    ///     <item>2: NWS</item>
    ///     <item>3: undefined</item>
    /// </list>
    /// </summary>
    [Name("Meldeverhältnis-Code")]
    public int TypeOfResidenceCode { get; set; }

    /// <summary>
    /// Gets or sets the residence of type, i.e. '1' => 'Hauptwohnsitz'.
    /// <list type="bullet">
    ///     <item>1: HWS</item>
    ///     <item>2: NWS</item>
    ///     <item>3: undefined</item>
    /// </list>
    /// </summary>
    [Name("Meldeverhältnis")]
    public string? TypeOfResidence { get; set; } = string.Empty;

    /// <inheritdoc cref="PersonEntity.SendVotingCardsToDomainOfInfluenceReturnAddress"/>
    [Name("Nicht zustellen")]
    public string? SendVotingCardsToDomainOfInfluenceReturnAddress { get; set; }
}
