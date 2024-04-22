// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using FluentValidation;

namespace Voting.Stimmregister.Core.Import.Loganto.Validators;

public static class DomainOfInfluenceValidationRuleBuilder
{
    // TODO: VOTING-2714 localization for validatoin messages
    public static IRuleBuilderOptions<T, string> MustNotContainInvalidCircleReference<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(literal => literal?.Contains("Kreis nicht gefunden", StringComparison.Ordinal) != true)
            .WithMessage("{PropertyName} enthält eine ungültige Kreis Referenz.");
    }
}
