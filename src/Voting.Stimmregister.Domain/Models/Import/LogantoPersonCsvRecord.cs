// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using CsvHelper.Configuration.Attributes;

namespace Voting.Stimmregister.Domain.Models.Import;

/// <summary>
/// CSV import model for person records.
/// </summary>
public class LogantoPersonCsvRecord : IPersonCsvRecord
{
    /// <summary>
    /// Gets or sets the identifier of the person entry as provided by the source system, i.e. '6938242'.
    /// </summary>
    [Name("SCHLUESSEL")]
    public string SourceSystemId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the 'eCH-0044:vnType' AHV number, i.e. '7560000111122' for '756.0000.1111.22'.
    /// A person is uniquely identified by the AHV number (AHVN). The AHVN can exist several times for the same person if several residential status exist (HWS, NWS).
    /// </summary>
    [Name("VN")]
    public long? Vn { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0011:officialName, i.e. 'Fisher'.
    /// </summary>
    [Name("OFFICIALNAME")]
    public string OfficialName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the eCH-0011:firstName, i.e. 'Jade Jamila'.
    /// </summary>
    [Name("FIRSTNAME")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the eCH-0044:sexType, i.e. 'W' for 'female'.
    /// <list type="bullet">
    ///     <item>?: undefined</item>
    ///     <item>M: male</item>
    ///     <item>F: female</item>
    /// </list>
    /// </summary>
    [Name("SEX")]
    public string? Sex { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0011:birthDataType, i.e. '21.09.1987'.
    /// </summary>
    [Name("DATEOFBIRTH")]
    public string DateOfBirth { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the eCH-0011:originalName, i.e. 'Tinner'.
    /// </summary>
    [Name("ORIGINALNAME")]
    public string? OriginalName { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0011:allianceName, i.e. 'Tinner'.
    /// </summary>
    [Name("ALLIANCENAME")]
    public string? AllianceName { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0011:aliasName, i.e. 'Fisher'.
    /// </summary>
    [Name("ALIASNAME")]
    public string? AliasName { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0011:otherName, i.e. 'Tinner'.
    /// </summary>
    [Name("OTHERNAME")]
    public string? OtherName { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0011:callName, i.e. 'Jade'.
    /// </summary>
    [Name("CALLNAME")]
    public string? CallName { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0008:countryType, i.e. 'CH'.
    /// </summary>
    [Name("COUNTRY")]
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:addressLineType, i.e. 'z.H. Max Muster'.
    /// </summary>
    [Name("ZUA_ADDRESSLINE1")]
    public string? ContactAddressExtensionLine1 { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:addressLineType, i.e. 'Abteilung Chuchichäschtli'.
    /// </summary>
    [Name("ZUA_ADDRESSLINE2")]
    public string? ContactAddressExtensionLine2 { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:streetType, i.e. 'St. Leonhardstrasse'.
    /// </summary>
    [Name("ZUA_STREET")]
    public string? ContactAddressStreet { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:houseNumberType, i.e. '5b'.
    /// </summary>
    [Name("ZUA_HOUSENUMBER")]
    public string? ContactAddressHouseNumber { get; set; }

    /// <summary>
    /// Gets or sets the dwellingnumber, i.e. '12'.
    /// </summary>
    [Name("ZUA_DWELLINGNUMBER")]
    public string? ContactAddressDwellingNumber { get; set; }

    /// <summary>
    /// Gets or sets the eCH0010:postOfficeBoxTextType, i.e. 'Postfach 3001'.
    /// </summary>
    [Name("ZUA_POSTOFFICEBOXNUMBER")]
    public string? ContactAddressPostOfficeBoxText { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:townType, i.e. 'St. Gallen'.
    /// </summary>
    [Name("ZUA_TOWN")]
    public string? ContactAddressTown { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:swissZipCodeType or eCH-0010:foreignZipCodeType, i.e. '9000' or 'EH129DN'.
    /// The property naming does not only imply swiss zip codes, but also foreign zip codes.
    /// </summary>
    [Name("ZUA_SWISSZIPCODE")]
    public string? ContactAddressSwissZipCode { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0007:cantonAbbreviation, i.e. 'SG'.
    /// </summary>
    [Name("ZUA_KANTON")]
    public string? ContactCantonAbbreviation { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:countryIdISO2Type, i.e. 'CH'.
    /// </summary>
    [Name("ZUA_COUNTRY")]
    public string? ContactAddressCountryIdIso2 { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0011:religionType, i.e. '711' for 'denominational'.
    /// <list type="bullet">
    ///     <item>111: evangelic</item>
    ///     <item>121: catholic</item>
    ///     <item>122: christ-catholic / old-catholic (valid for cantons ZH, BE, LU, SO,BS, BL, SH, SG, AG, NE, GE)</item>
    ///     <item>211: jewish community of faith (valid for cantons BE, FR, BS, SG,VD)</item>
    ///     <item>211201: israelite community of faith (valid for cantons ZH)</item>
    ///     <item>211301: jewish liberal community (valid for cantons ZH)</item>
    ///     <item>711: denominational</item>
    ///     <item>811: unacknowledged religious community</item>
    ///     <item>000: unknown</item>
    /// </list>
    /// </summary>
    [Name("RELIGION")]
    public string? Religion { get; set; }

    /// <summary>
    /// Gets or sets the native place according to eCH-0011:originName, i.e. 'St. Gallen'.
    /// </summary>
    [Name("ORIGINNAME_1")]
    public string? OriginName1 { get; set; }

    /// <summary>
    /// Gets or sets the native place according to eCH-0011:originName, i.e. 'Oberhelfenschwil'.
    /// </summary>
    [Name("ORIGINNAME_2")]
    public string? OriginName2 { get; set; }

    /// <summary>
    /// Gets or sets the native place according to eCH-0011:originName, i.e. 'Baar'.
    /// </summary>
    [Name("ORIGINNAME_3")]
    public string? OriginName3 { get; set; }

    /// <summary>
    /// Gets or sets the native place according to eCH-0011:originName, i.e. 'Lauterbrunnen'.
    /// </summary>
    [Name("ORIGINNAME_4")]
    public string? OriginName4 { get; set; }

    /// <summary>
    /// Gets or sets the native place according to eCH-0011:originName, i.e. 'Lauterbrunnen'.
    /// </summary>
    [Name("ORIGINNAME_5")]
    public string? OriginName5 { get; set; }

    /// <summary>
    /// Gets or sets the native place according to eCH-0011:originName, i.e. 'Lauterbrunnen'.
    /// </summary>
    [Name("ORIGINNAME_6")]
    public string? OriginName6 { get; set; }

    /// <summary>
    /// Gets or sets the native place according to eCH-0011:originName, i.e. 'Lauterbrunnen'.
    /// </summary>
    [Name("ORIGINNAME_7")]
    public string? OriginName7 { get; set; }

    /// <summary>
    /// Gets or sets the canton of the native place1, i.e. 'SG'.
    /// </summary>
    [Name("ON_CANTON_1")]
    public string? OnCanton1 { get; set; }

    /// <summary>
    /// Gets or sets the canton of the native place 2, i.e. 'SG'.
    /// </summary>
    [Name("ON_CANTON_2")]
    public string? OnCanton2 { get; set; }

    /// <summary>
    /// Gets or sets the canton of the native place 3, i.e. 'ZG'.
    /// </summary>
    [Name("ON_CANTON_3")]
    public string? OnCanton3 { get; set; }

    /// <summary>
    /// Gets or sets the canton of the native place 4, i.e. 'SG'.
    /// </summary>
    [Name("ON_CANTON_4")]
    public string? OnCanton4 { get; set; }

    /// <summary>
    /// Gets or sets the canton of the native place 5, i.e. 'SG'.
    /// </summary>
    [Name("ON_CANTON_5")]
    public string? OnCanton5 { get; set; }

    /// <summary>
    /// Gets or sets the canton of the native place 6, i.e. 'SG'.
    /// </summary>
    [Name("ON_CANTON_6")]
    public string? OnCanton6 { get; set; }

    /// <summary>
    /// Gets or sets the canton of the native place 7, i.e. 'SG'.
    /// </summary>
    [Name("ON_CANTON_7")]
    public string? OnCanton7 { get; set; }

    /// <summary>
    /// Gets or sets the residence permit number, i.e. '302'.
    /// </summary>
    [Name("RESIDENCEPERMIT")]
    public string? ResidencePermit { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '15.03.2015'.
    /// </summary>
    [Name("RESIDENCEPERMITVALIDFROM")]
    public string? ResidencePermitValidFrom { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '99.99.9999'.
    /// </summary>
    [Name("RESIDENCEPERMITVALIDTILL")]
    public string? ResidencePermitValidTill { get; set; }

    /// <summary>
    /// Gets or sets the residence entry date, i.e. '06.08.2020'.
    /// </summary>
    [Name("ENTRYDATE")]
    public string? ResidenceEntryDate { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0007:swissMunicipalityType, i.e. 'St. Gallen'.
    /// </summary>
    [Name("MUNICIPALITYNAME")]
    public string? MunicipalityName { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0007:swissMunicipalityType, i.e. '3203'.
    /// </summary>
    [Name("MUNICIPALITYID")]
    public int MunicipalityId { get; set; }

    /// <summary>
    /// Gets or sets the arrival date when a person immigrated to the current residence, i.e. '01.07.1994'.
    /// </summary>
    [Name("ARRIVALDATE")]
    public string? MoveInArrivalDate { get; set; }

    /// <summary>
    /// Gets or sets the municipality name where the person migrated from, i.e. 'St. Gallen'.
    /// </summary>
    [Name("ZU_MUNICIPALITYNAME")]
    public string? MoveInMunicipalityName { get; set; }

    /// <summary>
    /// Gets or sets the canton abbreviation where the person immigrated from, i.e. 'SG'.
    /// </summary>
    [Name("ZU-CANTONABBREVIATION")]
    public string? MoveInCantonAbbreviation { get; set; }

    /// <summary>
    /// Gets or sets the town where the person immigrated from, i.e. 'unbekannt'.
    /// </summary>
    [Name("ZU_TOWN")]
    public string? MoveInComeFrom { get; set; }

    /// <summary>
    /// Gets or sets the country where the person immigrated from, i.e. 'Deutschland'.
    /// </summary>
    [Name("ZU_COUNTRYNAMESHORT")]
    public string? MoveInCountryNameShort { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the immigrated information is unknown, i.e. 'X'.
    /// </summary>
    [BooleanFalseValues("")]
    [BooleanTrueValues("X")]
    [Name("ZU_UNKNOWN")]
    public bool MoveInUnknown { get; set; }

    /// <summary>
    /// Gets or sets the domain of influence id, i.e. '906'.
    /// This field is used to uniquely identity the data entry for a particular street and its associated circles.
    /// The identification is built by the street name, street number, street addition and postal code.
    /// It is primarily used to find the associated circles for one or more persons and vice versa.
    /// </summary>
    [Name("WOA_ADDRESS_ID")]
    public int? DomainOfInfluenceId { get; set; }

    /// <summary>
    /// Gets or sets the residence address eCH-0010:addressLineType, i.e. 'Missionshaus Untere Waid'.
    /// </summary>
    [Name("WOA_ADDRESSLINE1")]
    public string? ResidenceAddressExtensionLine1 { get; set; }

    /// <summary>
    /// Gets or sets the residence address eCH-0010:addressLineType, i.e. ''.
    /// </summary>
    [Name("WOA_ADDRESSLINE2")]
    public string? ResidenceAddressExtensionLine2 { get; set; }

    /// <summary>
    /// Gets or sets the residence address eCH-0010:streetType, i.e. 'Bahnhofstr.'.
    /// </summary>
    [Name("WOA_STREET")]
    public string? ResidenceAddressStreet { get; set; }

    /// <summary>
    /// Gets or sets the residence address eCH-0010:houseNumberType, i.e. '23a'.
    /// </summary>
    [Name("WOA_HOUSENUMBER")]
    public string? ResidenceAddressHouseNumber { get; set; }

    /// <summary>
    /// Gets or sets the ???, i.e. '???'.
    /// </summary>
    [Name("WOA_DWELLINGNUMBER")]
    public string? ResidenceAddressDwellingNumber { get; set; }

    /// <summary>
    /// Gets or sets the residence address eCH0010:postOfficeBoxTextType, i.e. 'Postfach 1000'.
    /// </summary>
    [Name("WOA_POSTOFFICEBOXNUMBER")]
    public string? ResidenceAddressPostOfficeBoxText { get; set; }

    /// <summary>
    /// Gets or sets the residence address eCH-0010:townType, i.e. 'Mörschwil'.
    /// </summary>
    [Name("WOA_TOWN")]
    public string? ResidenceAddressTown { get; set; }

    /// <summary>
    /// Gets or sets the residence address eCH-0010:swissZipCodeType, i.e. '9402'.
    /// </summary>
    [Name("WOA-SWISSZIPCODE")]
    public string? ResidenceAddressSwissZipCode { get; set; }

    /// <summary>
    /// Gets or sets the residence canton abbreviation eCH-0007:cantonAbbreviation, i.e. 'SG'.
    /// </summary>
    [Name("WOA_KANTON")]
    public string? ResidenceCantonAbbreviation { get; set; }

    /// <summary>
    /// Gets or sets the emigration data, i.e. ''.
    /// </summary>
    [Name("DEPARTUREDATE")]
    public string? EmigrationDate { get; set; }

    /// <summary>
    /// Gets or sets the emigration municipality name, i.e. ''.
    /// </summary>
    [Name("WEGZ_MUNICIPALITYNAME")]
    public string? EmigrationMunicipalityName { get; set; }

    /// <summary>
    /// Gets or sets the emigration canton abbreviation, i.e. ''.
    /// </summary>
    [Name("WEGZ_CANTONABBREVIATION")]
    public string? EmigrationCantonAbbreviation { get; set; }

    /// <summary>
    /// Gets or sets the emigration town, i.e. ''.
    /// </summary>
    [Name("WEGZ_TOWN")]
    public string? EmigrationTown { get; set; }

    /// <summary>
    /// Gets or sets the emigration country short name, i.e. ''.
    /// </summary>
    [Name("WEGZ_COUNTRYNAMESHORT")]
    public string? EmigrationCountryNameShort { get; set; }

    /// <summary>
    /// Gets or sets whether the emigration status unknown or not, i.e. ''.
    /// </summary>
    [Name("WEGZ_UNKNOWN")]
    public string? EmigrationUnknown { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the person has a main residence or not according to eCH-0011:typeOfResidenceType[1], i.e. 'X'.
    /// Indicates whether the given address of residence is the persons main residence.
    /// Note: If a person with HWS has transferred the voting right over to the NWS, the field RestrictedVotingAndElectionRightFederation is set to true.
    /// The person has no voting right at the HWS anymore in this case. A person can exist twice with the same AVHN if both, HWS and NWS is used.
    /// </summary>
    [BooleanFalseValues("")]
    [BooleanTrueValues("X")]
    [Name("HWS")]
    public bool HasMainResidence { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the person has a secondary residence according to eCH-0011:typeOfResidenceType[1], i.e. 'X'.
    /// Indicates whether the given address of residence is the persons secondary residence.
    /// Note: Persons with a secondary residence and no assigned circle information are not permitted to vote.
    /// Otherwise, if a person has transferred its voting right from HWS to the NWS, the voting right is given and circle information is set.
    /// </summary>
    [BooleanFalseValues("")]
    [BooleanTrueValues("X")]
    [Name("NWS")]
    public bool HasSecondaryResidence { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the voting right as xs:boolean (eCH unknown), i.e. 'false'.
    /// </summary>
    [BooleanFalseValues("")]
    [BooleanTrueValues("X")]
    [Name("VOTINGRIGHT")]
    public bool RestrictedVotingAndElectionRightFederation { get; set; }

    /// <summary>
    /// Gets or sets additional info of the voting right.
    /// The only currently well known value is 'P' (people without permanent domicile).
    /// </summary>
    [Name("VOTINGRIGHT_ADDITION")]
    [Optional]
    public string? VotingRightAddition { get; set; }

    /// <summary>
    /// Gets or sets the political circle id, i.e. 'C'.
    /// </summary>
    [Name("POLKREIS")]
    public string? PoliticalCircleId { get; set; }

    /// <summary>
    /// Gets or sets the political circle name, i.e. 'Centrum'.
    /// </summary>
    [Name("POLKREIS_TXT")]
    public string? PoliticalCircleName { get; set; }

    /// <summary>
    /// Gets or sets the catholic circle id, i.e. '200'.
    /// </summary>
    [Name("KATHKREIS")]
    public string? CatholicCircleId { get; set; }

    /// <summary>
    /// Gets or sets the catholic circle name, i.e. 'Kath. Kirchgemeinde Region Rorschach'.
    /// </summary>
    [Name("KATHKREIS_TXT")]
    public string? CatholicCircleName { get; set; }

    /// <summary>
    /// Gets or sets the evangelic circle id, i.e. '100'.
    /// </summary>
    [Name("EVANGKREIS")]
    public string? EvangelicCircleId { get; set; }

    /// <summary>
    /// Gets or sets the evangelic circle name, i.e. 'Evang. Kirchkreis Rotmonten'.
    /// </summary>
    [Name("EVANGKREIS_TXT")]
    public string? EvangelicCircleName { get; set; }

    /// <summary>
    /// Gets or sets the school circle id, i.e. '100'.
    /// </summary>
    [Name("SCHULKREIS")]
    public string? SchoolCircleId { get; set; }

    /// <summary>
    /// Gets or sets the school circle name, i.e. 'Schulzählkreis Rotmonten-Gerhalde'.
    /// </summary>
    [Name("SCHULKREIS_TXT")]
    public string? SchoolCircleName { get; set; }

    /// <summary>
    /// Gets or sets the traffic circle id, i.e. '0'.
    /// </summary>
    [Name("VERKEHRSKREIS")]
    public string? TrafficCircleId { get; set; }

    /// <summary>
    /// Gets or sets the traffic circle name, i.e. '0'.
    /// </summary>
    [Name("VERKEHRSKREIS_TXT")]
    public string? TrafficCircleName { get; set; }

    /// <summary>
    /// Gets or sets the folk circle id, i.e. '103'.
    /// </summary>
    [Name("VOLKSZKREIS")]
    public string? PeopleCircleId { get; set; }

    /// <summary>
    /// Gets or sets the folk circle name, i.e. 'Quartierverein Rotmonten'.
    /// </summary>
    [Name("VOLKSZKREIS_TXT")]
    public string? PeopleCircleName { get; set; }

    /// <summary>
    /// Gets or sets the language of correspondence (EK identifier), i.e. 'I' for Italian.
    /// </summary>
    [Name("LanguageOfCorrespondence")]
    public string? LanguageOfCorrespondence { get; set; }

    /// <summary>
    /// Gets or sets the residential district circle id, i.e. '10'.
    /// </summary>
    [Name("WOHNVIERTEL")]
    public string? ResidentialDistrictCircleId { get; set; }

    /// <summary>
    /// Gets or sets the residential district circle name, i.e. 'Rotmonten (Rotmonten) Osten'.
    /// </summary>
    [Name("WOHNVIERTEL_TXT")]
    public string? ResidentialDistrictCircleName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the person is the householder as xs:boolean (eCH unknown), i.e. 'false'.
    /// </summary>
    [BooleanFalseValues("")]
    [BooleanTrueValues("X")]
    [Name("HEAD_OF_HH_TYPE")]
    public bool IsHouseholder { get; set; }

    /// <summary>
    /// Gets or sets the residence building id, i.e. '10'.
    /// </summary>
    [Name("EGID")]
    public int? ResidenceBuildingId { get; set; }

    /// <summary>
    /// Gets or sets the residence apartment/flat id, i.e. '10'.
    /// </summary>
    [Name("EWID")]
    public int? ResidenceApartmentId { get; set; }

    public string BuildRecordIdentifier()
        => $"{FirstName} {OfficialName} ({DateOfBirth}, {Vn})";

    public bool HasResidenceAddress()
        => !string.IsNullOrWhiteSpace(ResidenceAddressExtensionLine1)
            || !string.IsNullOrWhiteSpace(ResidenceAddressExtensionLine2)
            || !string.IsNullOrWhiteSpace(ResidenceAddressStreet)
            || !string.IsNullOrWhiteSpace(ResidenceAddressHouseNumber)
            || !string.IsNullOrWhiteSpace(ResidenceAddressDwellingNumber)
            || !string.IsNullOrWhiteSpace(ResidenceAddressPostOfficeBoxText)
            || !string.IsNullOrWhiteSpace(ResidenceAddressTown)
            || !string.IsNullOrWhiteSpace(ResidenceAddressSwissZipCode)
            || !string.IsNullOrWhiteSpace(ResidenceCantonAbbreviation);

    public bool HasContactAddress()
        => !string.IsNullOrWhiteSpace(ContactAddressExtensionLine1)
            || !string.IsNullOrWhiteSpace(ContactAddressExtensionLine2)
            || !string.IsNullOrWhiteSpace(ContactAddressStreet)
            || !string.IsNullOrWhiteSpace(ContactAddressHouseNumber)
            || !string.IsNullOrWhiteSpace(ContactAddressDwellingNumber)
            || !string.IsNullOrWhiteSpace(ContactAddressPostOfficeBoxText)
            || !string.IsNullOrWhiteSpace(ContactAddressTown)
            || !string.IsNullOrWhiteSpace(ContactAddressSwissZipCode)
            || !string.IsNullOrWhiteSpace(ContactCantonAbbreviation);
}
