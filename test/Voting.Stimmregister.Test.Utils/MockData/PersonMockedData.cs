// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Testing.Mocks;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Test.Utils.MockData;

/// <summary>
/// <para>Person mock data seeder. Use this class to add some static seeding data.</para>
/// <para>
/// Mock Data Set:
///  > 2 persons for the municipality 'St.Gallen' with municipality id 3203.
///  > 2 persons for the municipality 'Goldach' with municipality id 3213.
///  > 2 persons for the municipality 'Jona' with municipality id 3340.
/// </para>
/// If data is adjusted, the persons signature and <see cref="BfsIntegrityMockedData"/>
/// need adjustments too <seealso cref="ApplyDoisToPersons"/>.
/// </summary>
public static class PersonMockedData
{
    public const int MunicipalityIdStGallen = 3203;
    public const string MunicipalityNameStGallen = "St. Gallen";

    public const int MunicipalityIdGoldach = 3213;
    public const string MunicipalityNameGoldach = "Goldach";

    public const int MunicipalityIdJona = 3340;
    public const string MunicipalityNameJona = "Rapperswil-Jona";

    public const int MunicipalityIdSwissAbroad = 9170;
    public const string MunicipalityNameSwissAbroad = "Auslandschweizer";

    public const short CantonBfsStGallen = 17;

    public static PersonEntity Person_3203_StGallen_1
        => new()
        {
            Id = Guid.Parse("5457877c-b57b-46d8-926c-9d8b6d13705b"),
            RegisterId = Guid.Parse("d13ea9e8-ce71-4d8f-9121-05bba13033a8"),
            SourceSystemId = "04999781365",
            CreatedDate = MockedClock.GetDate(),
            MunicipalityId = MunicipalityIdStGallen,
            MunicipalityName = MunicipalityNameStGallen,
            DomainOfInfluenceId = 7354,
            FirstName = "Natalie",
            OfficialName = "Lavigne",
            AllianceName = "Lavigne-Meyerhans",
            CallName = "Nati",
            DateOfBirth = new DateOnly(1999, 12, 30),
            ContactAddressStreet = "Achslenstr.",
            ContactAddressHouseNumber = "3",
            ContactAddressDwellingNumber = "4",
            ContactAddressZipCode = "9016",
            ContactAddressTown = "St. Gallen",
            ContactCantonAbbreviation = "SG",
            ContactAddressCountryIdIso2 = "CH",
            Country = "CH",
            CountryNameShort = "Schweiz",
            RestrictedVotingAndElectionRightFederation = false,
            IsValid = true,
            IsLatest = true,
            DeletedDate = null,
            ResidenceCantonAbbreviation = "SG",
            ResidenceCountry = "CH",
            SourceSystemName = ImportSourceSystem.Loganto,
            CantonBfs = CantonBfsStGallen,
            VersionCount = 1,
        };

    public static PersonEntity Person_3203_StGallen_OldVersion_1
        => new()
        {
            Id = Guid.Parse("9f6a783f-8dd0-4fca-b339-397a3173d2a0"),
            RegisterId = Person_3203_StGallen_1.RegisterId,
            SourceSystemId = "07988896311",
            CreatedDate = MockedClock.GetDate(),
            MunicipalityId = MunicipalityIdStGallen,
            MunicipalityName = MunicipalityNameStGallen,
            DomainOfInfluenceId = 7354,
            FirstName = "Natalie",
            OfficialName = "Lavigne",
            DateOfBirth = new DateOnly(1999, 12, 30),
            ResidenceAddressStreet = "Achslenstr.",
            ResidenceAddressHouseNumber = "3",
            ResidenceAddressZipCode = "9016",
            ResidenceAddressTown = "St. Gallen",
            ResidenceCantonAbbreviation = "SG",
            ResidenceCountry = "CH",
            Country = "CH",
            CountryNameShort = "Schweiz",
            RestrictedVotingAndElectionRightFederation = false,
            IsValid = true,
            IsLatest = false,
            DeletedDate = null,
            ContactAddressCountryIdIso2 = "CH",
            SourceSystemName = ImportSourceSystem.Loganto,
            CantonBfs = CantonBfsStGallen,
            VersionCount = 0,
        };

    public static PersonEntity Person_3203_StGallen_2
        => new()
        {
            Id = Guid.Parse("079d124d-7085-4967-ae47-747aa1eca6b1"),
            RegisterId = Guid.Parse("cff2b057-decc-4905-ae09-679bea44a0fc"),
            SourceSystemId = "04465569871",
            CreatedDate = MockedClock.GetDate(),
            MunicipalityId = MunicipalityIdStGallen,
            MunicipalityName = MunicipalityNameStGallen,
            DomainOfInfluenceId = 330789,
            FirstName = "Meghana",
            OfficialName = "Bal",
            DateOfBirth = new DateOnly(1990, 11, 15),
            ResidenceAddressStreet = "Sturzeneggstr.",
            ResidenceAddressHouseNumber = "31",
            ResidenceAddressZipCode = "9016",
            ResidenceAddressTown = "St. Gallen",
            ResidenceCantonAbbreviation = "SG",
            ResidenceCountry = "CH",
            Country = "CH",
            CountryNameShort = "Schweiz",
            RestrictedVotingAndElectionRightFederation = false,
            IsValid = true,
            IsLatest = true,
            DeletedDate = null,
            ContactAddressCountryIdIso2 = "CH",
            SendVotingCardsToDomainOfInfluenceReturnAddress = true,
            SourceSystemName = ImportSourceSystem.Loganto,
            CantonBfs = CantonBfsStGallen,
        };

