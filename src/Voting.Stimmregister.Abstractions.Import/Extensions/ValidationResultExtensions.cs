// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Linq;
using FluentValidation.Results;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Import.Extensions;

public static class ValidationResultExtensions
{
    public static RecordValidationErrorModel ToRecordValidationErrorModel(this ValidationResult validationResult, int recordNumber, string recordIdentifier)
    {
        return new RecordValidationErrorModel
        {
            RecordNumber = recordNumber,
            RecordIdentifier = recordIdentifier,
            Fields = validationResult
                .Errors
                .GroupBy(e => e.PropertyName)
                .Select(grouping => new FieldValidationErrorModel
                {
                    FieldName = grouping.Key,
                    Errors = grouping.Select(x => x.ErrorMessage).ToList(),
                })
                .ToList(),
        };
    }
}
