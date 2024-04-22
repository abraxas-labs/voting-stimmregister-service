// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using FluentValidation;

namespace Voting.Stimmregister.Core.Validators;

public static class PersonValidationRuleBuilder
{
    // TODO: VOTING-2714 localization for validatoin messages
    public static IRuleBuilderOptions<T, DateOnly> DateOnlyMustNotHaveMinValue<T>(this IRuleBuilder<T, DateOnly> ruleBuilder)
    {
        return ruleBuilder.Must(dateOnly => dateOnly > DateOnly.MinValue)
            .WithMessage("Das Datum von {PropertyName} ist kein gültiger Wert.");
    }

    public static IRuleBuilderOptions<T, DateOnly?> DateOnlyMustNotHaveMinValue<T>(this IRuleBuilder<T, DateOnly?> ruleBuilder)
    {
        return ruleBuilder.Must(dateOnly => dateOnly == null || dateOnly > DateOnly.MinValue)
            .WithMessage("Das Datum von {PropertyName} ist kein gültiger Wert.");
    }

    public static IRuleBuilderOptions<T, string?> MustNotContainWhitespacesOnly<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(u => string.IsNullOrEmpty(u) || u!.Trim().Length > 0)
            .WithMessage("Das Feld enthält ungültige Whitespaces.");
    }
}
