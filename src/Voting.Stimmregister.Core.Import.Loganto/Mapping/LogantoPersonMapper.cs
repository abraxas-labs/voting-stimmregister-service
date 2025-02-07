// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Globalization;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Core.Import.Mapping;
using Voting.Stimmregister.Abstractions.Core.Import.Models;
using Voting.Stimmregister.Core.Import.Loganto.Utils;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Constants;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Import;
using Voting.Stimmregister.Domain.Utils;

namespace Voting.Stimmregister.Core.Import.Loganto.Mapping;

public class LogantoPersonMapper : BasePersonMapper<LogantoPersonCsvRecord>
{
    private const string SwissCountryIdIso2 = "CH";
    private readonly ICountryHelperService _countryHelper;
    private readonly LogantoPersonImportConfig _config;

    public LogantoPersonMapper(ICountryHelperService countryHelper, IClock clock, LogantoPersonImportConfig config)
        : base(clock, ImportSourceSystem.Loganto, countryHelper)
    {
        _countryHelper = countryHelper;
        _config = config;
    }

    /// <summary>
    /// Maps the record properties to the person entity properties.
    /// Due to performance reasons, a static mapping has been preferred instead of AutoMapper.
    /// </summary>
    /// <param name="state">The import state model.</param>
    /// <param name="record">The csv record.</param>
    /// <param name="entity">The person entity.</param>
    protected override void MapRecordToPersonEntity(
        PersonImportStateModel state,
        LogantoPersonCsvRecord record,
        PersonEntity entity)
    {
        var country = _countryHelper.GetLogantoCountryTwoLetterIsoAndShortNameDe(record.Country)!;

        entity.SourceSystemId = record.SourceSystemId;
        entity.SourceSystemName = ImportSourceSystem.Loganto;
        entity.Vn = record.Vn;
        entity.OfficialName = record.OfficialName;
        entity.FirstName = record.FirstName;
        entity.Sex = LogantoUtil.ConvertLogantoSex(record.Sex);
        entity.DateOfBirth = LogantoUtil.ConvertLogantoDateOfBirth(record.DateOfBirth, true, out var dateOfBirthAdjusted);
        entity.DateOfBirthAdjusted = dateOfBirthAdjusted;
        entity.OriginalName = record.OriginalName;
        entity.AllianceName = !string.IsNullOrEmpty(record.AllianceName) ? $"{record.OfficialName}-{record.AllianceName}" : null;
        entity.AliasName = record.AliasName;
        entity.OtherName = record.OtherName;
        entity.CallName = record.CallName;
        entity.Country = country.Iso2Id;
        entity.CountryNameShort = country.ShortNameDe;
        entity.ContactAddressExtensionLine1 = record.ContactAddressExtensionLine1;
        entity.ContactAddressExtensionLine2 = record.ContactAddressExtensionLine2;
        entity.ContactAddressStreet = record.ContactAddressStreet;
        entity.ContactAddressHouseNumber = record.ContactAddressHouseNumber;
        entity.ContactAddressDwellingNumber = record.ContactAddressDwellingNumber;
        entity.ContactAddressPostOfficeBoxText = record.ContactAddressPostOfficeBoxText;
        entity.ContactAddressTown = record.ContactAddressTown;
        entity.ContactAddressZipCode = record.ContactAddressSwissZipCode;
        entity.ContactCantonAbbreviation = record.ContactCantonAbbreviation;
        entity.ContactAddressCountryIdIso2 = record.ContactAddressCountryIdIso2?.ToUpper(CultureInfo.InvariantCulture);
        entity.Religion = LogantoUtil.ConvertLogantoReligion(record.Religion);
        entity.ResidencePermit = LogantoUtil.GetRelevantResidencePermitPart(record.ResidencePermit);
        entity.ResidenceEntryDate = LogantoUtil.ConvertLogantoDate(record.ResidenceEntryDate);
        entity.MunicipalityName = record.MunicipalityName;
        entity.MunicipalityId = record.MunicipalityId;
        entity.LanguageOfCorrespondence = LogantoUtil.ConvertLogantoLanguage(record.LanguageOfCorrespondence);
        entity.DomainOfInfluenceId = record.DomainOfInfluenceId;
        entity.MoveInArrivalDate = LogantoUtil.ConvertLogantoDate(record.MoveInArrivalDate);
        entity.MoveInMunicipalityName = record.MoveInMunicipalityName;
        entity.MoveInCantonAbbreviation = record.MoveInCantonAbbreviation;
        entity.MoveInComesFrom = record.MoveInComeFrom;
        entity.MoveInCountryNameShort = record.MoveInCountryNameShort;
        entity.MoveInUnknown = record.MoveInUnknown;
        entity.ResidenceAddressExtensionLine1 = record.ResidenceAddressExtensionLine1;
        entity.ResidenceAddressExtensionLine2 = record.ResidenceAddressExtensionLine2;
        entity.ResidenceAddressStreet = record.ResidenceAddressStreet;
        entity.ResidenceAddressHouseNumber = record.ResidenceAddressHouseNumber;
        entity.ResidenceAddressDwellingNumber = record.ResidenceAddressDwellingNumber;
        entity.ResidenceAddressPostOfficeBoxText = record.ResidenceAddressPostOfficeBoxText;
        entity.ResidenceAddressTown = record.ResidenceAddressTown;
        entity.ResidenceAddressZipCode = record.ResidenceAddressSwissZipCode;
        entity.ResidenceCantonAbbreviation = record.ResidenceCantonAbbreviation;
        entity.ResidenceCountry = SwissCountryIdIso2;
        entity.EVoting = record.Vn.HasValue && state.EVotingEnabledVns.Contains(record.Vn.Value);
        entity.TypeOfResidence =
            LogantoUtil.EvaluateResidenceType(record.HasMainResidence, record.HasSecondaryResidence);
        entity.RestrictedVotingAndElectionRightFederation = LogantoUtil.IsRestrictedToVote(
            entity.TypeOfResidence,
            record.RestrictedVotingAndElectionRightFederation);
        entity.SendVotingCardsToDomainOfInfluenceReturnAddress = LogantoUtil.ShouldSendVotingCardsToDomainOfInfluenceReturnAddress(
            record,
            _config.BfsThatAllowSendingVotingCardForPeopleWithAwayAddresses,
            _config.BfsThatAllowSendingVotingCardForPeopleWithUnknownMainResidenceAddresses);
        entity.IsHouseholder = record.IsHouseholder;
        entity.ResidenceBuildingId = record.ResidenceBuildingId;
        entity.ResidenceApartmentId = record.ResidenceApartmentId;

        if (!Countries.IsSwitzerland(entity.Country))
        {
            entity.ResidencePermitValidFrom = LogantoUtil.ConvertLogantoDate(record.ResidencePermitValidFrom);
            entity.ResidencePermitValidTill = LogantoUtil.ConvertLogantoDate(record.ResidencePermitValidTill, false);
        }

        entity.ImportStatisticId = state.ImportStatisticId;
        entity.PersonDois.Clear();
        MapDomainOfInfluenceData(state, record, entity);
        MapFallbackAddress(entity, state);
    }