    public static PersonEntity Person_3203_StGallen_3_Foreigner
        => new()
        {
            Id = Guid.Parse("1bcf78fc-66df-42eb-9adf-157f884eb360"),
            RegisterId = Guid.Parse("e3db1f51-1115-45f9-9005-f89bbf14a5c8"),
            SourceSystemId = "05232361468",
            CreatedDate = MockedClock.GetDate(),
            MunicipalityId = MunicipalityIdStGallen,
            MunicipalityName = MunicipalityNameStGallen,
            Country = "DE",
            CountryNameShort = "Deutschland",
            DomainOfInfluenceId = 330789,
            FirstName = "Mustafa",
            OfficialName = "Berter",
            DateOfBirth = new DateOnly(2003, 11, 15),
            ResidenceAddressStreet = "Weststr.",
            ResidenceAddressHouseNumber = "12",
            ResidenceAddressZipCode = "9002",
            ResidenceAddressTown = "St. Gallen",
            ResidenceCantonAbbreviation = "SG",
            ResidenceCountry = "CH",
            RestrictedVotingAndElectionRightFederation = false,
            IsValid = true,
            IsLatest = true,
            DeletedDate = null,
            ResidencePermit = "02", // Aufenthalter
            ContactAddressCountryIdIso2 = "CH",
            SourceSystemName = ImportSourceSystem.Loganto,
            CantonBfs = CantonBfsStGallen,
        };

    public static PersonEntity Person_3203_StGallen_Foreigner_With_ResidenceValidDateRestriction_Expired
        => new()
        {
            Id = Guid.Parse("33cf78fc-66df-42eb-9adf-157f884eb360"),
            RegisterId = Guid.Parse("44db1f51-1115-45f9-9005-f89bbf14a5c8"),
            SourceSystemId = "04465679999",
            CreatedDate = MockedClock.GetDate(),
            MunicipalityId = MunicipalityIdStGallen,
            MunicipalityName = MunicipalityNameStGallen,
            Country = "DE",
            CountryNameShort = "Deutschland",
            DomainOfInfluenceId = 330789,
            FirstName = "Mustafa2",
            OfficialName = "Berter2",
            DateOfBirth = new DateOnly(1990, 11, 15),
            ResidenceAddressStreet = "Weststr.",
            ResidenceAddressHouseNumber = "12",
            ResidenceAddressZipCode = "9002",
            ResidenceAddressTown = "St. Gallen",
            ResidenceCantonAbbreviation = "SG",
            ResidenceCountry = "CH",
            RestrictedVotingAndElectionRightFederation = false,
            ResidencePermitValidFrom = new DateOnly(2000, 1, 1),
            ResidencePermitValidTill = new DateOnly(2001, 1, 1),
            IsValid = true,
            IsLatest = true,
            DeletedDate = null,
            ResidencePermit = "02", // Aufenthalter
            ContactAddressCountryIdIso2 = "CH",
            SourceSystemName = ImportSourceSystem.Loganto,
            CantonBfs = CantonBfsStGallen,
        };

    public static PersonEntity Person_3203_StGallen_Foreigner_With_ResidenceValidDateRestriction
        => new()
        {
            Id = Guid.Parse("55c578fc-66df-42eb-9adf-157f884eb450"),
            RegisterId = Guid.Parse("55db1f51-1115-45f9-9005-f89bbf14a512"),
            SourceSystemId = "04698799987",
            CreatedDate = MockedClock.GetDate(),
            MunicipalityId = MunicipalityIdStGallen,
            MunicipalityName = MunicipalityNameStGallen,
            Country = "DE",
            CountryNameShort = "Deutschland",
            DomainOfInfluenceId = 330789,
            FirstName = "Mustafa2",
            OfficialName = "Berter2",
            DateOfBirth = new DateOnly(1990, 11, 15),
            RestrictedVotingAndElectionRightFederation = false,
            ResidenceAddressStreet = "Weststr.",
            ResidenceAddressHouseNumber = "12",
            ResidenceAddressZipCode = "9002",
            ResidenceAddressTown = "St. Gallen",
            ResidenceCantonAbbreviation = "SG",
            ResidenceCountry = "CH",
            ResidencePermitValidFrom = MockedClock.NowDateOnly.AddDays(-20),
            ResidencePermitValidTill = MockedClock.NowDateOnly.AddDays(20),
            IsValid = true,
            IsLatest = true,
            DeletedDate = null,
            ResidencePermit = "02", // Aufenthalter
            ContactAddressCountryIdIso2 = "CH",
            SourceSystemName = ImportSourceSystem.Loganto,
            CantonBfs = CantonBfsStGallen,
        };

    public static PersonEntity Person_3203_StGallen_3
        => new()
        {
            Id = Guid.Parse("589a05e8-9548-4f64-96b2-8cce5fb55c2d"),
            RegisterId = Guid.Parse("c91497cd-21a6-4d0f-8748-3716cecb7d5f"),
            SourceSystemId = "03323256987",
            CreatedDate = MockedClock.GetDate(),
            Vn = 7564321686005,
            MunicipalityId = MunicipalityIdStGallen,
            MunicipalityName = MunicipalityNameStGallen,
            DomainOfInfluenceId = 330789,
            FirstName = "Carmen",
            OfficialName = "Ackermann",
            DateOfBirth = new DateOnly(1988, 8, 15),
            Religion = ReligionType.Catholic,
            ResidenceAddressStreet = "Bahnhofstr.",
            ResidenceAddressHouseNumber = "13",
            ResidenceAddressZipCode = "9000",
            ResidenceAddressTown = "St. Gallen",
            ResidenceCantonAbbreviation = "SG",
            ResidenceCountry = "CH",
            RestrictedVotingAndElectionRightFederation = false,
            Country = "CH",
            CountryNameShort = "Schweiz",
            IsValid = true,
            IsLatest = true,
            DeletedDate = null,
            EVoting = true,
            Sex = SexType.Female,
            TypeOfResidence = ResidenceType.HWS,
            ValidationErrors = "{\"FirstName\":[\"Validation errors message 1\", \"Validation errors message 2\", \"Validation errors message 3\"], \"OfficialName\":[\"The length must be greater than or equal to 6. You have entered 4 characters.\"]}",
            ContactCantonAbbreviation = "TG",
            ContactAddressCountryIdIso2 = "CH",
            SourceSystemName = ImportSourceSystem.Loganto,
            CantonBfs = CantonBfsStGallen,
        };

