// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using ABX_Voting_1_0;
using Ech0044_4_1;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Import.Mapping;
using Voting.Stimmregister.Abstractions.Import.Models;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using ResidenceType = Voting.Stimmregister.Domain.Enums.ResidenceType;
using SexType = Ech0044_4_1.SexType;

namespace Voting.Stimmregister.Import.Innosolv.Mapping;

internal class InnosolvPersonMapper : BasePersonMapper<PersonInfoType>
{
    private const string CountryUnknown = "Staat unbekannt";
    private readonly ImportsConfig _importConfig;

    public InnosolvPersonMapper(IClock clock, ImportsConfig importConfig)
        : base(clock, ImportSourceSystem.Innosolv)
    {
        _importConfig = importConfig;
    }

    protected override void MapRecordToEntity(PersonImportStateModel state, PersonInfoType record, PersonEntity entity)
    {
        entity.Vn = (long?)record.PersonIdentification.Vn;
        entity.SourceSystemId = record.PersonIdentification.LocalPersonId.PersonId;
        entity.OfficialName = record.NameData.OfficialName;
        entity.FirstName = record.NameData.FirstName;
        entity.Sex = record.PersonIdentification.Sex switch
        {
            SexType.Item1 => Domain.Enums.SexType.Male,
            SexType.Item2 => Domain.Enums.SexType.Female,
            _ => Domain.Enums.SexType.Undefined,
        };
        (entity.DateOfBirth, entity.DateOfBirthAdjusted) = ConvertToDateOnly(record.PersonIdentification.DateOfBirth);
        entity.OriginalName = record.NameData.OriginalName;
        entity.AllianceName = record.NameData.AllianceName;
        entity.AliasName = record.NameData.AliasName;
        entity.OtherName = record.NameData.OtherName;
        entity.CallName = record.NameData.CallName;
        entity.LanguageOfCorrespondence = record.PersonAdditionalData?.LanguageOfCorrespondance;
        entity.Religion = record.ReligionData.Religion switch
        {
            "111" => ReligionType.Evangelic,
            "121" => ReligionType.Catholic,
            "1221" => ReligionType.ChristCatholic,
            _ => ReligionType.Unknown,
        };

        entity.IsSwissAbroad = _importConfig.SwissAbroadMunicipalityIdWhitelist.Contains(entity.MunicipalityId.ToString());
        entity.SendVotingCardsToDomainOfInfluenceReturnAddress = record.DispatchLockVotingCard == true;
        entity.RestrictedVotingAndElectionRightFederation = record.PoliticalRightData?.RestrictedVotingAndElectionRightFederation == true;
        entity.Country = record.NationalityData.CountryInfo.FirstOrDefault()?.Country.CountryIdIso2;
        entity.CountryNameShort = record.NationalityData.CountryInfo.FirstOrDefault()?.Country.CountryNameShort ?? CountryUnknown;
        entity.TypeOfResidence = record.HasMainResidence != null
            ? ResidenceType.HWS
            : ResidenceType.NWS;
        entity.ResidencePermit = XmlUtil.GetXmlEnumAttributeValueFromEnum(record.ResidencePermit?.ResidencePermit);
        entity.ResidencePermitValidFrom = DateOnlyFromNullable(record.ResidencePermit?.ResidencePermitValidFrom);
        entity.ResidencePermitValidTill = DateOnlyFromNullable(record.ResidencePermit?.ResidencePermitValidTill);
        entity.ResidenceEntryDate = DateOnlyFromNullable(record.ResidencePermit?.EntryDate);

        entity.ImportStatisticId = state.ImportStatisticId;
        entity.EVoting = record.PersonIdentification.Vn.HasValue && state.EVotingEnabledVns.Contains((long)record.PersonIdentification.Vn.Value);

        MapContactAddress(entity, record);
        MapResidence(entity, record);
        MapDomainOfInfluences(state, entity, record);
    }

