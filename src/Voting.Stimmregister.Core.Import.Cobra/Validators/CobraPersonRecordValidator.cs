// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using FluentValidation;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Models.Import;

namespace Voting.Stimmregister.Core.Import.Cobra.Validators;

/// <summary>
/// Validator for the <see cref="CobraPersonCsvRecord"/> to ensure import records appear as expected.
/// </summary>
public class CobraPersonRecordValidator : AbstractValidator<CobraPersonCsvRecord>
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
        RuleFor(p => p.DateOfBirth).NotEmpty().MaximumLength(10);
        RuleFor(p => p.MunicipalityId).NotEmpty().LessThan(10000);
        RuleFor(p => p.VotingMunicipality).Equal(p => p.MunicipalityId.ToString());
        RuleFor(p => p.BfsNumber).Equal(p => p.MunicipalityId.ToString());
    }

    private void InitializeMaxLimitationsOnly()
    {
        RuleFor(p => p.Vn).LessThan(7570000000000);
        RuleFor(p => p.Salutation10).MaximumLength(_maxStringLength);
        RuleFor(p => p.Heimatgemeinden).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressLine1).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressLine2).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressLine3).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressLine4).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressLine5).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressLine6).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressLine7).MaximumLength(_maxStringLength);
        RuleFor(p => p.ResidenceCountry).MaximumLength(_maxStringLength);
        RuleFor(p => p.DistributionKey).MaximumLength(_maxStringLength);
        RuleFor(p => p.DistributionKey).MaximumLength(_maxStringLength);
        RuleFor(p => p.FederalVotingRight).MaximumLength(1);
        RuleFor(p => p.CantonalVotingRight).MaximumLength(1);
        RuleFor(p => p.CommunalVotingRight).MaximumLength(1);
        RuleFor(p => p.EntryValidityEndDate).MaximumLength(_maxStringLength);
        RuleFor(p => p.FrankingCode).MaximumLength(1);
        RuleFor(p => p.LanguageOfCorrespondence).MaximumLength(2);
        RuleFor(p => p.Language).MaximumLength(_maxStringLength);
        RuleFor(p => p.Aufenthaltsland).MaximumLength(_maxStringLength);
        RuleFor(p => p.Sex).MaximumLength(10);
        RuleFor(p => p.Salutation).MaximumLength(_maxStringLength);
        RuleFor(p => p.Title).MaximumLength(_maxStringLength);
        RuleFor(p => p.LetterSalutation).MaximumLength(_maxStringLength);
        RuleFor(p => p.PlaceOfBirth).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressStreet).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressHouseNumber).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressHouseNumberAddition).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressPostOfficeBoxText).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressZipCode).MaximumLength(20);
        RuleFor(p => p.ContactAddressTown).MaximumLength(_maxStringLength);
        RuleFor(p => p.ContactAddressLocality).MaximumLength(_maxStringLength);
        RuleFor(p => p.BfsCountry).MaximumLength(4);
        RuleFor(p => p.ModeOfShipment).MaximumLength(1);
        RuleFor(p => p.OriginName1).MaximumLength(_maxStringLength);
        RuleFor(p => p.OnCanton1).MaximumLength(2);
        RuleFor(p => p.MoveInComesFrom).MaximumLength(_maxStringLength);
        RuleFor(p => p.SwissAbroadEvotingFlag).NotNull();
    }
}