    public static PersonEntity Person_3203_StGallen_4_Without_Residence_And_Contact_Address
        => new()
        {
            Id = Guid.Parse("26372c45-11d2-49fd-9c04-b55d167bebd2"),
            RegisterId = Guid.Parse("4c140faf-7a7d-4ede-9d37-cf508e1d2cd1"),
            SourceSystemId = "00004578952",
            CreatedDate = MockedClock.GetDate(),
            Vn = 7564895123447,
            MunicipalityId = MunicipalityIdStGallen,
            MunicipalityName = MunicipalityNameStGallen,
            DomainOfInfluenceId = 7354,
            FirstName = "Luca",
            OfficialName = "Neilson",
            DateOfBirth = new DateOnly(1960, 2, 5),
            Religion = ReligionType.Evangelic,
            RestrictedVotingAndElectionRightFederation = false,
            Country = "CH",
            CountryNameShort = "Schweiz",
            IsValid = false,
            IsLatest = true,
            DeletedDate = null,
            EVoting = false,
            Sex = SexType.Male,
            TypeOfResidence = ResidenceType.HWS,
            ValidationErrors = "{\"Person\":[\"Validation errors message 1\", \"Es muss mindestens eine Wohn- oder Kontaktadresse gesetzt sein.\"]}",
            SourceSystemName = ImportSourceSystem.Loganto,
            CantonBfs = CantonBfsStGallen,
        };

    public static PersonEntity Person_3203_StGallen_5
        => new()
        {
            Id = Guid.Parse("93735eae-92a2-47e3-846d-fc50350b1c16"),
            RegisterId = Guid.Parse("43a45d10-9c2f-4f48-9e30-932fbcf03a00"),
            SourceSystemId = "03658945611",
            CreatedDate = MockedClock.GetDate(),
            Vn = 7563521987424,
            MunicipalityId = MunicipalityIdStGallen,
            MunicipalityName = MunicipalityNameStGallen,
            DomainOfInfluenceId = 652348,
            FirstName = "Katharina",
            OfficialName = "Jackson",
            AllianceName = "Jackson-Mercury",
            AliasName = "Catherina",
            CallName = "Cathy",
            OriginalName = "Katherina",
            OtherName = "Michaela",
            DateOfBirth = new DateOnly(1991, 2, 22),
            DateOfBirthAdjusted = true,
            Religion = ReligionType.Evangelic,
            ResidenceAddressStreet = "Weiherweg",
            ResidenceAddressHouseNumber = "27",
            ResidenceAddressZipCode = "9015",
            ResidenceAddressTown = "Winkeln SG",
            ResidenceCountry = "CH",
            ResidenceCantonAbbreviation = "SG",
            ResidenceAddressDwellingNumber = "27",
            ResidenceAddressExtensionLine1 = "Madame",
            ResidenceAddressExtensionLine2 = "Jackson",
            ResidenceAddressPostOfficeBoxText = "Postfach",
            ResidencePermit = "30",
            ResidencePermitValidFrom = new DateOnly(2001, 2, 22),
            ResidencePermitValidTill = new DateOnly(2002, 2, 22),
            ResidenceEntryDate = new DateOnly(2001, 2, 22),
            TypeOfResidence = ResidenceType.HWS,
            RestrictedVotingAndElectionRightFederation = false,
            Country = "CH",
            CountryNameShort = "Schweiz",
            IsValid = false,
            IsLatest = true,
            DeletedDate = null,
            EVoting = true,
            Sex = SexType.Female,
            ValidationErrors = "{\"FirstName\":[\"Validation errors message 1\", \"Validation errors message 2\", \"Validation errors message 3\"], \"OfficialName\":[\"The length must be greater than or equal to 6. You have entered 4 characters.\"]}",
            SourceSystemName = ImportSourceSystem.Loganto,
            ContactAddressLine1 = "Frau",
            ContactAddressLine2 = "Katharina Jackson",
            ContactAddressLine3 = "Weiherweg 27",
            ContactAddressLine4 = "Postfach 1",
            ContactAddressLine5 = "9015 Winkeln SG",
            ContactAddressLine6 = "CH",
            ContactAddressLine7 = "Schweiz",
            ContactAddressLocality = "Winklen SG",
            ContactAddressStreet = "Weiherweg",
            ContactAddressHouseNumber = "27",
            ContactAddressTown = "Winkeln SG",
            ContactCantonAbbreviation = "SG",
            ContactAddressDwellingNumber = "27",
            ContactAddressExtensionLine1 = "Madame",
            ContactAddressExtensionLine2 = "Jackson",
            ContactAddressZipCode = "9015",
            ContactAddressPostOfficeBoxText = "Postfach",
            ContactAddressPostOfficeBoxNumber = 1,
            ContactAddressCountryIdIso2 = "CH",
            MoveInUnknown = false,
            MoveInMunicipalityName = "Basel",
            MoveInArrivalDate = new DateOnly(2001, 2, 22),
            MoveInComesFrom = "Basel",
            MoveInCantonAbbreviation = "BS",
            MoveInCountryNameShort = "Schweiz",
            CantonBfs = CantonBfsStGallen,
        };