    private static void MapDomainOfInfluenceData(
        PersonImportStateModel importState,
        LogantoPersonCsvRecord record,
        PersonEntity personEntity)
    {
        // Domain of Influence (OG)
        AddPersonDoiEntityByOriginNameAndOnCanton(personEntity, record.OriginName1, record.OnCanton1);
        AddPersonDoiEntityByOriginNameAndOnCanton(personEntity, record.OriginName2, record.OnCanton2);
        AddPersonDoiEntityByOriginNameAndOnCanton(personEntity, record.OriginName3, record.OnCanton3);
        AddPersonDoiEntityByOriginNameAndOnCanton(personEntity, record.OriginName4, record.OnCanton4);
        AddPersonDoiEntityByOriginNameAndOnCanton(personEntity, record.OriginName5, record.OnCanton5);
        AddPersonDoiEntityByOriginNameAndOnCanton(personEntity, record.OriginName6, record.OnCanton6);
        AddPersonDoiEntityByOriginNameAndOnCanton(personEntity, record.OriginName7, record.OnCanton7);

        // Domain of Influence (SK, SC, KI, KO, AN, KIKAT, KIEVA, ANVEK, ANWOK, ANVOK)
        var domainOfInfluence = record.DomainOfInfluenceId == null
            ? null
            : importState.FindDomainOfInfluence(record.DomainOfInfluenceId);

        if (domainOfInfluence == null)
        {
            MapCirclesFromPersonRecord(record, personEntity);
        }
        else
        {
            MapCirclesFromDomainOfInfluenceReference(personEntity, domainOfInfluence);
        }

        // Domain of Influence (CH, CT, BZ, MU)
        MapDoiFromAcl(importState, personEntity);
    }

    private static void MapDoiFromAcl(PersonImportStateModel importState, PersonEntity personEntity)
    {
        foreach (var doi in importState.PersonDoisFromAclByBfs)
        {
            if (string.IsNullOrWhiteSpace(doi.Name))
            {
                continue;
            }

            var personDoiEntity = CreatePersonDoiEntity(personEntity.Id, doi.Identifier, doi.Name, doi.Canton, doi.DomainOfInfluenceType);
            personEntity.PersonDois.Add(personDoiEntity);
        }
    }

