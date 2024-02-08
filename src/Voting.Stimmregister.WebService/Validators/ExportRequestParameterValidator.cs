// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using FluentValidation;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.WebService.Validators;

/// <summary>
/// Validator for the <see cref="PersonSearchParametersExportModel"/> to ensure export request parameters are valid.
/// </summary>
public class ExportRequestParameterValidator : AbstractValidator<PersonSearchParametersExportModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExportRequestParameterValidator"/> class with fluent validation rulesets.
    /// </summary>
    public ExportRequestParameterValidator()
    {
        RuleFor(x => x.Criteria).NotEmpty().When(x => !x.FilterId.HasValue && !x.VersionId.HasValue);
        RuleFor(x => x.FilterId).NotNull().When(x => !x.VersionId.HasValue && x.Criteria.Count == 0);
        RuleFor(x => x.VersionId).NotNull().When(x => !x.FilterId.HasValue && x.Criteria.Count == 0);
    }
}
