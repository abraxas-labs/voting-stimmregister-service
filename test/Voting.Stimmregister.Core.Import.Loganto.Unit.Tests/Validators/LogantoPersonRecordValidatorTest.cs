// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using FluentValidation.TestHelper;
using Voting.Stimmregister.Core.Import.Loganto.Validators;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Models.Import;
using Xunit;

namespace Voting.Stimmregister.Core.Import.Loganto.Unit.Tests.Validators;

public class LogantoPersonRecordValidatorTest
{
    [Theory]
    [InlineData("Pre\0aaa")]
    [InlineData(@"Pre\0ccc")]
    [InlineData(@"Pre\xddd")]
    public void WhenMoveInComeFromContainsCommandCharacters_ShouldHaveValidationError(string moveInComeFrom)
    {
        var csvRecordModel = new LogantoPersonCsvRecord
        {
            MoveInComeFrom = moveInComeFrom,
        };
        var csvRecordValidator = GetNewCsvRecordValidator();
        var validationResult = csvRecordValidator.TestValidate(csvRecordModel);

        validationResult.ShouldHaveValidationErrorFor(record => record.MoveInComeFrom);
    }

    private static LogantoPersonRecordValidator GetNewCsvRecordValidator()
        => new(new PersonImportConfig());
}