    public static PersonEntity Person_3203_StGallen_Duplicate_Of_Goldach_1
        => new()
        {
            Id = Guid.Parse("73682f27-6ff2-449d-923a-e912e18c8e41"),
            RegisterId = Guid.Parse("c7e3855c-7357-4b3b-9520-3498f1b82800"),
            SourceSystemId = "04977989856",
            CreatedDate = MockedClock.GetDate().AddDays(-1),
            ModifiedDate = MockedClock.GetDate().AddDays(-1),
            Vn = 7563149003309,
            MunicipalityId = MunicipalityIdStGallen,
            MunicipalityName = MunicipalityNameStGallen,
            DomainOfInfluenceId = 54468,
            FirstName = "Mikael",
            OfficialName = "Ross",
            DateOfBirth = new DateOnly(1959, 02, 01),
            ResidenceAddressStreet = "Bahnhofstrasse",
            ResidenceAddressHouseNumber = "20a",
            ResidenceAddressZipCode = "9000",
            ResidenceAddressTown = "St.Gallen",
            ResidenceCantonAbbreviation = string.Empty,
            ResidenceCountry = "CH",
            Country = "CH",
            CountryNameShort = "Schweiz",
            RestrictedVotingAndElectionRightFederation = false,
            IsValid = true,
            IsLatest = true,
            DeletedDate = null,
            ContactCantonAbbreviation = "ZH",
            ContactAddressCountryIdIso2 = "CH",
            SourceSystemName = ImportSourceSystem.Loganto,
            CantonBfs = CantonBfsStGallen,
        };

    public static PersonEntity Person_3213_Goldach_1
        => new()
        {
            Id = Guid.Parse("118f61a6-7e85-49d4-ba10-237dcb6fa221"),
            RegisterId = Guid.Parse("129813d0-8077-4f5e-995a-7ca7e0080a5b"),
            SourceSystemId = "04977989856",
            CreatedDate = MockedClock.GetDate(),
            ModifiedDate = MockedClock.GetDate(),
            Vn = 7563149003309,
            MunicipalityId = MunicipalityIdGoldach,
            MunicipalityName = MunicipalityNameGoldach,
            DomainOfInfluenceId = 858,
            FirstName = "Mikael",
            OfficialName = "Ross",
            DateOfBirth = new DateOnly(1959, 02, 01),
            ResidenceAddressStreet = "Libellenstr.",
            ResidenceAddressHouseNumber = "6",
            ResidenceAddressZipCode = "9016",
            ResidenceAddressTown = "Goldach",
            ResidenceCantonAbbreviation = string.Empty,
            ResidenceCountry = "CH",
            Country = "CH",
            CountryNameShort = "Schweiz",
            RestrictedVotingAndElectionRightFederation = false,
            IsValid = true,
            IsLatest = true,
            DeletedDate = null,
            ContactCantonAbbreviation = "ZH",
            ContactAddressCountryIdIso2 = "CH",
            SourceSystemName = ImportSourceSystem.Loganto,
            CantonBfs = CantonBfsStGallen,
        };

    public static PersonEntity Person_3213_Goldach_Swiss_Abroad
        => new()
        {
            Id = Guid.Parse("56a7200f-e210-443e-9895-a601a5e9ec47"),
            RegisterId = Guid.Parse("b46b61ff-410d-4eaf-951e-697176ed9bca"),
            SourceSystemId = "09896553658",
            TypeOfResidence = ResidenceType.HWS,
            CreatedDate = MockedClock.GetDate(),
            Vn = 7564788944060,
            MunicipalityId = MunicipalityIdGoldach,
            MunicipalityName = MunicipalityNameGoldach,
            DomainOfInfluenceId = 858,
            FirstName = "Zoe",
            OfficialName = "Meyer",
            DateOfBirth = new DateOnly(1955, 01, 07),
            ContactAddressStreet = "Tirolerplatz",
            ContactAddressHouseNumber = "1",
            ContactAddressZipCode = "5555",
            ContactAddressTown = "Wien",
            ContactAddressCountryIdIso2 = "CH",
            ContactCantonAbbreviation = "ZH",
            ResidenceCantonAbbreviation = string.Empty,
            ResidenceCountry = "AT",
            IsSwissAbroad = true,
            EVoting = true,
            Country = "CH",
            CountryNameShort = "Schweiz",
            RestrictedVotingAndElectionRightFederation = false,
            IsValid = true,
            IsLatest = true,
            DeletedDate = null,
            SourceSystemName = ImportSourceSystem.Loganto,
            CantonBfs = CantonBfsStGallen,
        };

    public static PersonEntity Person_3213_Goldach_eVoter
        => new()
        {
            Id = Guid.Parse("37595c68-3535-419d-8f18-6e2f6a7d9ede"),
            RegisterId = Guid.Parse("f91f101e-1528-4f4f-8fba-0a480ce81cb1"),
            SourceSystemId = "09645231479",
            CreatedDate = MockedClock.GetDate(),
            MunicipalityId = MunicipalityIdGoldach,
            MunicipalityName = MunicipalityNameGoldach,
            DomainOfInfluenceId = 858,
            FirstName = "Gabriela",
            OfficialName = "Gerber",
            DateOfBirth = new DateOnly(1962, 05, 02),
            ResidenceAddressStreet = "Hafenstr.",
            ResidenceAddressHouseNumber = "12",
            ResidenceAddressZipCode = "9016",
            ResidenceAddressTown = "Goldach",
            ResidenceCantonAbbreviation = string.Empty,
            ResidenceCountry = "CH",
            Country = "CH",
            CountryNameShort = "Schweiz",
            RestrictedVotingAndElectionRightFederation = false,
            IsValid = true,
            IsLatest = true,
            DeletedDate = null,
            EVoting = true,
            ContactCantonAbbreviation = "SG",
            ContactAddressCountryIdIso2 = "CH",
            SourceSystemName = ImportSourceSystem.Loganto,
            CantonBfs = CantonBfsStGallen,
        };

