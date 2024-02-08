// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Import.Mapping;
using Voting.Stimmregister.Abstractions.Import.Models;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Import;
using Voting.Stimmregister.Domain.Utils;
using Voting.Stimmregister.Import.Loganto.Utils;

namespace Voting.Stimmregister.Import.Loganto.Mapping;

public class CobraPersonMapper : BasePersonMapper<CobraPersonCsvRecord>
{
    private readonly ICountryHelperService _countryHelper;

    public CobraPersonMapper(ICountryHelperService countryHelper, IClock clock)
        : base(clock, ImportSourceSystem.Cobra)
    {
        _countryHelper = countryHelper;
    }

    protected override void MapRecordToEntity(
        PersonImportStateModel importState,
        CobraPersonCsvRecord record,
        PersonEntity entity)
    {
        entity.SourceSystemId = record.SourceSystemId;
        entity.SourceSystemName = ImportSourceSystem.Cobra;
        entity.Vn = record.Vn;
        entity.OfficialName = record.OfficialName;
        entity.FirstName = record.FirstName;
        entity.Sex = CobraUtil.ConvertSexType(record.Sex);
        entity.DateOfBirth = CobraUtil.ConvertDateOfBirth(record.DateOfBirth, out var dateOfBirthAdjusted);
        entity.DateOfBirthAdjusted = dateOfBirthAdjusted;
        entity.LanguageOfCorrespondence = record.LanguageOfCorrespondence;
        entity.EVoting = record.SwissAbroadEvotingFlag == true;
        entity.MunicipalityId = record.MunicipalityId;
        entity.MunicipalityName = importState.MunicipalityName;
        entity.ContactAddressStreet = record.ContactAddressStreet;
        entity.ContactAddressHouseNumber = record.ContactAddressHouseNumber;
        entity.ContactAddressDwellingNumber = string.IsNullOrEmpty(record.ContactAddressHouseNumberAddition)
            ? null
            : record.ContactAddressHouseNumberAddition;
        entity.ContactAddressPostOfficeBoxText = record.ContactAddressPostOfficeBoxText;
        entity.ContactAddressTown = record.ContactAddressTown;
        entity.ContactAddressLocality = record.ContactAddressLocality;
        entity.ContactAddressZipCode = record.ContactAddressZipCode;
        entity.ResidenceCountry = _countryHelper.GetCountryTwoLetterIsoCode(record.ResidenceCountry, record.BfsCountry);
        entity.MoveInComesFrom = record.MoveInComesFrom;
        entity.MoveInArrivalDate = DateOnly.FromDateTime(entity.CreatedDate);
        entity.ImportStatisticId = importState.ImportStatisticId;
        MapContactAddress(entity, record);
        MapDomainOfInfluenceData(importState, record, entity);
        MapStaticAssignments(entity);
    }

    /// <summary>
    /// Builds the contact address lines of a record
    /// <seealso cref="MapContactAddress"/>.
    /// </summary>
    /// <param name="record">The csv record.</param>
    /// <returns>A list of contact address lines, always contains 7 items/lines, some may be null strings.</returns>
    private static IReadOnlyList<string?> BuildContactAddressLines(CobraPersonCsvRecord record)
    {
        // replicate the address line logic from the eai-business-charon data conversion tool
        // (eCH-0045.Transformer/VotingPlaceHelper.cs#L64)
        // address line 1 is always first and official name
        // line 7 always the country
        // line 2-6 are filled with the contact address lines 1-7 with duplicates and empty lines removed
        // if there are more than 5 lines filled, the additional lines are not considered.
        const int maxLines = 5;
        var lines = new[]
            {
                record.ContactAddressLine1,
                record.ContactAddressLine2,
                record.ContactAddressLine3,
                record.ContactAddressLine4,
                record.ContactAddressLine5,
                record.ContactAddressLine6,
                record.ContactAddressLine7,
            }
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct()
            .Take(maxLines)
            .ToList();

        while (lines.Count < maxLines)
        {
            lines.Add(null);
        }

        lines.Insert(0, $"{record.FirstName} {record.OfficialName}");
        lines.Add(record.ResidenceCountry ?? string.Empty);
        return lines;
    }

    private static void MapContactAddress(PersonEntity entity, CobraPersonCsvRecord record)
    {
        var lines = BuildContactAddressLines(record);
        entity.ContactAddressLine1 = lines[0];
        entity.ContactAddressLine2 = lines[1];
        entity.ContactAddressLine3 = lines[2];
        entity.ContactAddressLine4 = lines[3];
        entity.ContactAddressLine5 = lines[4];
        entity.ContactAddressLine6 = lines[5];
        entity.ContactAddressLine7 = lines[6];
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
        CobraPersonCsvRecord record,
        PersonEntity entity)
    {
        // Domain of Influence (OG)
        if (!string.IsNullOrWhiteSpace(record.OriginName1) && !string.IsNullOrWhiteSpace(record.OnCanton1))
        {
            entity.PersonDois.Add(new()
            {
                Id = Guid.NewGuid(),
                PersonId = entity.Id,
                DomainOfInfluenceType = DomainOfInfluenceType.Og,
                Name = record.OriginName1 ?? string.Empty,
                Canton = record.OnCanton1 ?? string.Empty,
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
