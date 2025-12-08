// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Core.Import.Mapping;
using Voting.Stimmregister.Abstractions.Core.Import.Models;
using Voting.Stimmregister.Core.Import.Cobra.Tg.Utils;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Import;
using Voting.Stimmregister.Domain.Utils;

namespace Voting.Stimmregister.Core.Import.Cobra.Tg.Mapping;

public class CobraPersonMapper : BasePersonMapper<CobraTgPersonCsvRecord>
{
    private readonly ICountryHelperService _countryHelper;

    public CobraPersonMapper(ICountryHelperService countryHelper, IClock clock)
        : base(clock, ImportSourceSystem.CobraTg, countryHelper)
    {
        _countryHelper = countryHelper;
    }

    protected override void MapRecordToPersonEntity(
        PersonImportStateModel state,
        CobraTgPersonCsvRecord record,
        PersonEntity entity)
    {
        entity.SourceSystemId = record.SourceSystemId;
        entity.SourceSystemName = ImportSourceSystem.CobraTg;
        entity.OfficialName = record.OfficialName;
        entity.FirstName = record.FirstName;
        entity.Sex = CobraUtil.ConvertSalutationToSexType(record.Salutation);
        entity.DateOfBirth = CobraUtil.ConvertDateOfBirth(record.DateOfBirth, out var dateOfBirthAdjusted);
        entity.DateOfBirthAdjusted = dateOfBirthAdjusted;
        entity.LanguageOfCorrespondence = record.LanguageOfCorrespondence;
        entity.EVoting = true;
        entity.MunicipalityId = record.MunicipalityId;
        entity.MunicipalityName = CobraUtil.DefaultMunicipalityName;
        entity.ContactAddressStreet = record.ContactAddressStreet;
        entity.ContactAddressHouseNumber = record.ContactAddressHouseNumber;
        entity.ContactAddressExtensionLine1 = record.ContactAddressExtensionLine1;
        entity.ContactAddressExtensionLine2 = record.ContactAddressExtensionLine2;
        entity.ContactAddressPostOfficeBoxText = record.ContactAddressPostOfficeBoxText;
        entity.ContactAddressTown = record.ContactAddressTown;
        entity.ContactAddressLocality = record.ContactAddressLocality;
        entity.ContactAddressZipCode = record.ContactAddressZipCode;
        entity.ContactAddressCountryIdIso2 = _countryHelper.GetCountryTwoLetterIsoCode(record.ResidenceCountry, iso2: record.ResidenceCountryShort);
        entity.ResidenceCountry = entity.ContactAddressCountryIdIso2;
        entity.MoveInArrivalDate = DateOnly.FromDateTime(entity.CreatedDate);
        entity.ImportStatisticId = state.ImportStatisticId;
        MapDomainOfInfluenceData(state, record, entity);
        MapStaticAssignments(entity);
    }

    /// <summary>
    /// Static values are assigned to person properties which are implicitly
    /// specified via the persons of type "Swiss Abroad".
    /// </summary>
    /// <param name="entity">The person entity.</param>
    private static void MapStaticAssignments(PersonEntity entity)
    {
        entity.TypeOfResidence = ResidenceType.HWS;
        entity.IsSwissAbroad = true;
        entity.RestrictedVotingAndElectionRightFederation = false;
        entity.Country = CobraUtil.DefaultCountryValue;
        entity.CountryNameShort = CobraUtil.DefaultCountryNameShortValue;
    }

    private static void MapDomainOfInfluenceData(
        PersonImportStateModel importState,
        CobraTgPersonCsvRecord record,
        PersonEntity entity)
    {
        // Domain of Influence (OG)
        if (!string.IsNullOrWhiteSpace(record.OriginName1))
        {
            entity.PersonDois.Add(new()
            {
                Id = Guid.NewGuid(),
                PersonId = entity.Id,
                DomainOfInfluenceType = DomainOfInfluenceType.Og,
                Name = record.OriginName1 ?? string.Empty,
                Canton = CobraUtil.DefaultOriginOnCanton,
            });
        }

        // Domain of Influence (CH, CT, BZ, MU)
        foreach (var doi in importState.PersonDoisFromAclByBfs)
        {
            entity.PersonDois.Add(new()
            {
                Id = Guid.NewGuid(),
                PersonId = entity.Id,
                DomainOfInfluenceType = doi.DomainOfInfluenceType,
                Name = doi.Name,
                Canton = doi.Canton,
                Identifier = doi.Identifier,
            });
        }
    }
}
