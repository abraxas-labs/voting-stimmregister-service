// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Stimmregister.Core.Import.Cobra.Utils;
using Voting.Stimmregister.Domain.Enums;
using Xunit;

namespace Voting.Stimmregister.Core.Import.Cobra.Unit.Tests.Utils;

public class CobraUtilUnitTest
{
    [Theory]
    [InlineData("21.09.1960", 21, 9, 1960, false)]
    public void WhenValidDateSting_ShouldReturnRightDate(
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
    public void WhenInvalidDateSting_ShouldReturnDateMinValueAdjusted(string dateString)
    {
        var parsedDateOnly = CobraUtil.ConvertDateOfBirth(dateString, out var dateAdjusted);
        Assert.True(dateAdjusted);
        Assert.Equal(DateOnly.MinValue, parsedDateOnly);
    }

    [Theory]
    [InlineData("männlich", SexType.Male)]
    [InlineData("weiblich", SexType.Female)]
    [InlineData("", SexType.Undefined)]
    [InlineData(" ", SexType.Undefined)]
    [InlineData(null, SexType.Undefined)]
    public void WhenLogantoSexCode_ShouldReturnRightSexEnum(string? sexString, SexType expectedSexEnum)
    {
        var convertedSexEnum = CobraUtil.ConvertSexType(sexString);
        Assert.Equal(convertedSexEnum, expectedSexEnum);
    }
}
