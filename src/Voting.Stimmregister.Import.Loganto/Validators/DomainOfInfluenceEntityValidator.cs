// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using FluentValidation;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Import.Loganto.Validators;

/// <summary>
/// Validator for the <see cref="DomainOfInfluenceEntity"/> for validation reports.
/// </summary>
public class DomainOfInfluenceEntityValidator : AbstractValidator<DomainOfInfluenceEntity>
{
    private const int MaxCircleIdStringLength = 10;

    private readonly int _maxStringLength;
    private readonly int _maxIntNumber;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainOfInfluenceEntityValidator"/> class with fluent validation rulesets.
    /// </summary>
    /// <param name="doiImportConfig">The domain of influence import config.</param>
    public DomainOfInfluenceEntityValidator(DomainOfInfluenceImportConfig doiImportConfig)
    {
        _maxStringLength = doiImportConfig.EntityValidationMaxStringLength != 0
            ? doiImportConfig.EntityValidationMaxStringLength
            : 150;

        _maxIntNumber = doiImportConfig.EntityValidationMaxIntNumber != 0
            ? doiImportConfig.EntityValidationMaxIntNumber
            : 100000000;

        InitializeRuleset();
    }

    /// <summary>
    /// Initializes the fluent validation ruleset.
    /// </summary>
    private void InitializeRuleset()
    {
        RuleFor(v => v.MunicipalityId).NotEmpty().GreaterThan(0).LessThan(9999);
        RuleFor(v => v.DomainOfInfluenceId).NotEmpty().LessThan(_maxIntNumber);
        RuleFor(v => v.Street).NotEmpty().MaximumLength(_maxStringLength);
        RuleFor(v => v.HouseNumber).NotEmpty().MaximumLength(6);
        RuleFor(v => v.HouseNumberAddition).MaximumLength(6);
        RuleFor(v => v.SwissZipCode).NotEmpty().GreaterThan(999).LessThan(10000);
        RuleFor(v => v.Town).MaximumLength(_maxStringLength);
        RuleFor(v => v.PoliticalCircleId).MaximumLength(MaxCircleIdStringLength);
        RuleFor(v => v.PoliticalCircleName).MaximumLength(_maxStringLength).MustNotContainInvalidCircleReference();
        RuleFor(v => v.CatholicChurchCircleId).MaximumLength(MaxCircleIdStringLength);
        RuleFor(v => v.CatholicChurchCircleName).MaximumLength(_maxStringLength).MustNotContainInvalidCircleReference();
        RuleFor(v => v.EvangelicChurchCircleId).MaximumLength(MaxCircleIdStringLength);
        RuleFor(v => v.EvangelicChurchCircleName).MaximumLength(_maxStringLength).MustNotContainInvalidCircleReference();
        RuleFor(v => v.SchoolCircleId).MaximumLength(MaxCircleIdStringLength);
        RuleFor(v => v.SchoolCircleName).MaximumLength(_maxStringLength).MustNotContainInvalidCircleReference();
        RuleFor(v => v.TrafficCircleId).MaximumLength(MaxCircleIdStringLength);
        RuleFor(v => v.TrafficCircleName).MaximumLength(_maxStringLength).MustNotContainInvalidCircleReference();
        RuleFor(v => v.ResidentialDistrictCircleId).MaximumLength(MaxCircleIdStringLength);
        RuleFor(v => v.ResidentialDistrictCircleName).MaximumLength(_maxStringLength).MustNotContainInvalidCircleReference();
        RuleFor(v => v.PeopleCouncilCircleId).MaximumLength(MaxCircleIdStringLength);
        RuleFor(v => v.PeopleCouncilCircleName).MaximumLength(_maxStringLength).MustNotContainInvalidCircleReference();
    }
}