    private void MapDomainOfInfluences(PersonImportStateModel state, PersonEntity entity, PersonInfoType record)
    {
        MapPlaceOfOriginDomainOfInfluences(entity, record);
        MapAclBfsDomainOfInfluences(state, entity);
        MapDistrictsToDomainOfInfluences(entity, record);
    }

    private void MapDistrictsToDomainOfInfluences(PersonEntity entity, PersonInfoType record)
    {
        if (record.District == null || record.District.Count == 0)
        {
            return;
        }

        foreach (var district in record.District)
        {
            entity.PersonDois.Add(new PersonDoiEntity
            {
                Id = Guid.NewGuid(),
                PersonId = entity.Id,
                Identifier = district.LocalDistrictId,
                Name = district.DistrictName,
                DomainOfInfluenceType = district.DistrictCategory switch
                {
                    "CH" => DomainOfInfluenceType.Ch,
                    "CT" => DomainOfInfluenceType.Ct,
                    "BZ" => DomainOfInfluenceType.Bz,
                    "MU" => DomainOfInfluenceType.Mu,
                    "SC" => DomainOfInfluenceType.Sc,
                    "KI" => DomainOfInfluenceType.Ki,
                    "OG" => DomainOfInfluenceType.Og,
                    "KO" => DomainOfInfluenceType.Ko,
                    "SK" => DomainOfInfluenceType.Sk,
                    _ => DomainOfInfluenceType.An,
                },
            });
        }
    }

    private void MapAclBfsDomainOfInfluences(PersonImportStateModel state, PersonEntity entity)
    {
        foreach (var doi in state.PersonDoisFromAclByBfs)
        {
            entity.PersonDois.Add(new PersonDoiEntity
            {
                Id = Guid.NewGuid(),
                PersonId = entity.Id,
                DomainOfInfluenceType = doi.DomainOfInfluenceType,
                Identifier = doi.Identifier,
                Canton = doi.Canton,
                Name = doi.Name,
            });
        }
    }

    private void MapPlaceOfOriginDomainOfInfluences(PersonEntity entity, PersonInfoType record)
    {
        if (record.PlaceOfOrigin == null || record.PlaceOfOrigin.Count == 0)
        {
            return;
        }

        foreach (var placeOfOrigin in record.PlaceOfOrigin)
        {
            entity.PersonDois.Add(new PersonDoiEntity
            {
                Id = Guid.NewGuid(),
                PersonId = entity.Id,
                DomainOfInfluenceType = DomainOfInfluenceType.Og,
                Canton = placeOfOrigin.Canton.ToString().ToUpperInvariant(),
                Name = placeOfOrigin.OriginName,
                Identifier = placeOfOrigin.PlaceOfOriginId?.ToString() ?? string.Empty,
            });
        }
    }

    private void MapResidence(PersonEntity entity, PersonInfoType record)
    {
        var residence = record.HasMainResidence ?? record.HasSecondaryResidence;
        if (residence == null)
        {
            return;
        }

        entity.ResidenceAddressExtensionLine1 = residence.DwellingAddress.Address.AddressLine1;
        entity.ResidenceAddressExtensionLine2 = residence.DwellingAddress.Address.AddressLine2;
        entity.ResidenceAddressStreet = residence.DwellingAddress.Address.Street;
        entity.ResidenceAddressHouseNumber = residence.DwellingAddress.Address.HouseNumber;
        entity.ResidenceAddressDwellingNumber = residence.DwellingAddress.Address.DwellingNumber;
        entity.ResidenceAddressTown = residence.DwellingAddress.Address.Town;
        entity.ResidenceCantonAbbreviation = residence.ReportingMunicipality?.CantonAbbreviation.ToString();
        entity.ResidenceCountry = residence.DwellingAddress.Address.Country;
        entity.ResidenceAddressZipCode = residence.DwellingAddress.Address.SwissZipCode.ToString();
        entity.MoveInArrivalDate = DateOnlyFromNullable(residence.ArrivalDate);
        entity.MoveInUnknown = residence.ComesFrom?.UnknownValueSpecified;
        entity.MoveInCantonAbbreviation = residence.ComesFrom?.SwissTown?.CantonAbbreviation?.ToString().ToUpperInvariant();
        entity.MoveInMunicipalityName = residence.ComesFrom?.SwissTown?.MunicipalityName;
        entity.MoveInCountryNameShort = residence.ComesFrom?.ForeignCountry?.Country?.CountryNameShort;

        if (entity.MoveInUnknown == true)
        {
            entity.MoveInComesFrom = residence.ComesFrom?.ForeignCountry?.Town
                ?? residence.ComesFrom?.SwissTown?.MunicipalityName;
        }
    }