    public static PersonEntity Person_3340_Jona_1
        => new()
        {
            Id = Guid.Parse("b180a66c-7d19-423b-9060-3f81bedeca02"),
            RegisterId = Guid.Parse("64903a2e-d721-4607-9b7a-44db4d175511"),
            SourceSystemId = "07979565211",
            CreatedDate = MockedClock.GetDate(),
            MunicipalityId = MunicipalityIdJona,
            MunicipalityName = MunicipalityNameJona,
            DomainOfInfluenceId = 103671,
            FirstName = "Franklin",
            OfficialName = "Morin",
            DateOfBirth = new DateOnly(2022, 03, 22),
            ResidenceAddressStreet = "Stampfstr.",
            ResidenceAddressHouseNumber = "23",
            ResidenceAddressZipCode = "9016",
            ResidenceAddressTown = "Goldach",
            ResidenceCantonAbbreviation = "AG",
            ResidenceCountry = "CH",
            Country = "CH",
            CountryNameShort = "Schweiz",
            RestrictedVotingAndElectionRightFederation = false,
            IsValid = true,
            IsLatest = true,
            DeletedDate = null,
            ContactCantonAbbreviation = string.Empty,
            ContactAddressCountryIdIso2 = "CH",
            SourceSystemName = ImportSourceSystem.Loganto,
            CantonBfs = CantonBfsStGallen,
        };

    public static PersonEntity Person_3340_Jona_2
        => new()
        {
            Id = Guid.Parse("b8727d7d-ae58-4c64-8deb-4b6bb20bd92a"),
            RegisterId = Guid.Parse("db4b128e-ad35-4abc-84d2-639c406e5824"),
            SourceSystemId = "04412346589",
            CreatedDate = MockedClock.GetDate(),
            MunicipalityId = MunicipalityIdJona,
            MunicipalityName = MunicipalityNameJona,
            DomainOfInfluenceId = 387108,
            FirstName = "Alexandra",
            OfficialName = "Wood",
            DateOfBirth = new DateOnly(1986, 06, 14),
            ResidenceAddressStreet = "Meienbergstr.",
            ResidenceAddressHouseNumber = "9",
            ResidenceAddressZipCode = "9016",
            ResidenceAddressTown = "Goldach",
            ResidenceCantonAbbreviation = "SG",
            ResidenceCountry = "CH",
            Country = "CH",
            CountryNameShort = "Schweiz",
            RestrictedVotingAndElectionRightFederation = false,
            IsValid = true,
            IsLatest = true,
            DeletedDate = null,
            ContactAddressCountryIdIso2 = "CH",
            SourceSystemName = ImportSourceSystem.Loganto,
            CantonBfs = CantonBfsStGallen,
        };

    public static PersonEntity Person_3213_Goldach_2
        => new()
        {
            Id = Guid.Parse("67de7b86-71a8-43d3-b5e4-a17fa008d41a"),
            RegisterId = Guid.Parse("ea041b02-57cb-431b-bf82-9c1619bb9522"),
            SourceSystemId = "04795632489",
            CreatedDate = MockedClock.GetDate(),
            MunicipalityId = MunicipalityIdGoldach,
            MunicipalityName = MunicipalityNameGoldach,
            DomainOfInfluenceId = 375259,
            FirstName = "Blake",
            OfficialName = "Jean-Baptiste",
            DateOfBirth = new DateOnly(1978, 01, 11),
            ResidenceAddressStreet = "Hohrainweg",
            ResidenceAddressHouseNumber = "5",
            ResidenceAddressZipCode = "9016",
            ResidenceAddressTown = "Goldach",
            ResidenceCantonAbbreviation = "SG",
            ResidenceCountry = "CH",
            Country = "CH",
            CountryNameShort = "Schweiz",
            RestrictedVotingAndElectionRightFederation = false,
            IsValid = true,
            IsLatest = true,
            DeletedDate = null,
            ContactAddressCountryIdIso2 = "CH",
            SourceSystemName = ImportSourceSystem.Loganto,
            CantonBfs = CantonBfsStGallen,
        };

    public static PersonEntity PersonForValidations
        => new()
        {
            Id = Guid.Parse("67de7b86-71a8-43d3-b5e4-a17fa008d41a"),
            RegisterId = Guid.Parse("ea041b02-57cb-431b-bf82-9c1619bb9522"),
            SourceSystemId = "04875312457",
            CreatedDate = MockedClock.GetDate(),
            MunicipalityId = MunicipalityIdGoldach,
            MunicipalityName = MunicipalityNameGoldach,
            DomainOfInfluenceId = 375259,
            FirstName = "Blake",
            OfficialName = "Jean-Baptiste",
            DateOfBirth = new DateOnly(1978, 01, 11),
            ResidenceAddressStreet = "Hohrainweg",
            ResidenceAddressHouseNumber = "5",
            ResidenceAddressZipCode = "9016",
            ResidenceAddressTown = "Goldach",
            ResidenceCountry = "CH",
            Country = "CH",
            CountryNameShort = "Schweiz",
            IsValid = true,
            IsLatest = true,
            DeletedDate = null,
            Sex = SexType.Male,
            TypeOfResidence = ResidenceType.HWS,
            ContactAddressZipCode = "9016",
            ContactAddressCountryIdIso2 = "CH",
            MoveInArrivalDate = new DateOnly(2021, 1, 1),
            RestrictedVotingAndElectionRightFederation = false,
            ResidenceCantonAbbreviation = "SG",
            PersonDois = new HashSet<PersonDoiEntity>()
            {
                new() { Identifier = "SG", Canton = "SG", Name = "Goldach", DomainOfInfluenceType = DomainOfInfluenceType.Og },
            },
            SourceSystemName = ImportSourceSystem.Loganto,
            CantonBfs = CantonBfsStGallen,
        };

