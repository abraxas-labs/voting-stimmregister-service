// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using CsvHelper.Configuration.Attributes;

namespace Voting.Stimmregister.Domain.Models.Import;

/// <summary>
/// CSV import model for person records from Cobra (TG).
/// </summary>
public class CobraTgPersonCsvRecord : IPersonCsvRecord
{
    /// <summary>
    /// Gets or sets the identifier of the person entry as provided by the source system, i.e. '123456789'.
    /// </summary>
    [Name("ID")]
    [Index(18)]
    public string SourceSystemId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the 'eCH-0044:vnType' AHV number, i.e. '7560000111122' for '756.0000.1111.22'.
    /// Not provided by the import.
    /// </summary>
    [Ignore]
    public long? Vn { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0011:officialName, i.e. 'Fisher'.
    /// </summary>
    [Name("Nachname")]
    [Index(3)]
    public string OfficialName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the eCH-0011:firstName, i.e. 'Jade Jamila'.
    /// </summary>
    [Name("Vorname")]
    [Index(4)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title, i.e. ''.
    /// Not required by VOTING Stimmregister.
    /// </summary>
    [Name("Titel")]
    [Index(2)]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0011:birthDataType, i.e. '21.09.1987 00:00:00'.
    /// </summary>
    [Name("Geburtstag")]
    [Index(15)]
    public string DateOfBirth { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the eCH-0045:residenceCountry, i.e. 'DE'.
    /// Data Conversion: Convert from long name to standardized ISO 3166-1 (alpha-2) code.
    /// The COBRA field 'Land' and 'LandKurz' can be used to get a better result in the data conversion.
    /// </summary>
    [Name("Land")]
    [Index(13)]
    public string? ResidenceCountry { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0045:residenceCountry, i.e. 'DE'.
    /// Data Conversion: Convert from long name to standardized ISO 3166-1 (alpha-2) code.
    /// The COBRA field 'Land' and 'LandKurz' can be used to get a better result in the data conversion.
    /// </summary>
    [Name("LandKurz")]
    [Index(14)]
    public string? ResidenceCountryShort { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0007:swissMunicipalityType, i.e. '9200' for Swiss abroad.
    /// The municipality id (BFS) is used during the import to differentiate individual DOI imports on a municipality base.
    /// Further, it is required for filtering to improve the query performance.
    /// </summary>
    [Ignore]
    public int MunicipalityId { get; set; } = 9200;

    /// <summary>
    /// Gets or sets the eCH-0045:languageOfCorrespondence code, i.e. 'de'.
    /// </summary>
    [Name("Sprache")]
    [Index(16)]
    public string? LanguageOfCorrespondence { get; set; }

    /// <summary>
    /// Gets or sets the salutation, i.e. 'Frau'.
    /// Required for mapping 'Sex':
    /// <list type="bullet">
    ///     <item>Herr: male</item>
    ///     <item>Frau: female</item>
    ///     <item>all others: undefined</item>
    /// </list>
    /// </summary>
    [Name("Anrede")]
    [Index(1)]
    public string? Salutation { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:AddressLine1, i.e. 'Apartado 1'.
    /// </summary>
    [Name("Adresszusatz")]
    [Index(5)]
    public string? ContactAddressExtensionLine1 { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:AddressLine2, i.e. 'Apto. 14a'.
    /// </summary>
    [Name("Strassezusatz")]
    [Index(9)]
    public string? ContactAddressExtensionLine2 { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:streetType, i.e. 'St. Leonhardstrasse'.
    /// </summary>
    [Name("Strasse")]
    [Index(6)]
    public string? ContactAddressStreet { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:houseNumberType, i.e. '5'.
    /// </summary>
    [Name("Hausnummer")]
    [Index(7)]
    public string? ContactAddressHouseNumber { get; set; }

    /// <summary>
    /// Gets or sets the eCH0010:postOfficeBoxTextType, i.e. 'SA-19'.
    /// </summary>
    [Name("Postfach")]
    [Index(8)]
    public string? ContactAddressPostOfficeBoxText { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:swissZipCodeType, i.e. '9000'.
    /// </summary>
    [Name("PLZ")]
    [Index(10)]
    public string? ContactAddressZipCode { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:townType, i.e. 'Hermanus'.
    /// </summary>
    [Name("Ort")]
    [Index(11)]
    public string? ContactAddressTown { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:locality, i.e. 'Western Cape'.
    /// </summary>
    [Name("Ortszusatz")]
    [Index(12)]
    public string? ContactAddressLocality { get; set; }

    /// <summary>
    /// Gets or sets the mode of shipment, i.e. 'ÜL'.
    /// Not required by VOTING Stimmregister.
    /// </summary>
    [Name("Versandart")]
    [Index(17)]
    public string? ModeOfShipment { get; set; }

    /// <summary>
    /// Gets or sets the native place according to eCH-0011:originName, i.e. 'Lauterbrunnen'.
    /// </summary>
    [Name("Stimmgemeinde")]
    [Index(0)]
    public string? OriginName1 { get; set; }

    /// <summary>
    /// Gets or sets the active flag, i.e. ''.
    /// Not required by VOTING Stimmregister.
    /// </summary>
    [Name("Aktiv")]
    [Index(19)]
    public string? Active { get; set; }

    /// <summary>
    /// Gets or sets the combination of street and house number, i.e. 'Rue 12'.
    /// Not required by VOTING Stimmregister.
    /// </summary>
    [Name("Komb StrNr")]
    [Index(20)]
    public string? CombinedStreetAndHouseNumber { get; set; }

    /// <summary>
    /// Gets or sets the combination of postal code and locality, i.e. '34533 Musterstadt'.
    /// Not required by VOTING Stimmregister.
    /// </summary>
    [Name("Komb PLZOrt")]
    [Index(21)]
    public string? CombinedPostalCodeandLocality { get; set; }

    /// <summary>
    /// Gets a record identifier for troubleshooting if a record couldn't be imported due to record validation errors.
    /// </summary>
    /// <returns>A record identifier literal.</returns>
    public string BuildRecordIdentifier()
        => $"{FirstName} {OfficialName} ({DateOfBirth}, {SourceSystemId})";
}
