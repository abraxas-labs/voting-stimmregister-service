// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Domain.Models;

/// <summary>
/// Person database model.
/// </summary>
public class PersonEntity : ImportedEntity
{
    /// <summary>
    /// Gets or sets the person's unique register identifier.
    /// Note: A person can have multiple versions and therefore multiple entities with version id's (), but just one RegisterId.
    /// </summary>
    public Guid RegisterId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the person entry as provided by the source system, i.e. '6938242'.
    /// </summary>
    public string? SourceSystemId { get; set; }

    /// <summary>
    /// Gets or sets the source system name, i.e. 'Cobra' or 'Loganto'.
    /// <list type="bullet">
    ///     <item>0: Unspecified</item>
    ///     <item>1: Loganto</item>
    ///     <item>2: Cobra</item>
    ///     <item>3: VotingBasis</item>
    /// </list>
    /// </summary>
    public ImportSourceSystem SourceSystemName { get; set; }

    /// <summary>
    /// Gets or sets the 'eCH-0044:vnType' AHV number, i.e. '7560000111122' for '756.0000.1111.22'.
    /// A person is uniquely identified by the AHV number (AHVN).
    /// The AHVN can exist several times for the same person if several residential status exist (HWS, NWS).
    /// There is no AHV number available for Swiss abroad.
    /// </summary>
    public long? Vn { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0011:officialName, i.e. 'Fisher'.
    /// </summary>
    public string OfficialName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the eCH-0011:firstName, i.e. 'Jade Jamila'.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the eCH-0044:sexType, i.e. 'W' for 'female'.
    /// <list type="bullet">
    ///     <item>?: undefined</item>
    ///     <item>M: male</item>
    ///     <item>F: female</item>
    /// </list>
    /// </summary>
    public SexType Sex { get; set; } = SexType.Undefined;

    /// <summary>
    /// Gets or sets the eCH-0011:birthDataType, i.e. '21.09.1987'.
    /// </summary>
    public DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="DateOfBirth"/> field required adjustment from import data.
    /// </summary>
    public bool DateOfBirthAdjusted { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0011:originalName, i.e. 'Tinner'.
    /// </summary>
    public string? OriginalName { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0011:allianceName, i.e. 'Tinner'.
    /// </summary>
    public string? AllianceName { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0011:aliasName, i.e. 'Fisher'.
    /// </summary>
    public string? AliasName { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0011:otherName, i.e. 'Tinner'.
    /// </summary>
    public string? OtherName { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0011:callName, i.e. 'Jade'.
    /// </summary>
    public string? CallName { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0008:countryType (countryIdISO2Type), i.e. 'CH'.
    /// Nationality of a person.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0008:countryType (countryNameShort), i.e. 'Schweiz'.
    /// </summary>
    public string? CountryNameShort { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:addressLineType, i.e. 'z.H. Max Muster'.
    /// </summary>
    public string? ContactAddressExtensionLine1 { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:addressLineType, i.e. 'Abteilung Chuchichäschtli'.
    /// </summary>
    public string? ContactAddressExtensionLine2 { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:streetType, i.e. 'St. Leonhardstrasse'.
    /// </summary>
    public string? ContactAddressStreet { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:houseNumberType, i.e. '5b'.
    /// </summary>
    public string? ContactAddressHouseNumber { get; set; }

    /// <summary>
    /// Gets or sets the dwellingnumber, i.e. '12'.
    /// </summary>
    public string? ContactAddressDwellingNumber { get; set; }

    /// <summary>
    /// Gets or sets the eCH0010:postOfficeBoxTextType, i.e. 'Postfach 2000'.
    /// </summary>
    public string? ContactAddressPostOfficeBoxText { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:postOfficeBoxNumberType, i.e. '2000'.
    /// </summary>
    public int? ContactAddressPostOfficeBoxNumber { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:townType, i.e. 'St. Gallen'.
    /// </summary>
    public string? ContactAddressTown { get; set; }

    /// <summary>
    /// Gets or sets the zip code.
    /// Format for Swiss citizens: eCH-0010:swissZipCodeType, i.e. '9000'.
    /// Format for Swiss abroad: i.e. 'EH3 9QN'.
    /// </summary>
    public string? ContactAddressZipCode { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:locality, i.e. 'Western Cape'.
    /// Cobra field only.
    /// </summary>
    public string? ContactAddressLocality { get; set; }

    /// <summary>
    /// Gets or sets the eCH0045:person (Extension) contact address line, i.e. 'Apartment 86'.
    /// Part of the addresses for Swiss abroad do not use the common street fields, instead address line 1-7 are filled.
    /// Cobra field only.
    /// </summary>
    public string? ContactAddressLine1 { get; set; }

    /// <summary>
    /// Gets or sets the eCH0045:person (Extension) contact address line, i.e. 'Molinaccio'.
    /// refer to: <see cref="ContactAddressLine1"/>.
    /// Cobra field only.
    /// </summary>
    public string? ContactAddressLine2 { get; set; }

    /// <summary>
    /// Gets or sets the eCH0045:person (Extension) contact address line, i.e. 'Playa'.
    /// refer to: <see cref="ContactAddressLine1"/>.
    /// Cobra field only.
    /// </summary>
    public string? ContactAddressLine3 { get; set; }

    /// <summary>
    /// Gets or sets the eCH0045:person (Extension) contact address line, i.e. 'P.O. Box 4'.
    /// refer to: <see cref="ContactAddressLine1"/>.
    /// Cobra field only.
    /// </summary>
    public string? ContactAddressLine4 { get; set; }

    /// <summary>
    /// Gets or sets the eCH0045:person (Extension) contact address line, i.e. '74160 Beaumont'.
    /// refer to: <see cref="ContactAddressLine1"/>.
    /// Cobra field only.
    /// </summary>
    public string? ContactAddressLine5 { get; set; }

    /// <summary>
    /// Gets or sets the eCH0045:person (Extension) contact address line, i.e. 'Dorking'.
    /// refer to: <see cref="ContactAddressLine1"/>.
    /// Cobra field only.
    /// </summary>
    public string? ContactAddressLine6 { get; set; }

    /// <summary>
    /// Gets or sets the eCH0045:person (Extension) contact address line, i.e. 'AL1 3GW'.
    /// refer to: <see cref="ContactAddressLine1"/>.
    /// Cobra field only.
    /// </summary>
    public string? ContactAddressLine7 { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0007:cantonAbbreviation, i.e. 'SG'.
    /// </summary>
    public string? ContactCantonAbbreviation { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0010:countryIdISO2Type, i.e. 'CH'.
    /// </summary>
    public string? ContactAddressCountryIdIso2 { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0045:languageOfCorrespondence, i.e. 'de'.
    /// Cobra field only.
    /// </summary>
    public string? LanguageOfCorrespondence { get; set; }

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
    public ReligionType Religion { get; set; }

    /// <summary>
    /// Gets or sets the residence permit number, i.e. '30'.
    /// Importers usually only store the first two chars.
    /// </summary>
    public string? ResidencePermit { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '15.03.2015'.
    /// </summary>
    public DateOnly? ResidencePermitValidFrom { get; set; }

    /// <summary>
    /// Gets or sets the , i.e. '99.99.9999'.
    /// </summary>
    public DateOnly? ResidencePermitValidTill { get; set; }

    /// <summary>
    /// Gets or sets the residence entry date, i.e. '06.08.2020'.
    /// </summary>
    public DateOnly? ResidenceEntryDate { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0007:cantonAbbreviation, i.e. 'SG'.
    /// </summary>
    public string? ResidenceCantonAbbreviation { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0007:swissMunicipalityType, i.e. 'St. Gallen'.
    /// </summary>
    public string? MunicipalityName { get; set; }

    /// <summary>
    /// Gets or sets the domain of influence id, i.e. '906'.
    /// This field is used to uniquely identity the data entry for a particular street and its associated circles.
    /// The identification is built by the street name, street number, street addition and postal code.
    /// It is primarily used to find the associated circles for one or more persons and vice versa.
    /// </summary>
    public int? DomainOfInfluenceId { get; set; }

    /// <summary>
    /// Gets or sets the arrival date when a person immigrated to the current residence, i.e. '01.07.1994'.
    /// </summary>
    public DateOnly? MoveInArrivalDate { get; set; }

    /// <summary>
    /// Gets or sets the municipality name where the person migrated from, i.e. 'St. Gallen'.
    /// </summary>
    public string? MoveInMunicipalityName { get; set; }

    /// <summary>
    /// Gets or sets the canton abbreviation where the person immigrated from, i.e. 'SG'.
    /// </summary>
    public string? MoveInCantonAbbreviation { get; set; }

    /// <summary>
    /// Gets or sets the town where the person immigrated from, i.e. 'unbekannt'.
    /// </summary>
    public string? MoveInComesFrom { get; set; }

    /// <summary>
    /// Gets or sets the country where the person immigrated from, i.e. 'Deutschland'.
    /// </summary>
    public string? MoveInCountryNameShort { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the immigrated information is unknown, i.e. 'X'.
    /// </summary>
    public bool? MoveInUnknown { get; set; }

    /// <summary>
    /// Gets or sets the residence address eCH-0010:addressLineType, i.e. 'Missionshaus Untere Waid'.
    /// </summary>
    public string? ResidenceAddressExtensionLine1 { get; set; }

    /// <summary>
    /// Gets or sets the residence address eCH-0010:addressLineType, i.e. ''.
    /// </summary>
    public string? ResidenceAddressExtensionLine2 { get; set; }

    /// <summary>
    /// Gets or sets the residence address eCH-0010:streetType, i.e. 'Bahnhofstr.'.
    /// </summary>
    public string? ResidenceAddressStreet { get; set; }

    /// <summary>
    /// Gets or sets the residence address eCH-0010:houseNumberType, i.e. '23a'.
    /// </summary>
    public string? ResidenceAddressHouseNumber { get; set; }

    /// <summary>
    /// Gets or sets the ???, i.e. '???'.
    /// </summary>
    public string? ResidenceAddressDwellingNumber { get; set; }

    /// <summary>
    /// Gets or sets the residence address eCH0010:postOfficeBoxTextType, i.e. 'Postfach 2400'.
    /// </summary>
    public string? ResidenceAddressPostOfficeBoxText { get; set; }

    /// <summary>
    /// Gets or sets the residence address eCH-0010:townType, i.e. 'Mörschwil'.
    /// </summary>
    public string? ResidenceAddressTown { get; set; }

    /// <summary>
    /// Gets or sets the eCH-0045:residenceCountry, i.e. 'DE'.
    /// Data Conversion: Convert from long name to standardized ISO 3166-1 (alpha-2) code.
    /// The COBRA field 'Bfs-Ländernummer' can be used to get a better result in the data conversion.
    /// </summary>
    public string? ResidenceCountry { get; set; }

    /// <summary>
    /// Gets or sets the residence address eCH-0010:swissZipCodeType, i.e. '9402'.
    /// </summary>
    public string? ResidenceAddressZipCode { get; set; }

    /// <summary>
    /// Gets or sets the type of residence, i.e. 'HWS'.
    /// </summary>
    public ResidenceType TypeOfResidence { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the voting right as xs:boolean (eCH unknown), i.e. 'false'.
    /// </summary>
    public bool RestrictedVotingAndElectionRightFederation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the person is registered for e-voting or not.
    /// Synced by the importers by reading the additional eVoter state (whether a person enabled eVoting).
    /// Therefore it can take up to the next importer run for an eVoter state change to be reflected on the person.
    /// </summary>
    public bool EVoting { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the person is a swiss person living abroad.
    /// </summary>
    public bool IsSwissAbroad { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the voting documents are sent to the address of the voter (<c>false</c>)
    /// or to the domain of influence documents return address (eg. the municipality administration, <c>true</c>).
    /// This is usually true for people without a permanent domicile.
    /// In german usually called 'nicht Zustellen' (do not deliver to the citizen).
    /// </summary>
    public bool SendVotingCardsToDomainOfInfluenceReturnAddress { get; set; }

    /// <summary>
    /// Gets or sets the dataset created date.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the dataset modified date.
    /// </summary>
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// Gets or sets the dataset modified date.
    /// </summary>
    public DateTime? DeletedDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this entity is deleted.
    /// </summary>
    public bool IsDeleted
    {
        get => DeletedDate.HasValue;
        set => _ = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the version of person data ist the latest.
    /// </summary>
    public bool IsLatest { get; set; }

    /// <summary>
    /// Gets or sets the version count.
    /// </summary>
    public int VersionCount { get; set; }

    // References

    /// <summary>
    /// Gets or sets the import id reference.
    /// </summary>
    public Guid? ImportStatisticId { get; set; }

    /// <summary>
    /// Gets or sets the import entity reference.
    /// </summary>
    public ImportStatisticEntity? ImportStatistic { get; set; }

    /// <summary>
    /// Gets or sets the filter version persons.
    /// </summary>
    public HashSet<FilterVersionPersonEntity> FilterVersionPersons { get; set; } = new();

    /// <summary>
    /// Gets or sets the person DOIs.
    /// </summary>
    public ICollection<PersonDoiEntity> PersonDois { get; set; } = new List<PersonDoiEntity>();

    /// <summary>
    /// Gets or sets the canton bfs number, i.e. '17'.
    /// </summary>
    public short CantonBfs { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the version of person data is a householder.
    /// </summary>
    public bool IsHouseholder { get; set; }

    /// <summary>
    /// Gets or sets the residence building id (EGID).
    /// </summary>
    public int? ResidenceBuildingId { get; set; }

    /// <summary>
    /// Gets or sets the residence apartment/flat id (EWID).
    /// </summary>
    public int? ResidenceApartmentId { get; set; }
}