    private void MapContactAddress(PersonEntity entity, PersonInfoType record)
    {
        var contactAddress = record.ContactData?.ContactAddress?.AddressInformation;
        if (contactAddress == null)
        {
            MapContactAddressFromResidence(entity, record);
            return;
        }

        entity.ContactAddressExtensionLine1 = contactAddress.AddressLine1;
        entity.ContactAddressExtensionLine2 = contactAddress.AddressLine2;
        entity.ContactAddressStreet = contactAddress.Street;
        entity.ContactAddressHouseNumber = contactAddress.HouseNumber;
        entity.ContactAddressDwellingNumber = contactAddress.DwellingNumber;

        if (!string.IsNullOrEmpty(contactAddress.PostOfficeBoxText))
        {
            entity.ContactAddressPostOfficeBoxText = contactAddress.PostOfficeBoxText;
        }

        entity.ContactAddressPostOfficeBoxNumber = (int?)contactAddress.PostOfficeBoxNumber;
        entity.ContactAddressTown = contactAddress.Town;
        entity.ContactAddressLocality = contactAddress.Locality;
        entity.ContactAddressZipCode = contactAddress.ForeignZipCode ?? contactAddress.SwissZipCode?.ToString();
    }

    private void MapContactAddressFromResidence(PersonEntity entity, PersonInfoType record)
    {
        var address = record.HasMainResidence?.DwellingAddress.Address
            ?? record.HasSecondaryResidence?.DwellingAddress.Address;
        if (address == null)
        {
            return;
        }

        entity.ContactAddressExtensionLine1 = address.AddressLine1;
        entity.ContactAddressExtensionLine2 = address.AddressLine2;
        entity.ContactAddressStreet = address.Street;
        entity.ContactAddressHouseNumber = address.HouseNumber;
        entity.ContactAddressDwellingNumber = address.DwellingNumber;
        entity.ContactAddressTown = address.Town;
        entity.ContactAddressLocality = address.Locality;
        entity.ContactAddressZipCode = address.SwissZipCode.ToString();
    }

    private DateOnly? DateOnlyFromNullable(DateTime? dateTime)
    {
        return dateTime.HasValue
            ? DateOnly.FromDateTime(dateTime.Value)
            : null;
    }

    private (DateOnly Date, bool DateAdjusted) ConvertToDateOnly(DatePartiallyKnownType partiallyKnownDate)
    {
        if (partiallyKnownDate.YearMonthDay.HasValue)
        {
            return (DateOnly.FromDateTime(partiallyKnownDate.YearMonthDay.Value), false);
        }

        if (!string.IsNullOrWhiteSpace(partiallyKnownDate.YearMonth))
        {
            var parts = partiallyKnownDate.YearMonth.Split('-', 2);
            var yearPart = int.Parse(parts[0]);
            var monthPart = int.Parse(parts[1]);
            return (new DateOnly(yearPart, monthPart, 1), true);
        }

        var year = int.Parse(partiallyKnownDate.Year);
        return (new DateOnly(year, 1, 1), true);
    }
}
