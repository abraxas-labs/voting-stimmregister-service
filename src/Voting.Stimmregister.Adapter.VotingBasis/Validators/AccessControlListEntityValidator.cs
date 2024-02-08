// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using FluentValidation;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.VotingBasis.Validators;

/// <summary>
/// Validator for the <see cref="AccessControlListDoiEntity"/> for validation reports.
/// </summary>
internal class AccessControlListEntityValidator : AbstractValidator<AccessControlListDoiEntity>
{
    private const int MaxBfsStringLength = 8;
    private readonly int _maxStringLength = 150;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessControlListEntityValidator"/> class with fluent validation rule set.
    /// </summary>
    internal AccessControlListEntityValidator()
    {
        InitializeRuleset();
    }

    /// <summary>
    /// Initializes the fluent validation rule set.
    /// </summary>
    private void InitializeRuleset()
    {
        RuleFor(v => v.Name).NotEmpty().MaximumLength(_maxStringLength);
        RuleFor(v => v.Bfs).NotEmpty().MaximumLength(MaxBfsStringLength);
        RuleFor(v => v.TenantName).NotEmpty().MaximumLength(_maxStringLength);
        RuleFor(v => v.TenantId).NotEmpty().MaximumLength(_maxStringLength);
        RuleFor(v => v.Type).IsInEnum().NotEqual(DomainOfInfluenceType.Unspecified);
        RuleFor(v => v.Canton).IsInEnum();
    }
}