    private static void MapCirclesFromPersonRecord(LogantoPersonCsvRecord record, PersonEntity personEntity)
    {
        AddPersonDoiEntityByCircleIdAndCircleName(personEntity, record.PoliticalCircleId, record.PoliticalCircleName, DomainOfInfluenceType.Sk);
        AddPersonDoiEntityByCircleIdAndCircleName(personEntity, record.CatholicCircleId, record.CatholicCircleName, DomainOfInfluenceType.KiKat);
        AddPersonDoiEntityByCircleIdAndCircleName(personEntity, record.EvangelicCircleId, record.EvangelicCircleName, DomainOfInfluenceType.KiEva);
        AddPersonDoiEntityByCircleIdAndCircleName(personEntity, record.SchoolCircleId, record.SchoolCircleName, DomainOfInfluenceType.Sc);
        AddPersonDoiEntityByCircleIdAndCircleName(personEntity, record.TrafficCircleId, record.TrafficCircleName, DomainOfInfluenceType.AnVek);
        AddPersonDoiEntityByCircleIdAndCircleName(personEntity, record.ResidentialDistrictCircleId, record.ResidentialDistrictCircleName, DomainOfInfluenceType.AnWok);
        AddPersonDoiEntityByCircleIdAndCircleName(personEntity, record.PeopleCircleId, record.PeopleCircleName, DomainOfInfluenceType.AnVok);
    }

    private static void MapCirclesFromDomainOfInfluenceReference(
        PersonEntity personEntity,
        DomainOfInfluenceEntity domainOfInfluenceEntity)
    {
        AddPersonDoiEntityByCircleIdAndCircleName(personEntity, domainOfInfluenceEntity.PoliticalCircleId, domainOfInfluenceEntity.PoliticalCircleName, DomainOfInfluenceType.Sk);
        AddPersonDoiEntityByCircleIdAndCircleName(personEntity, domainOfInfluenceEntity.CatholicChurchCircleId, domainOfInfluenceEntity.CatholicChurchCircleName, DomainOfInfluenceType.KiKat);
        AddPersonDoiEntityByCircleIdAndCircleName(personEntity, domainOfInfluenceEntity.EvangelicChurchCircleId, domainOfInfluenceEntity.EvangelicChurchCircleName, DomainOfInfluenceType.KiEva);
        AddPersonDoiEntityByCircleIdAndCircleName(personEntity, domainOfInfluenceEntity.SchoolCircleId, domainOfInfluenceEntity.SchoolCircleName, DomainOfInfluenceType.Sc);
        AddPersonDoiEntityByCircleIdAndCircleName(personEntity, domainOfInfluenceEntity.TrafficCircleId, domainOfInfluenceEntity.TrafficCircleName, DomainOfInfluenceType.AnVek);
        AddPersonDoiEntityByCircleIdAndCircleName(personEntity, domainOfInfluenceEntity.ResidentialDistrictCircleId, domainOfInfluenceEntity.ResidentialDistrictCircleName, DomainOfInfluenceType.AnWok);
        AddPersonDoiEntityByCircleIdAndCircleName(personEntity, domainOfInfluenceEntity.PeopleCouncilCircleId, domainOfInfluenceEntity.PeopleCouncilCircleName, DomainOfInfluenceType.AnVok);
    }

    private static void AddPersonDoiEntityByCircleIdAndCircleName(
        PersonEntity personEntity,
        string? circleId,
        string? circleName,
        DomainOfInfluenceType type)
    {
        if (string.IsNullOrWhiteSpace(circleName))
        {
            return;
        }

        var personDoi = CreatePersonDoiEntity(personEntity.Id, circleId, circleName, string.Empty, type);
        personEntity.PersonDois.Add(personDoi);
    }

    private static void AddPersonDoiEntityByOriginNameAndOnCanton(
        PersonEntity personEntity,
        string? originName,
        string? onCanton)
    {
        if (string.IsNullOrWhiteSpace(originName))
        {
            return;
        }

        var personDoi = CreatePersonDoiEntity(personEntity.Id, null, originName, onCanton, DomainOfInfluenceType.Og);
        personEntity.PersonDois.Add(personDoi);
    }

    private static PersonDoiEntity CreatePersonDoiEntity(
        Guid personId,
        string? identifier,
        string? name,
        string? canton,
        DomainOfInfluenceType type)
    {
        return new PersonDoiEntity
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            DomainOfInfluenceType = type,
            Name = name ?? string.Empty,
            Canton = canton ?? string.Empty,
            Identifier = identifier ?? string.Empty,
        };
    }
}
