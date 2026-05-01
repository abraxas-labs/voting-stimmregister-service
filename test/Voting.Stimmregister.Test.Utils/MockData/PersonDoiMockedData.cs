// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Test.Utils.MockData;

/// <summary>
/// Mock data set for person DOI.
/// If anything is adjusted, the signature of the <see cref="PersonMockedData"/>
/// and <see cref="BfsIntegrityMockedData"/> need updates.
/// </summary>
public static class PersonDoiMockedData
{
    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_1_PoliticalCircle_Osten
        => new()
        {
            Id = Guid.Parse("e7cc3ada-ec2d-42cd-be2b-639decd6082b"),
            PersonId = PersonMockedData.Person_3203_StGallen_1.Id,
            Canton = "SG",
            Identifier = "O",
            Name = "Osten",
            DomainOfInfluenceType = DomainOfInfluenceType.Sk,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_1_ConfederationCircle
        => new()
        {
            Id = Guid.Parse("d3cf06ca-ce2d-11ed-afa1-0242ac120002"),
            PersonId = PersonMockedData.Person_3203_StGallen_1.Id,
            Canton = "SG",
            Identifier = "1",
            Name = "Kanton St.Gallen eidgenössisch",
            DomainOfInfluenceType = DomainOfInfluenceType.Ch,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_1_CantonCircle1
        => new()
        {
            Id = Guid.Parse("5bb21eae-ce30-11ed-afa1-0242ac120002"),
            PersonId = PersonMockedData.Person_3203_StGallen_1.Id,
            Canton = "SG",
            Identifier = "17",
            Name = "Kanton St.Gallen kantonal",
            DomainOfInfluenceType = DomainOfInfluenceType.Ct,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_1_DistrictCircle1
        => new()
        {
            Id = Guid.Parse("8fd33d30-ce30-11ed-afa1-0242ac120002"),
            PersonId = PersonMockedData.Person_3203_StGallen_1.Id,
            Canton = "SG",
            Identifier = string.Empty,
            Name = "Gerichtskreis St.Gallen",
            DomainOfInfluenceType = DomainOfInfluenceType.Bz,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_1_DistrictCircle2
        => new()
        {
            Id = Guid.Parse("7e82c72f-fb0e-4142-8b71-aec540ee4887"),
            PersonId = PersonMockedData.Person_3203_StGallen_1.Id,
            Canton = "SG",
            Identifier = string.Empty,
            Name = "Wahlkreis St.Gallen",
            DomainOfInfluenceType = DomainOfInfluenceType.Bz,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_1_MunicipalityCircle
        => new()
        {
            Id = Guid.Parse("2965380e-b5b1-4b7d-891b-0862749a40c0"),
            PersonId = PersonMockedData.Person_3203_StGallen_1.Id,
            Canton = "SG",
            Identifier = "3203",
            Name = "St.Gallen",
            DomainOfInfluenceType = DomainOfInfluenceType.Mu,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_1_Origin1
        => new()
        {
            Id = Guid.Parse("8671a470-5033-49f2-991a-69c397defa5b"),
            PersonId = PersonMockedData.Person_3203_StGallen_1.Id,
            Canton = "AR",
            Identifier = string.Empty,
            Name = "Teufen",
            DomainOfInfluenceType = DomainOfInfluenceType.Og,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_OldVersion_1_PoliticalCircle_Osten
        => new()
        {
            Id = Guid.Parse("28e9f6d8-00b4-402c-885f-2a62511f64a4"),
            PersonId = PersonMockedData.Person_3203_StGallen_OldVersion_1.Id,
            Canton = "SG",
            Identifier = "O",
            Name = "Osten",
            DomainOfInfluenceType = DomainOfInfluenceType.Sk,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_OldVersion_2_PoliticalCircle_Osten
        => new()
        {
            Id = Guid.Parse("4f864c1f-7da4-4fa1-863c-0d4893e5b9b7"),
            PersonId = PersonMockedData.Person_3203_StGallen_OldVersion_2.Id,
            Canton = "SG",
            Identifier = "O",
            Name = "Osten",
            DomainOfInfluenceType = DomainOfInfluenceType.Sk,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_OldVersion_3_PoliticalCircle_Osten
        => new()
        {
            Id = Guid.Parse("53f80ae6-d1c8-491f-bdf6-c7c854fef96a"),
            PersonId = PersonMockedData.Person_3203_StGallen_OldVersion_3.Id,
            Canton = "SG",
            Identifier = "O",
            Name = "Osten",
            DomainOfInfluenceType = DomainOfInfluenceType.Sk,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_2_PoliticalCircle_Westen
        => new()
        {
            Id = Guid.Parse("d96ded43-d340-41aa-a41d-d5e21d3fbbd3"),
            PersonId = PersonMockedData.Person_3203_StGallen_2.Id,
            Canton = "SG",
            Identifier = "We",
            Name = "Westen",
            DomainOfInfluenceType = DomainOfInfluenceType.Sk,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_2_Origin1
        => new()
        {
            Id = Guid.Parse("a0b4d914-3e0c-4124-b929-e5f6e6b2f67b"),
            PersonId = PersonMockedData.Person_3203_StGallen_2.Id,
            Canton = "AI",
            Identifier = string.Empty,
            Name = "Appenzell",
            DomainOfInfluenceType = DomainOfInfluenceType.Og,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_2_CatholicCircle
        => new()
        {
            Id = Guid.Parse("d94eb15d-dfd6-4bd1-9ba7-ac7da2e2dcf4"),
            PersonId = PersonMockedData.Person_3203_StGallen_2.Id,
            Canton = "SG",
            Identifier = "Lachen",
            Name = "Katholische-Kirchgemeinde Lachen",
            DomainOfInfluenceType = DomainOfInfluenceType.KiKat,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_2_EvangelicCircle
        => new()
        {
            Id = Guid.Parse("65757146-9b1d-41c0-9e22-07fe8a4928bb"),
            PersonId = PersonMockedData.Person_3203_StGallen_2.Id,
            Canton = "SG",
            Identifier = "Rotmonten",
            Name = "Evangelische-Kirchgemeinde Rotmonten",
            DomainOfInfluenceType = DomainOfInfluenceType.KiEva,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_2_SchoolCircle
        => new()
        {
            Id = Guid.Parse("402c6235-375f-4968-a739-472337d86361"),
            PersonId = PersonMockedData.Person_3203_StGallen_2.Id,
            Canton = "SG",
            Identifier = "800",
            Name = "Schule Notkarsegg",
            DomainOfInfluenceType = DomainOfInfluenceType.Sc,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_2_ElementarySchoolCircle
        => new()
        {
            Id = Guid.Parse("eae9ea29-560f-4454-a655-61de0ab310d3"),
            PersonId = PersonMockedData.Person_3203_StGallen_2.Id,
            Canton = "SG",
            Identifier = "20",
            Name = "Primarschulkreis St. Gallen",
            DomainOfInfluenceType = DomainOfInfluenceType.ScPs,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_2_UpperSchoolCircle
        => new()
        {
            Id = Guid.Parse("e38ffd8b-dd80-47f1-ab9e-e3a416ba2ccc"),
            PersonId = PersonMockedData.Person_3203_StGallen_2.Id,
            Canton = "SG",
            Identifier = "21",
            Name = "Oberstufenschulkreis St. Gallen",
            DomainOfInfluenceType = DomainOfInfluenceType.ScOs,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_2_TrafficCircle
        => new()
        {
            Id = Guid.Parse("6b0f4042-7c00-46ab-bac6-a08b6ba46304"),
            PersonId = PersonMockedData.Person_3203_StGallen_2.Id,
            Canton = "SG",
            Identifier = "UH-02",
            Name = "UH-Verkehr",
            DomainOfInfluenceType = DomainOfInfluenceType.AnVek,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_2_ResidentialDistrictCircle
        => new()
        {
            Id = Guid.Parse("ca112e1f-957a-45c6-918c-56d6a8d80b1a"),
            PersonId = PersonMockedData.Person_3203_StGallen_2.Id,
            Canton = "SG",
            Identifier = "K999",
            Name = "Wohnquartier 999",
            DomainOfInfluenceType = DomainOfInfluenceType.AnWok,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_2_PeopleCircle
        => new()
        {
            Id = Guid.Parse("d6d19368-f9cf-4596-9e2c-adc106098f80"),
            PersonId = PersonMockedData.Person_3203_StGallen_2.Id,
            Canton = "SG",
            Identifier = "Q999",
            Name = "Haus 999",
            DomainOfInfluenceType = DomainOfInfluenceType.AnVok,
        };

    public static PersonDoiEntity PersonDoi_Person_9170_Auslandschweizer_2_Origin1_StGallen
        => new()
        {
            Id = Guid.Parse("0225789d-17bd-48f8-11b0-4865343afe69"),
            PersonId = PersonMockedData.Person_9170_Auslandschweizer_2.Id,
            Canton = "SG",
            Identifier = "SG",
            Name = "St. Gallen",
            DomainOfInfluenceType = DomainOfInfluenceType.Og,
        };

    public static PersonDoiEntity PersonDoi_Person_9170_Auslandschweizer_2_Origin2_Thurgau
       => new()
       {
           Id = Guid.Parse("55d5119d-17bd-48f8-9ab0-4865343afe69"),
           PersonId = PersonMockedData.Person_9170_Auslandschweizer_2.Id,
           Canton = "TG",
           Identifier = "TG",
           Name = "Thurgau",
           DomainOfInfluenceType = DomainOfInfluenceType.Og,
       };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_Foreigner_With_ResidenceValidDateRestriction_PoliticialCircle_Westen
        => new()
        {
            Id = Guid.Parse("03d5789d-17bd-48f8-9ab0-4865343afe69"),
            PersonId = PersonMockedData.Person_3203_StGallen_Foreigner_With_ResidenceValidDateRestriction.Id,
            Canton = "SG",
            Identifier = "W",
            Name = "Westen",
            DomainOfInfluenceType = DomainOfInfluenceType.Sk,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_3_Foreigner_Convederation
       => new()
       {
           Id = Guid.Parse("545fc4e4-5f7f-4ead-b958-2f137e177825"),
           PersonId = PersonMockedData.Person_3203_StGallen_3_Foreigner.Id,
           Canton = "SG",
           Identifier = "1",
           Name = "Kanton St.Gallen eidgenössisch",
           DomainOfInfluenceType = DomainOfInfluenceType.Ch,
       };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_Foreigner_With_ResidenceValidDateRestriction_Expired_Convederation
       => new()
       {
           Id = Guid.Parse("4dea0640-6c2f-4801-81b7-92ffe7cf76f6"),
           PersonId = PersonMockedData.Person_3203_StGallen_Foreigner_With_ResidenceValidDateRestriction_Expired.Id,
           Canton = "SG",
           Identifier = "1",
           Name = "Kanton St.Gallen eidgenössisch",
           DomainOfInfluenceType = DomainOfInfluenceType.Ch,
       };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_3_PoliticalCircle_Centrum
        => new()
        {
            Id = Guid.Parse("14fdda21-2cb1-4a1e-8111-eaa4a8a9b50f"),
            PersonId = PersonMockedData.Person_3203_StGallen_3.Id,
            Canton = "SG",
            Identifier = "C",
            Name = "Centrum",
            DomainOfInfluenceType = DomainOfInfluenceType.Sk,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_3_CatholicCircle_Neudorf
        => new()
        {
            Id = Guid.Parse("88fdd321-2cb1-4a1e-8111-eaa4a8a9b50f"),
            PersonId = PersonMockedData.Person_3203_StGallen_3.Id,
            Identifier = "ND",
            Name = "Neudorf",
            DomainOfInfluenceType = DomainOfInfluenceType.KiKat,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_3_Origin1
        => new()
        {
            Id = Guid.Parse("52054fba-01df-4201-bbc8-bd6b753c1192"),
            PersonId = PersonMockedData.Person_3203_StGallen_3.Id,
            Canton = "SG",
            Identifier = string.Empty,
            Name = "St.Gallen",
            DomainOfInfluenceType = DomainOfInfluenceType.Og,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_Auslaender_Staatenlos_PoliticalCircle_Centrum
    => new()
    {
        Id = Guid.Parse("e7325a82-c145-4ce3-95a3-350d17417b67"),
        PersonId = PersonMockedData.Person_3203_StGallen_Auslaender_Staatenlos.Id,
        Canton = "SG",
        Identifier = "C",
        Name = "Centrum",
        DomainOfInfluenceType = DomainOfInfluenceType.Sk,
    };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_Auslaender_Staatenlos_CatholicCircle_Neudorf
        => new()
        {
            Id = Guid.Parse("2d348d59-e75c-41cf-93bf-7f10b9e96c6f"),
            PersonId = PersonMockedData.Person_3203_StGallen_Auslaender_Staatenlos.Id,
            Identifier = "ND",
            Name = "Neudorf",
            DomainOfInfluenceType = DomainOfInfluenceType.KiKat,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_Auslaender_Staatenlos_Origin1
        => new()
        {
            Id = Guid.Parse("67657876-e229-463e-ad18-da87209d1fa5"),
            PersonId = PersonMockedData.Person_3203_StGallen_Auslaender_Staatenlos.Id,
            Canton = "SG",
            Identifier = string.Empty,
            Name = "St.Gallen",
            DomainOfInfluenceType = DomainOfInfluenceType.Og,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_5_Origin
        => new()
        {
            Id = Guid.Parse("2c966417-ea2e-4da1-a280-ac6ddf6b286a"),
            PersonId = PersonMockedData.Person_3203_StGallen_5.Id,
            Identifier = "SG",
            Canton = "St. Gallen",
            Name = "Goldach",
            DomainOfInfluenceType = DomainOfInfluenceType.Og,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_5_PoliticalCircle
        => new()
        {
            Id = Guid.Parse("36b27812-aaab-48a8-9a75-4836640edd4c"),
            PersonId = PersonMockedData.Person_3203_StGallen_5.Id,
            Identifier = "Political",
            Canton = "SG",
            Name = "Politischer Wahlkreis",
            DomainOfInfluenceType = DomainOfInfluenceType.Sk,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_5_CatholicCircle
        => new()
        {
            Id = Guid.Parse("b3d9672e-1029-47dc-b968-6f679879e736"),
            PersonId = PersonMockedData.Person_3203_StGallen_5.Id,
            Identifier = "KiKat",
            Canton = "SG",
            Name = "Katholische Kirche",
            DomainOfInfluenceType = DomainOfInfluenceType.KiKat,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_5_EvangelicCircle
        => new()
        {
            Id = Guid.Parse("d6e902aa-54e5-4934-8e94-2af07f50ed1e"),
            PersonId = PersonMockedData.Person_3203_StGallen_5.Id,
            Identifier = "KiEva",
            Canton = "SG",
            Name = "Evangelische Kirche",
            DomainOfInfluenceType = DomainOfInfluenceType.KiEva,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_5_SchoolCircle
        => new()
        {
            Id = Guid.Parse("0f476742-5b33-49e4-a81c-f7c945d08c55"),
            PersonId = PersonMockedData.Person_3203_StGallen_5.Id,
            Identifier = "School",
            Canton = "SG",
            Name = "Schule",
            DomainOfInfluenceType = DomainOfInfluenceType.Sc,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_5_ElementarySchoolCircle
        => new()
        {
            Id = Guid.Parse("cb12916d-6261-4bf8-ade6-bd579c731197"),
            PersonId = PersonMockedData.Person_3203_StGallen_5.Id,
            Identifier = "40",
            Canton = "SG",
            Name = "Primarschulkreis Riethüsli",
            DomainOfInfluenceType = DomainOfInfluenceType.ScPs,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_5_UpperSchoolCircle
        => new()
        {
            Id = Guid.Parse("4d4c4e3d-6e4f-4aa1-905c-4cae8f97e0b2"),
            PersonId = PersonMockedData.Person_3203_StGallen_5.Id,
            Identifier = "41",
            Canton = "SG",
            Name = "Oberstufenschulkreis Riethüsli",
            DomainOfInfluenceType = DomainOfInfluenceType.ScOs,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_5_PeopleCircle
        => new()
        {
            Id = Guid.Parse("e05a4ab9-bf19-47bd-8d21-45231c66a9d4"),
            PersonId = PersonMockedData.Person_3203_StGallen_5.Id,
            Identifier = "AnVok",
            Canton = "SG",
            Name = "Volkskreis",
            DomainOfInfluenceType = DomainOfInfluenceType.AnVok,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_5_ResidentialDistrictCircle
        => new()
        {
            Id = Guid.Parse("d5a95384-a77d-4884-bd19-04bff4f0de03"),
            PersonId = PersonMockedData.Person_3203_StGallen_5.Id,
            Identifier = "AnWok",
            Canton = "SG",
            Name = "Wohnviertel",
            DomainOfInfluenceType = DomainOfInfluenceType.AnWok,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_5_TrafficCircle
        => new()
        {
            Id = Guid.Parse("1b594628-a23c-48d0-a61a-f786bce6f119"),
            PersonId = PersonMockedData.Person_3203_StGallen_5.Id,
            Identifier = "AnVek",
            Canton = "SG",
            Name = "Verkehrskreis",
            DomainOfInfluenceType = DomainOfInfluenceType.AnVek,
        };

    public static PersonDoiEntity PersonDoi_Person_3213_Goldach_1_PoliticalCircle_Centrum
        => new()
        {
            Id = Guid.Parse("c75e5fa1-e579-422e-b8b4-745b58e57da9"),
            PersonId = PersonMockedData.Person_3213_Goldach_1.Id,
            Canton = "SG",
            Identifier = "Ce",
            Name = "Centrum",
            DomainOfInfluenceType = DomainOfInfluenceType.Sk,
        };

    public static PersonDoiEntity PersonDoi_Person_3213_Goldach_1_Origin1
        => new()
        {
            Id = Guid.Parse("d4a4be2e-119b-49be-a0e5-4cbe53b1dd02"),
            PersonId = PersonMockedData.Person_3213_Goldach_1.Id,
            Canton = "AR",
            Identifier = string.Empty,
            Name = "Gais",
            DomainOfInfluenceType = DomainOfInfluenceType.Og,
        };

    public static PersonDoiEntity PersonDoi_Person_3213_Goldach_1_CatholicCircle
        => new()
        {
            Id = Guid.Parse("ee7e1e5a-141c-4a33-8c8a-774017449ffd"),
            PersonId = PersonMockedData.Person_3213_Goldach_1.Id,
            Canton = "SG",
            Identifier = "Otmar",
            Name = "Katholische-Kirchgemeinde Otmar",
            DomainOfInfluenceType = DomainOfInfluenceType.KiKat,
        };

    public static PersonDoiEntity PersonDoi_Person_3213_Goldach_1_EvangelicCircle
        => new()
        {
            Id = Guid.Parse("eb3564d5-3d79-4add-9d54-59543544c6e3"),
            PersonId = PersonMockedData.Person_3213_Goldach_1.Id,
            Canton = "SG",
            Identifier = "Riethüsli",
            Name = "Evangelische-Kirchgemeinde Riethüsli",
            DomainOfInfluenceType = DomainOfInfluenceType.KiEva,
        };

    public static PersonDoiEntity PersonDoi_Person_3213_Goldach_1_SchoolCircle
        => new()
        {
            Id = Guid.Parse("a2e9ea57-2434-4cab-9fd4-2a249fcc4a3e"),
            PersonId = PersonMockedData.Person_3213_Goldach_1.Id,
            Canton = "SG",
            Identifier = "600",
            Name = "Schule Schönau",
            DomainOfInfluenceType = DomainOfInfluenceType.Sc,
        };

    public static PersonDoiEntity PersonDoi_Person_3213_Goldach_1_ElementarySchoolCircle
        => new()
        {
            Id = Guid.Parse("0b42c8ab-c472-44a4-a7a1-ec9e3d482ecd"),
            PersonId = PersonMockedData.Person_3213_Goldach_1.Id,
            Canton = "SG",
            Identifier = "30",
            Name = "Primarschulkreis Goldach",
            DomainOfInfluenceType = DomainOfInfluenceType.ScPs,
        };

    public static PersonDoiEntity PersonDoi_Person_3213_Goldach_1_UpperSchoolCircle
        => new()
        {
            Id = Guid.Parse("5b11ac79-4eb3-4c2b-a3b2-8d418aed2a10"),
            PersonId = PersonMockedData.Person_3213_Goldach_1.Id,
            Canton = "SG",
            Identifier = "31",
            Name = "Oberstufenschulkreis Goldach",
            DomainOfInfluenceType = DomainOfInfluenceType.ScOs,
        };

    public static PersonDoiEntity PersonDoi_Person_3213_Goldach_1_TrafficCircle
        => new()
        {
            Id = Guid.Parse("ba9c4b36-d9af-4f1f-83f2-a9e0d7c28cfa"),
            PersonId = PersonMockedData.Person_3213_Goldach_1.Id,
            Canton = "SG",
            Identifier = "TC-01",
            Name = "TCS",
            DomainOfInfluenceType = DomainOfInfluenceType.AnVek,
        };

    public static PersonDoiEntity PersonDoi_Person_3213_Goldach_1_ResidentialDistrictCircle
        => new()
        {
            Id = Guid.Parse("3be981a0-7365-4080-84d6-513179d5e539"),
            PersonId = PersonMockedData.Person_3213_Goldach_1.Id,
            Canton = "SG",
            Identifier = "K1",
            Name = "Kreis 1",
            DomainOfInfluenceType = DomainOfInfluenceType.AnWok,
        };

    public static PersonDoiEntity PersonDoi_Person_3213_Goldach_1_PeopleCircle
        => new()
        {
            Id = Guid.Parse("4924c0f4-5700-4e95-acc2-924f65b5c530"),
            PersonId = PersonMockedData.Person_3213_Goldach_1.Id,
            Canton = "SG",
            Identifier = "Q1",
            Name = "Quartier 1",
            DomainOfInfluenceType = DomainOfInfluenceType.AnVok,
        };

    public static PersonDoiEntity PersonDoi_Person_3213_Goldach_AT_1_Origin1
        => new()
        {
            Id = Guid.Parse("ac28d520-fa44-4c4d-bbfb-9118ed45a34a"),
            PersonId = PersonMockedData.Person_3213_Goldach_Swiss_Abroad.Id,
            Canton = "SG",
            Identifier = string.Empty,
            Name = "Goldach",
            DomainOfInfluenceType = DomainOfInfluenceType.Og,
        };

    public static PersonDoiEntity PersonDoi_Person_3213_Goldach_eVoter_PoliticalCircle_Centrum
        => new()
        {
            Id = Guid.Parse("1f38060c-0672-4916-8f55-03606091f4ca"),
            PersonId = PersonMockedData.Person_3213_Goldach_eVoter.Id,
            Canton = "SG",
            Identifier = "C",
            Name = "Centrum",
            DomainOfInfluenceType = DomainOfInfluenceType.Sk,
        };

    public static PersonDoiEntity PersonDoi_Person_3213_Goldach_eVoter_Origin1
        => new()
        {
            Id = Guid.Parse("c372a0b9-f153-41c4-9c92-245abde024f4"),
            PersonId = PersonMockedData.Person_3213_Goldach_eVoter.Id,
            Canton = "AR",
            Identifier = string.Empty,
            Name = "Gais",
            DomainOfInfluenceType = DomainOfInfluenceType.Og,
        };

    public static PersonDoiEntity PersonDoi_Person_3340_Jona_1_PoliticalCircle_Mittle
        => new()
        {
            Id = Guid.Parse("786c4cdd-ea21-4803-9a94-eb4e4f63cf97"),
            PersonId = PersonMockedData.Person_3340_Jona_1.Id,
            Canton = "SG",
            Identifier = "M",
            Name = "Mitte",
            DomainOfInfluenceType = DomainOfInfluenceType.Sk,
        };

    public static PersonDoiEntity PersonDoi_Person_3340_Jona_2_PoliticalCircle_Nord
        => new()
        {
            Id = Guid.Parse("79d6c0fe-610f-4bd3-8b34-e319ba9ba15e"),
            PersonId = PersonMockedData.Person_3340_Jona_2.Id,
            Canton = "SG",
            Identifier = "N",
            Name = "Nord",
            DomainOfInfluenceType = DomainOfInfluenceType.Sk,
        };

    public static PersonDoiEntity PersonDoi_Person_3213_Goldach_2_PoliticalCircle_Centrum
        => new()
        {
            Id = Guid.Parse("12d6c0fe-610f-4bd3-8b34-e388ba9ba15e"),
            PersonId = PersonMockedData.Person_3213_Goldach_2.Id,
            Canton = "SG",
            Identifier = "C",
            Name = "Centrum",
            DomainOfInfluenceType = DomainOfInfluenceType.Sk,
        };

    public static PersonDoiEntity PersonDoi_Person_3213_Goldach_2_Origin1
        => new()
        {
            Id = Guid.Parse("52d73c91-ad82-4824-bf8c-977744a2ca34"),
            PersonId = PersonMockedData.Person_3213_Goldach_2.Id,
            Canton = "AR",
            Identifier = string.Empty,
            Name = "Heiden",
            DomainOfInfluenceType = DomainOfInfluenceType.Og,
        };

    public static PersonDoiEntity PersonDoi_Person_3203_StGallen_Under18_Origin1
        => new()
        {
            Id = Guid.Parse("f21996fb-8dc1-4ccb-a814-e081fbcd49f7"),
            PersonId = PersonMockedData.Person_3203_StGallen_Under18.Id,
            Canton = "AR",
            Identifier = string.Empty,
            Name = "Herisau",
            DomainOfInfluenceType = DomainOfInfluenceType.Og,
        };

    public static PersonDoiEntity PersonDoi_Person_9170_Auslandschweizer_1_OriginNameWithOnCantonBL
        => new()
        {
            Id = Guid.Parse("7333c0fe-610f-4bd3-8b34-e3222a9baccc"),
            PersonId = PersonMockedData.Person_9170_Auslandschweizer_1.Id,
            Canton = "BL",
            Identifier = "BL",
            Name = "Basel Landschaft",
            DomainOfInfluenceType = DomainOfInfluenceType.Og,
        };

    public static PersonDoiEntity PersonDoi_Person_9170_Auslandschweizer_Under18_Origin1
        => new()
        {
            Id = Guid.Parse("c9745603-ba59-49df-8925-8081dfdf69e5"),
            PersonId = PersonMockedData.Person_9170_Auslandschweizer_Under18.Id,
            Canton = "ZH",
            Identifier = string.Empty,
            Name = "Zürich",
            DomainOfInfluenceType = DomainOfInfluenceType.Og,
        };

    public static PersonDoiEntity PersonDoi_Person_9170_Auslaender_PoliticalCircle_Westen
        => new()
        {
            Id = Guid.Parse("40123f6b-a3f8-4fb0-8d59-61c55636e2f7"),
            PersonId = PersonMockedData.Person_9170_Auslaender.Id,
            Canton = "TG",
            Identifier = "W",
            Name = "Westen",
            DomainOfInfluenceType = DomainOfInfluenceType.Sk,
        };

    public static PersonDoiEntity PersonDoi_Person_9170_Auslaender_OriginNameWithOnCantonTG
        => new()
        {
            Id = Guid.Parse("c03f86f8-79f0-45dd-8a7f-835f4c5bda37"),
            PersonId = PersonMockedData.Person_9170_Auslaender.Id,
            Canton = "TG",
            Identifier = "TG",
            Name = "Thurgau",
            DomainOfInfluenceType = DomainOfInfluenceType.Og,
        };

    public static PersonDoiEntity PersonDoi_Person_9170_RestrictedVoting_PoliticalCircle_Westen
        => new()
        {
            Id = Guid.Parse("55555558-79f0-45dd-8a7f-835f4c5bda37"),
            PersonId = PersonMockedData.Person_9170_RestrictedVoting.Id,
            Canton = string.Empty,
            Identifier = "W",
            Name = "Westen",
            DomainOfInfluenceType = DomainOfInfluenceType.Sk,
        };

    public static PersonDoiEntity PersonDoi_Person_9170_RestrictedVoting_Origin1
        => new()
        {
            Id = Guid.Parse("350cd211-346c-4a7f-8263-fb24823ee4c4"),
            PersonId = PersonMockedData.Person_9170_RestrictedVoting.Id,
            Canton = "SG",
            Identifier = string.Empty,
            Name = "Mörschwil",
            DomainOfInfluenceType = DomainOfInfluenceType.Og,
        };

    public static IEnumerable<PersonDoiEntity> All
    {
        get
        {
            yield return PersonDoi_Person_3203_StGallen_1_PoliticalCircle_Osten;
            yield return PersonDoi_Person_3203_StGallen_1_ConfederationCircle;
            yield return PersonDoi_Person_3203_StGallen_1_CantonCircle1;
            yield return PersonDoi_Person_3203_StGallen_1_DistrictCircle1;
            yield return PersonDoi_Person_3203_StGallen_1_DistrictCircle2;
            yield return PersonDoi_Person_3203_StGallen_1_MunicipalityCircle;
            yield return PersonDoi_Person_3203_StGallen_1_Origin1;
            yield return PersonDoi_Person_3203_StGallen_OldVersion_1_PoliticalCircle_Osten;
            yield return PersonDoi_Person_3203_StGallen_OldVersion_2_PoliticalCircle_Osten;
            yield return PersonDoi_Person_3203_StGallen_OldVersion_3_PoliticalCircle_Osten;
            yield return PersonDoi_Person_3203_StGallen_2_PoliticalCircle_Westen;
            yield return PersonDoi_Person_3203_StGallen_2_Origin1;
            yield return PersonDoi_Person_3203_StGallen_2_CatholicCircle;
            yield return PersonDoi_Person_3203_StGallen_2_EvangelicCircle;
            yield return PersonDoi_Person_3203_StGallen_2_SchoolCircle;
            yield return PersonDoi_Person_3203_StGallen_2_ElementarySchoolCircle;
            yield return PersonDoi_Person_3203_StGallen_2_UpperSchoolCircle;
            yield return PersonDoi_Person_3203_StGallen_2_TrafficCircle;
            yield return PersonDoi_Person_3203_StGallen_2_ResidentialDistrictCircle;
            yield return PersonDoi_Person_3203_StGallen_2_PeopleCircle;
            yield return PersonDoi_Person_9170_Auslandschweizer_2_Origin1_StGallen;
            yield return PersonDoi_Person_9170_Auslandschweizer_2_Origin2_Thurgau;
            yield return PersonDoi_Person_3203_StGallen_Foreigner_With_ResidenceValidDateRestriction_PoliticialCircle_Westen;
            yield return PersonDoi_Person_3203_StGallen_3_Foreigner_Convederation;
            yield return PersonDoi_Person_3203_StGallen_Foreigner_With_ResidenceValidDateRestriction_Expired_Convederation;
            yield return PersonDoi_Person_3203_StGallen_3_PoliticalCircle_Centrum;
            yield return PersonDoi_Person_3203_StGallen_3_CatholicCircle_Neudorf;
            yield return PersonDoi_Person_3203_StGallen_3_Origin1;
            yield return PersonDoi_Person_3203_StGallen_5_Origin;
            yield return PersonDoi_Person_3203_StGallen_5_PoliticalCircle;
            yield return PersonDoi_Person_3203_StGallen_5_CatholicCircle;
            yield return PersonDoi_Person_3203_StGallen_5_EvangelicCircle;
            yield return PersonDoi_Person_3203_StGallen_5_SchoolCircle;
            yield return PersonDoi_Person_3203_StGallen_5_ElementarySchoolCircle;
            yield return PersonDoi_Person_3203_StGallen_5_UpperSchoolCircle;
            yield return PersonDoi_Person_3203_StGallen_5_TrafficCircle;
            yield return PersonDoi_Person_3203_StGallen_5_ResidentialDistrictCircle;
            yield return PersonDoi_Person_3203_StGallen_5_PeopleCircle;
            yield return PersonDoi_Person_3213_Goldach_1_PoliticalCircle_Centrum;
            yield return PersonDoi_Person_3213_Goldach_1_Origin1;
            yield return PersonDoi_Person_3213_Goldach_1_CatholicCircle;
            yield return PersonDoi_Person_3213_Goldach_1_EvangelicCircle;
            yield return PersonDoi_Person_3213_Goldach_1_SchoolCircle;
            yield return PersonDoi_Person_3213_Goldach_1_ElementarySchoolCircle;
            yield return PersonDoi_Person_3213_Goldach_1_UpperSchoolCircle;
            yield return PersonDoi_Person_3213_Goldach_1_TrafficCircle;
            yield return PersonDoi_Person_3213_Goldach_1_ResidentialDistrictCircle;
            yield return PersonDoi_Person_3213_Goldach_1_PeopleCircle;
            yield return PersonDoi_Person_3213_Goldach_AT_1_Origin1;
            yield return PersonDoi_Person_3213_Goldach_eVoter_PoliticalCircle_Centrum;
            yield return PersonDoi_Person_3213_Goldach_eVoter_Origin1;
            yield return PersonDoi_Person_3340_Jona_1_PoliticalCircle_Mittle;
            yield return PersonDoi_Person_3340_Jona_2_PoliticalCircle_Nord;
            yield return PersonDoi_Person_3213_Goldach_2_PoliticalCircle_Centrum;
            yield return PersonDoi_Person_3213_Goldach_2_Origin1;
            yield return PersonDoi_Person_3203_StGallen_Under18_Origin1;
            yield return PersonDoi_Person_9170_Auslandschweizer_1_OriginNameWithOnCantonBL;
            yield return PersonDoi_Person_9170_Auslandschweizer_Under18_Origin1;
            yield return PersonDoi_Person_9170_Auslaender_PoliticalCircle_Westen;
            yield return PersonDoi_Person_9170_Auslaender_OriginNameWithOnCantonTG;
            yield return PersonDoi_Person_9170_RestrictedVoting_PoliticalCircle_Westen;
            yield return PersonDoi_Person_9170_RestrictedVoting_Origin1;
            yield return PersonDoi_Person_3203_StGallen_Auslaender_Staatenlos_PoliticalCircle_Centrum;
            yield return PersonDoi_Person_3203_StGallen_Auslaender_Staatenlos_CatholicCircle_Neudorf;
            yield return PersonDoi_Person_3203_StGallen_Auslaender_Staatenlos_Origin1;
        }
    }
}
