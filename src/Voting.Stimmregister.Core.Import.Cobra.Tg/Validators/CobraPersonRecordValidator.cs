// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using FluentValidation;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Models.Import;

namespace Voting.Stimmregister.Core.Import.Cobra.Tg.Validators;

/// <summary>
/// Validator for the <see cref="CobraTgPersonCsvRecord"/> to ensure import records appear as expected.
/// </summary>
public class CobraPersonRecordValidator : AbstractValidator<CobraTgPersonCsvRecord>
{
    private readonly int _maxStringLength;

    /// <summary>
    /// Initializes a new instance of the <see cref="CobraPersonRecordValidator"/> class with fluent validation rulesets.
    /// </summary>
    /// <param name="personImportConfig">The person import config.</param>
    public CobraPersonRecordValidator(PersonImportConfig personImportConfig)
    {
        _maxStringLength = personImportConfig.RecordValidationMaxStringLength == 0
            ? 150
            : personImportConfig.RecordValidationMaxStringLength;

        InitializeRuleset();
    }

    /// <summary>
    /// Initializes the fluent validation rulesets.
    /// </summary>
    private void InitializeRuleset()
    {
        InitializeEssentials();
        InitializeMaxLimitationsOnly();
    }

    private void InitializeEssentials()
    {
        RuleFor(p => p.SourceSystemId).NotEmpty().MaximumLength(_maxStringLength);
        RuleFor(p => p.FirstName).NotEmpty().MaximumLength(_maxStringLength);
        RuleFor(p => p.OfficialName).NotEmpty().MaximumLength(_maxStringLength);
        RuleFor(p => p.DateOfBirth).NotEmpty().MaximumLength(19);
        RuleFor(p => p.MunicipalityId).NotEmpty().LessThan(10000);
    }

    private void InitializeMaxLimitationsOnly()
    {
        RuleFor(p => p.Vn).LessThan(7570000000000);
        RuleFor(p => p.Title).MaximumLength(_maxStringLength);
        RuleFor(p => p.Salutation).MaximumLength(_maxStringLength);
        RuleFor(p => p.ResidenceCountry).MaximumLength(_maxStringLength);
        RuleFor(p => p.ResidenceCountryShort).MaximumLength(_maxStringLength);
        RuleFor(p => p.LanguageOfCorrespondence).MaximumLength(2);
        RuleFor(p => p.Salutation).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressStreet).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressHouseNumber).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressExtensionLine1).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressExtensionLine2).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressPostOfficeBoxText).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressZipCode).MaximumLength(20);
        RuleFor(p => p.ContactAddressTown).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressLocality).MaximumLength(_maxStringLength);
        RuleFor(p => p.ModeOfShipment).MaximumLength(_maxStringLength);
        RuleFor(p => p.OriginName1).MaximumLength(_maxStringLength);
        RuleFor(p => p.Active).MaximumLength(_maxStringLength);
        RuleFor(p => p.CombinedStreetAndHouseNumber).MaximumLength(_maxStringLength);
        RuleFor(p => p.CombinedPostalCodeandLocality).MaximumLength(_maxStringLength);
    }
}
