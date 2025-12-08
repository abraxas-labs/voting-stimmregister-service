// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Stimmregister.Core.Import.Cobra.Tg.Utils;
using Voting.Stimmregister.Domain.Enums;
using Xunit;

namespace Voting.Stimmregister.Core.Import.Cobra.Unit.Tests.Utils;

public class CobraTgUtilUnitTest
{
    [Theory]
    [InlineData("21.09.1960 00:00:00", 21, 9, 1960, false)]
    public void WhenValidDateOfBirthString_ShouldReturnRightDate(
        string dateString,
        int expectedDay,
        int expectedMonth,
        int expectedYear,
        bool expectedDateAdjustedState)
    {
        var expectedDateOnly = new DateOnly(expectedYear, expectedMonth, expectedDay);
        var parsedDateOnly = CobraUtil.ConvertDateOfBirth(dateString, out var dateAdjusted);
        Assert.Equal(expectedDateOnly, parsedDateOnly);
        Assert.Equal(expectedDateAdjustedState, dateAdjusted);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12.31.2022")]
    [InlineData("29.02.2021")]
    [InlineData("01.02.2021")]
    public void WhenInvalidDateOfBirthString_ShouldReturnDateMinValueAdjusted(string dateString)
    {
        var parsedDateOnly = CobraUtil.ConvertDateOfBirth(dateString, out var dateAdjusted);
        Assert.True(dateAdjusted);
        Assert.Equal(DateOnly.MinValue, parsedDateOnly);
    }

    [Theory]
    [InlineData("Herr", SexType.Male)]
    [InlineData("Frau", SexType.Female)]
    [InlineData("Her", SexType.Undefined)]
    [InlineData("", SexType.Undefined)]
    [InlineData(" ", SexType.Undefined)]
    [InlineData(null, SexType.Undefined)]
    public void WhenSalutationAsInput_ShouldReturnRightSexEnum(string? salutation, SexType expectedSexEnum)
    {
        var convertedSexEnum = CobraUtil.ConvertSalutationToSexType(salutation);
        Assert.Equal(convertedSexEnum, expectedSexEnum);
    }
}
