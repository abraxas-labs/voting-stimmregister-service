// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Text.RegularExpressions;
using FluentValidation;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Models.Import;

namespace Voting.Stimmregister.Core.Import.Loganto.Validators;

/// <summary>
/// Validator for the <see cref="LogantoPersonCsvRecord"/> to ensure import records appear as expected.
/// </summary>
public class LogantoPersonRecordValidator : AbstractValidator<LogantoPersonCsvRecord>
{
    private static readonly Regex _nonControlCharactersRegex = new(@"^(?:(?!\\x|\\0|\0).)*$", RegexOptions.None, TimeSpan.FromMilliseconds(500));

    private readonly int _maxStringLength;
    private readonly int _maxIntNumber;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogantoPersonRecordValidator"/> class with fluent validation rule sets.
    /// </summary>
    /// <param name="personImportConfig">The person import config.</param>
    public LogantoPersonRecordValidator(PersonImportConfig personImportConfig)
    {
        _maxStringLength = personImportConfig.RecordValidationMaxStringLength != 0
            ? personImportConfig.RecordValidationMaxStringLength
            : 150;

        _maxIntNumber = personImportConfig.RecordValidationMaxIntNumber != 0
            ? personImportConfig.RecordValidationMaxIntNumber
            : 100000000;

        InitializeRuleSet();
    }

    /// <summary>
    /// Initializes the fluent validation rule sets.
    /// </summary>
    private void InitializeRuleSet()
    {
        InitializeEssentials();
        InitializeMaxLimitationsOnly();
    }

    private void InitializeEssentials()
    {
        RuleFor(p => p.SourceSystemId).NotEmpty().MaximumLength(_maxStringLength);
        RuleFor(p => p.FirstName).NotEmpty().MaximumLength(_maxStringLength);
        RuleFor(p => p.OfficialName).NotEmpty().MaximumLength(_maxStringLength);
        RuleFor(p => p.DateOfBirth).NotEmpty().MaximumLength(10);
        RuleFor(p => p.MunicipalityId).NotEmpty().LessThan(10000);
    }

    private void InitializeMaxLimitationsOnly()
    {
        RuleFor(p => p.Vn).LessThan(7570000000000);
        RuleFor(p => p.DomainOfInfluenceId).LessThan(_maxIntNumber);
        RuleFor(p => p.Sex).MaximumLength(1);
        RuleFor(p => p.Religion).MaximumLength(5);
        RuleFor(p => p.OriginalName).MaximumLength(_maxStringLength);
        RuleFor(p => p.AliasName).MaximumLength(_maxStringLength);
        RuleFor(p => p.AliasName).MaximumLength(_maxStringLength);
        RuleFor(p => p.OtherName).MaximumLength(_maxStringLength);
        RuleFor(p => p.CallName).MaximumLength(_maxStringLength);
        RuleFor(p => p.Country).MaximumLength(_maxStringLength);
        RuleFor(p => p.MunicipalityName).MaximumLength(_maxStringLength);
        RuleFor(p => p.OriginName1).MaximumLength(_maxStringLength);
        RuleFor(p => p.OriginName2).MaximumLength(_maxStringLength);
        RuleFor(p => p.OriginName3).MaximumLength(_maxStringLength);
        RuleFor(p => p.OriginName4).MaximumLength(_maxStringLength);
        RuleFor(p => p.OriginName5).MaximumLength(_maxStringLength);
        RuleFor(p => p.OriginName6).MaximumLength(_maxStringLength);
        RuleFor(p => p.OriginName7).MaximumLength(_maxStringLength);
        RuleFor(p => p.OnCanton1).MaximumLength(2);
        RuleFor(p => p.OnCanton2).MaximumLength(2);
        RuleFor(p => p.OnCanton3).MaximumLength(2);
        RuleFor(p => p.OnCanton4).MaximumLength(2);
        RuleFor(p => p.OnCanton5).MaximumLength(2);
        RuleFor(p => p.OnCanton6).MaximumLength(2);
        RuleFor(p => p.OnCanton7).MaximumLength(2);
        RuleFor(p => p.ResidencePermit).MaximumLength(_maxStringLength);
        RuleFor(p => p.ResidencePermitValidFrom).MaximumLength(10);
        RuleFor(p => p.ResidencePermitValidTill).MaximumLength(10);
        RuleFor(p => p.ResidenceEntryDate).MaximumLength(10);
        RuleFor(p => p.ContactAddressExtensionLine1).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressExtensionLine2).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressStreet).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressHouseNumber).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressDwellingNumber).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressPostOfficeBoxText).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressTown).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressSwissZipCode).MaximumLength(15);
        RuleFor(p => p.ContactCantonAbbreviation).MaximumLength(2);
        RuleFor(p => p.ContactAddressCountryIdIso2).MaximumLength(2);
        RuleFor(p => p.ResidenceAddressExtensionLine1).MaximumLength(_maxStringLength);
        RuleFor(p => p.ResidenceAddressExtensionLine2).MaximumLength(_maxStringLength);
        RuleFor(p => p.ResidenceAddressStreet).MaximumLength(_maxStringLength);
        RuleFor(p => p.ResidenceAddressHouseNumber).MaximumLength(_maxStringLength);
        RuleFor(p => p.ResidenceAddressDwellingNumber).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressPostOfficeBoxText).MaximumLength(_maxStringLength);
        RuleFor(p => p.ResidenceAddressTown).MaximumLength(_maxStringLength);
        RuleFor(p => p.ResidenceAddressSwissZipCode).MaximumLength(4);
        RuleFor(p => p.ResidenceCantonAbbreviation).MaximumLength(2);
        RuleFor(p => p.MoveInArrivalDate).MaximumLength(10);
        RuleFor(p => p.MoveInMunicipalityName).MaximumLength(_maxStringLength);
        RuleFor(p => p.MoveInCantonAbbreviation).MaximumLength(_maxStringLength);
        RuleFor(p => p.MoveInComeFrom).MaximumLength(_maxStringLength).Matches(_nonControlCharactersRegex);
        RuleFor(p => p.MoveInCountryNameShort).MaximumLength(_maxStringLength);
        RuleFor(p => p.PoliticalCircleId).MaximumLength(_maxStringLength);
        RuleFor(p => p.PoliticalCircleName).MaximumLength(_maxStringLength);
        RuleFor(p => p.CatholicCircleId).MaximumLength(_maxStringLength);
        RuleFor(p => p.CatholicCircleName).MaximumLength(_maxStringLength);
        RuleFor(p => p.EvangelicCircleId).MaximumLength(_maxStringLength);
        RuleFor(p => p.EvangelicCircleName).MaximumLength(_maxStringLength);
        RuleFor(p => p.SchoolCircleId).MaximumLength(_maxStringLength);
        RuleFor(p => p.SchoolCircleName).MaximumLength(_maxStringLength);
        RuleFor(p => p.TrafficCircleId).MaximumLength(_maxStringLength);
        RuleFor(p => p.TrafficCircleName).MaximumLength(_maxStringLength);
        RuleFor(p => p.ResidentialDistrictCircleId).MaximumLength(_maxStringLength);
        RuleFor(p => p.ResidentialDistrictCircleName).MaximumLength(_maxStringLength);
        RuleFor(p => p.PeopleCircleId).MaximumLength(_maxStringLength);
        RuleFor(p => p.PeopleCircleName).MaximumLength(_maxStringLength);
        RuleFor(p => p.LanguageOfCorrespondence).MaximumLength(_maxStringLength);
    }
}