    public static PersonEntity Person_3203_StGallen_Deleted_1
       => new()
       {
           Id = Guid.Parse("bf3fd8ad-b603-44a8-a778-e994e1df4d61"),
           RegisterId = Guid.Parse("fdc6cdb4-58b0-4e07-97cd-c52437d26aff"),
           SourceSystemId = "04678874221",
           CreatedDate = MockedClock.GetDate(),
           MunicipalityId = MunicipalityIdStGallen,
           MunicipalityName = MunicipalityNameStGallen,
           DomainOfInfluenceId = 7354,
           FirstName = "Natalie",
           OfficialName = "Lavigne",
           DateOfBirth = new DateOnly(1999, 12, 30),
           ResidenceAddressStreet = "Achslenstr.",
           ResidenceAddressHouseNumber = "3",
           ResidenceAddressZipCode = "9016",
           ResidenceAddressTown = "St. Gallen",
           ResidenceCantonAbbreviation = "SG",
           ResidenceCountry = "CH",
           Country = "CH",
           CountryNameShort = "Schweiz",
           RestrictedVotingAndElectionRightFederation = false,
           IsValid = true,
           IsLatest = true,
           IsDeleted = true,
           DeletedDate = MockedClock.GetDate(),
           ContactAddressCountryIdIso2 = "CH",
           SourceSystemName = ImportSourceSystem.Loganto,
           CantonBfs = CantonBfsStGallen,
       };

    public static PersonEntity Person_3203_StGallen_Under18 =>
        new()
        {
            Id = Guid.Parse("bf3fd8ad-b603-44a2-a778-e994eedfdd61"),
            RegisterId = Guid.Parse("fdc6cdb4-58b0-4e07-97cd-c52437d2eeff"),
            SourceSystemId = "09876256412",
            CreatedDate = MockedClock.GetDate(),
            MunicipalityId = MunicipalityIdStGallen,
            MunicipalityName = MunicipalityNameStGallen,
            DomainOfInfluenceId = 7354,
            FirstName = "Natalie",
            OfficialName = "Lavigne",
            DateOfBirth = new DateOnly(MockedClock.UtcNowDate.Year - 5, 12, 30),
            ResidenceAddressStreet = "Achslenstr.",
            ResidenceAddressHouseNumber = "3",
            ResidenceAddressZipCode = "9016",
            ResidenceAddressTown = "St. Gallen",
            ResidenceCantonAbbreviation = "SG",
            ResidenceCountry = "CH",
            Country = "CH",
            CountryNameShort = "Schweiz",
            IsValid = true,
            IsLatest = true,
            RestrictedVotingAndElectionRightFederation = false,
            ContactAddressCountryIdIso2 = "CH",
            SourceSystemName = ImportSourceSystem.Loganto,
            CantonBfs = CantonBfsStGallen,
        };

    public static PersonEntity Person_3203_StGallen_Invalid
        => new()
        {
            Id = Guid.Parse("12359c10-3767-4045-b310-d0d12580e08e"),
            RegisterId = Guid.Parse("2a489f75-9c05-4869-a4c2-5a17e3959a46"),
            CreatedDate = MockedClock.GetDate(),
            MunicipalityId = MunicipalityIdStGallen,
            Vn = 7567298579364,
            DomainOfInfluenceId = 7354,
            FirstName = "Frauke",
            OfficialName = "Grolham",
            DateOfBirth = new DateOnly(1991, 11, 10),
            ResidenceAddressStreet = "Axstr.",
            ResidenceAddressHouseNumber = "7b",
            ResidenceAddressZipCode = "9016",
            ResidenceAddressTown = "St. Gallen",
            ResidenceCantonAbbreviation = "SG",
            ResidenceCountry = "CH",
            Country = "CH",
            CountryNameShort = "Schweiz",
            RestrictedVotingAndElectionRightFederation = false,
            IsValid = false,
            IsLatest = true,
            DeletedDate = null,
            ContactAddressCountryIdIso2 = "CH",
            SourceSystemName = ImportSourceSystem.Loganto,
            CantonBfs = CantonBfsStGallen,
        };

    public static PersonEntity Person_9170_Auslandschweizer_1
        => new()
        {
            Id = Guid.Parse("ef3fd8ad-b603-44a8-a7cc-e994e1df4222"),
            RegisterId = Guid.Parse("faa6cdb4-58b0-4e07-97cd-c52227d26a33"),
            SourceSystemId = "02347558965",
            CreatedDate = MockedClock.GetDate(),
            ModifiedDate = MockedClock.GetDate(),
            MunicipalityId = MunicipalityIdSwissAbroad,
            MunicipalityName = MunicipalityNameSwissAbroad,
            IsSwissAbroad = true,
            DomainOfInfluenceId = 7354,
            FirstName = "Natasha",
            OfficialName = "Berweger",
            DateOfBirth = new DateOnly(1997, 12, 30),
            RestrictedVotingAndElectionRightFederation = false,
            ResidenceCountry = "DE",
            IsValid = true,
            IsLatest = true,
            Country = "CH",
            CountryNameShort = "Schweiz",
            Sex = SexType.Female,
            DateOfBirthAdjusted = true,
            ContactAddressExtensionLine1 = "Appartement 2",
            ContactAddressExtensionLine2 = "Stockwerk 10",
            ContactAddressStreet = "Petrusstr.",
            ContactAddressHouseNumber = "3",
            ContactAddressZipCode = "90316",
            ContactAddressTown = "Berlin",
            ContactAddressLine1 = "Petrusstr",
            ContactAddressLine2 = "857",
            ContactAddressLine3 = "Sins",
            ContactAddressLine4 = string.Empty,
            ContactAddressLine5 = string.Empty,
            ContactAddressLine6 = "Berlin ",
            ContactAddressLine7 = "EH3 9QN ",
            ContactAddressCountryIdIso2 = "DE",
            Religion = ReligionType.Unknown,
            MoveInArrivalDate = DateOnly.Parse("2020-01-10"),
            MoveInComesFrom = "Wartau",
            TypeOfResidence = ResidenceType.Undefined,
            ResidenceCantonAbbreviation = "SG",
            SourceSystemName = ImportSourceSystem.Cobra,
            CantonBfs = CantonBfsStGallen,
            EVoting = true,
        };

