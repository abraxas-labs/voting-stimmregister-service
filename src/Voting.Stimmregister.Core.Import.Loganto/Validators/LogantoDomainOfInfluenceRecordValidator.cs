// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using FluentValidation;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Models.Import;

namespace Voting.Stimmregister.Core.Import.Loganto.Validators;

/// <summary>
/// Validator for the <see cref="LogantoDomainOfInfluenceCsvRecord"/> to ensure import records appear as expected.
/// </summary>
public class LogantoDomainOfInfluenceRecordValidator : AbstractValidator<LogantoDomainOfInfluenceCsvRecord>
{
    private readonly int _maxStringLength;
    private readonly int _maxIntNumber;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogantoDomainOfInfluenceRecordValidator"/> class with fluent validation rulesets.
    /// </summary>
    /// <param name="doiImportConfig">The domain of influence import config.</param>
    public LogantoDomainOfInfluenceRecordValidator(DomainOfInfluenceImportConfig doiImportConfig)
    {
        _maxStringLength = doiImportConfig.RecordValidationMaxStringLength != 0
            ? doiImportConfig.RecordValidationMaxStringLength
            : 250;

        _maxIntNumber = doiImportConfig.RecordValidationMaxIntNumber != 0
            ? doiImportConfig.RecordValidationMaxIntNumber
            : 100000000;

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
        RuleFor(v => v.MunicipalityId).NotEmpty().GreaterThan(0).LessThan(10000);
        RuleFor(v => v.DomainOfInfluenceId).NotEmpty().LessThan(_maxIntNumber);
        RuleFor(v => v.Street).NotEmpty().MaximumLength(_maxStringLength);
        RuleFor(v => v.HouseNumber).NotEmpty().MaximumLength(_maxStringLength);
        RuleFor(v => v.SwissZipCode).NotEmpty().GreaterThan(999).LessThan(10000);
    }

    private void InitializeMaxLimitationsOnly()
    {
        RuleFor(v => v.HouseNumberAddition).MaximumLength(_maxStringLength);
        RuleFor(v => v.Town).MaximumLength(_maxStringLength);
        RuleFor(v => v.PoliticalCircleId).MaximumLength(_maxStringLength);
        RuleFor(v => v.PoliticalCircleName).MaximumLength(_maxStringLength);
        RuleFor(v => v.CatholicChurchCircleId).MaximumLength(_maxStringLength);
        RuleFor(v => v.CatholicChurchCircleName).MaximumLength(_maxStringLength);
        RuleFor(v => v.EvangelicChurchCircleId).MaximumLength(_maxStringLength);
        RuleFor(v => v.EvangelicChurchCircleName).MaximumLength(_maxStringLength);
        RuleFor(v => v.SchoolCircleId).MaximumLength(_maxStringLength);
        RuleFor(v => v.SchoolCircleName).MaximumLength(_maxStringLength);
        RuleFor(v => v.TrafficCircleId).MaximumLength(_maxStringLength);
        RuleFor(v => v.TrafficCircleName).MaximumLength(_maxStringLength);
        RuleFor(v => v.ResidentialDistrictCircleId).MaximumLength(_maxStringLength);
        RuleFor(v => v.ResidentialDistrictCircleName).MaximumLength(_maxStringLength);
        RuleFor(v => v.PeopleCouncilCircleId).MaximumLength(_maxStringLength);
        RuleFor(v => v.PeopleCouncilCircleName).MaximumLength(_maxStringLength);
    }
}