    public static PersonEntity Person_9170_Auslandschweizer_2
        => new()
        {
            Id = Guid.Parse("e44fd8ad-b603-44a8-a7cc-e994e1df4222"),
            RegisterId = Guid.Parse("22a6cdb4-58b0-4e07-97cd-122227d26a33"),
            SourceSystemId = "01234975689",
            CreatedDate = MockedClock.GetDate(),
            ModifiedDate = MockedClock.GetDate(),
            MunicipalityId = MunicipalityIdSwissAbroad,
            MunicipalityName = MunicipalityNameSwissAbroad,
            DomainOfInfluenceId = 7354,
            FirstName = "Peter",
            OfficialName = "Miro",
            DateOfBirth = new DateOnly(1994, 6, 7),
            ResidenceAddressStreet = "Big Ben Avenue",
            ResidenceAddressHouseNumber = "5",
            ResidenceAddressZipCode = "9031",
            ResidenceAddressTown = "London",
            ResidenceCountry = "GB",
            RestrictedVotingAndElectionRightFederation = false,
            IsValid = true,
            IsLatest = true,
            Country = "CH",
            CountryNameShort = "Schweiz",
            Sex = SexType.Male,
            DateOfBirthAdjusted = true,
            ContactAddressStreet = "Big Ben Avenue",
            ContactAddressHouseNumber = "5",
            ContactAddressZipCode = "9031",
            ContactAddressTown = "London",
            ContactAddressLine1 = "Big Ben Avenue",
            ContactAddressLine2 = "5",
            ContactAddressLine3 = "Sins",
            ContactAddressLine4 = string.Empty,
            ContactAddressLine5 = string.Empty,
            ContactAddressLine6 = "Edinburgh ",
            ContactAddressLine7 = "EH3 9QN ",
            ContactAddressCountryIdIso2 = "GB",
            Religion = ReligionType.Unknown,
            MoveInArrivalDate = DateOnly.Parse("2019-01-10"),
            MoveInComesFrom = "Wittenbach",
            TypeOfResidence = ResidenceType.HWS,
            ResidenceCantonAbbreviation = "SG",
            SourceSystemName = ImportSourceSystem.Cobra,
            CantonBfs = CantonBfsStGallen,
            EVoting = true,
        };

    public static PersonEntity Person_9170_Auslaender
        => new()
        {
            Id = Guid.Parse("e44f11ad-b603-44a8-a7cc-e994e1df4222"),
            RegisterId = Guid.Parse("22a6cd88-58b0-4e07-97cd-122227d26a33"),
            SourceSystemId = "09753456785",
            CreatedDate = MockedClock.GetDate(),
            ModifiedDate = MockedClock.GetDate(),
            MunicipalityId = MunicipalityIdSwissAbroad,
            MunicipalityName = MunicipalityNameSwissAbroad,
            DomainOfInfluenceId = 7354,
            FirstName = "Pierre",
            OfficialName = "Miro",
            DateOfBirth = new DateOnly(1994, 6, 7),
            ResidenceAddressStreet = "Big Ben Avenue",
            ResidenceAddressHouseNumber = "5",
            ResidenceAddressZipCode = "9017",
            ResidenceAddressTown = "London",
            ResidenceCountry = "GB",
            RestrictedVotingAndElectionRightFederation = false,
            IsValid = true,
            IsLatest = true,
            Country = "DE",
            CountryNameShort = "Deutschland",
            Sex = SexType.Male,
            DateOfBirthAdjusted = true,
            ContactAddressStreet = "Big Ben Avenue",
            ContactAddressHouseNumber = "5",
            ContactAddressZipCode = "9017",
            ContactAddressTown = "London",
            ContactAddressLine1 = "Big Ben Avenue",
            ContactAddressLine2 = "5",
            ContactAddressLine3 = "Sins",
            ContactAddressLine4 = string.Empty,
            ContactAddressLine5 = string.Empty,
            ContactAddressLine6 = "Edinburgh ",
            ContactAddressLine7 = "EH3 9QN ",
            ContactAddressCountryIdIso2 = "GB",
            Religion = ReligionType.Unknown,
            MoveInArrivalDate = DateOnly.Parse("2021-01-10"),
            MoveInComesFrom = "Wittenbach",
            TypeOfResidence = ResidenceType.HWS,
            ResidenceCantonAbbreviation = "SG",
            SourceSystemName = ImportSourceSystem.Cobra,
            CantonBfs = CantonBfsStGallen,
        };

    public static PersonEntity Person_9170_Auslandschweizer_Under18 =>
        new()
        {
            Id = Guid.Parse("555fd8ad-b603-44a8-a7cc-e994e1df4222"),
            RegisterId = Guid.Parse("6666cdb4-58b0-4e07-97cd-122227d26a33"),
            SourceSystemId = "07586348975",
            CreatedDate = MockedClock.GetDate(),
            ModifiedDate = MockedClock.GetDate(),
            MunicipalityId = MunicipalityIdSwissAbroad,
            MunicipalityName = MunicipalityNameSwissAbroad,
            DomainOfInfluenceId = 7354,
            FirstName = "Pierre",
            OfficialName = "Klein",
            DateOfBirth = new DateOnly(MockedClock.UtcNowDate.Year - 12, 6, 7),
            ResidenceAddressStreet = "Big Ben Avenue",
            ResidenceAddressHouseNumber = "5",
            ResidenceAddressZipCode = "3317",
            ResidenceAddressTown = "London",
            ResidenceCantonAbbreviation = "SG",
            ResidenceCountry = "GB",
            RestrictedVotingAndElectionRightFederation = false,
            IsValid = true,
            IsLatest = true,
            Country = "CH",
            CountryNameShort = "Schweiz",
            Sex = SexType.Male,
            DateOfBirthAdjusted = true,
            ContactAddressStreet = "Big Ben Avenue",
            ContactAddressHouseNumber = "5",
            ContactAddressZipCode = "1317",
            ContactAddressTown = "London",
            ContactAddressLine1 = "Big Ben Avenue",
            ContactAddressLine2 = "5",
            ContactAddressLine3 = "Sins",
            ContactAddressLine4 = string.Empty,
            ContactAddressLine5 = string.Empty,
            ContactAddressLine6 = "Edinburgh ",
            ContactAddressLine7 = "EH3 9QN ",
            ContactAddressCountryIdIso2 = "GB",
            Religion = ReligionType.Unknown,
            MoveInArrivalDate = DateOnly.Parse("2021-01-10"),
            MoveInComesFrom = "Wittenbach",
            TypeOfResidence = ResidenceType.HWS,
            SourceSystemName = ImportSourceSystem.Cobra,
            CantonBfs = CantonBfsStGallen,
            EVoting = true,
        };

    public static PersonEntity Person_9170_RestrictedVoting =>
        new()
        {
            Id = Guid.Parse("ff4fd8ad-b603-44a8-a7cc-e994e1df4211"),
            RegisterId = Guid.Parse("11a6ddb4-58b0-4e07-97cd-122227d26a44"),
            SourceSystemId = "04589765235",
            CreatedDate = MockedClock.GetDate(),
            ModifiedDate = MockedClock.GetDate(),
            MunicipalityId = MunicipalityIdSwissAbroad,
            MunicipalityName = MunicipalityNameSwissAbroad,
            DomainOfInfluenceId = 7354,
            FirstName = "Restricted",
            OfficialName = "Voting",
            DateOfBirth = new DateOnly(2000, 6, 7),
            ResidenceAddressStreet = "Champs Elisee",
            ResidenceAddressHouseNumber = "5",
            ResidenceAddressZipCode = "9031",
            ResidenceAddressTown = "Paris",
            ResidenceCountry = "FR",
            IsValid = true,
            IsLatest = true,
            Country = "CH",
            CountryNameShort = "Schweiz",
            Sex = SexType.Male,
            DateOfBirthAdjusted = true,
            ContactAddressStreet = "Champs Elisee",
            ContactAddressHouseNumber = "5",
            ContactAddressZipCode = "4643",
            ContactAddressTown = "Paris",
            ContactAddressCountryIdIso2 = "FR",
            Religion = ReligionType.Catholic,
            MoveInArrivalDate = DateOnly.Parse("2021-01-10"),
            MoveInComesFrom = "St. Gallen",
            TypeOfResidence = ResidenceType.NWS,
            RestrictedVotingAndElectionRightFederation = true,
            ResidenceCantonAbbreviation = "SG",
            SourceSystemName = ImportSourceSystem.Cobra,
            CantonBfs = CantonBfsStGallen,
            EVoting = true,
        };

    public static IEnumerable<PersonEntity> All
    {
        get
        {
            yield return Person_3203_StGallen_1;
            yield return Person_3203_StGallen_OldVersion_1;
            yield return Person_3203_StGallen_2;
            yield return Person_3203_StGallen_Foreigner_With_ResidenceValidDateRestriction;
            yield return Person_3203_StGallen_Foreigner_With_ResidenceValidDateRestriction_Expired;
            yield return Person_3203_StGallen_3_Foreigner;
            yield return Person_3203_StGallen_Deleted_1;
            yield return Person_3203_StGallen_3;
            yield return Person_3203_StGallen_4_Without_Residence_And_Contact_Address;
            yield return Person_3203_StGallen_5;
            yield return Person_3203_StGallen_Under18;
            yield return Person_3203_StGallen_Invalid;
            yield return Person_3203_StGallen_Duplicate_Of_Goldach_1;
            yield return Person_3213_Goldach_1;
            yield return Person_3213_Goldach_2;
            yield return Person_3213_Goldach_eVoter;
            yield return Person_3213_Goldach_Swiss_Abroad;
            yield return Person_3340_Jona_1;
            yield return Person_3340_Jona_2;
            yield return Person_9170_Auslandschweizer_1;
            yield return Person_9170_Auslandschweizer_2;
            yield return Person_9170_Auslandschweizer_Under18;
            yield return Person_9170_Auslaender;
            yield return Person_9170_RestrictedVoting;
        }
    }

    /// <summary>
    /// Seeds mock data defined in this task.
    /// </summary>
    /// <param name="runScoped">The run scoped action.</param>
    /// <returns>A <see cref="Task"/> from the run scoped action where data is seeded async.</returns>
    public static Task Seed(Func<Func<IServiceProvider, Task>, Task> runScoped)
    {
        return runScoped(async sp =>
        {
            var db = sp.GetRequiredService<IDataContext>();

            var all = All.ToList();
            ApplyDoisToPersons(all);

            db.Persons.AddRange(all);
            await db.SaveChangesAsync();
        });
    }

    private static void ApplyDoisToPersons(IReadOnlyCollection<PersonEntity> allPersons)
    {
        var personDois = PersonDoiMockedData.All.ToList();
        var doisPerPerson = personDois.GroupBy(x => x.PersonId).ToDictionary(x => x.Key, x => x.ToList());
        foreach (var person in allPersons)
        {
            person.PersonDois = doisPerPerson.GetValueOrDefault(person.Id) ?? new List<PersonDoiEntity>();
        }
    }
}
