// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using FluentAssertions;
using Voting.Stimmregister.Core.Import.Loganto.Utils;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models.Import;
using Xunit;

namespace Voting.Stimmregister.Core.Import.Loganto.Unit.Tests.Utils;

public class LogantoUtilUnitTest
{
    [Theory]
    [InlineData("21.09.1960", "21.09.1960")]
    [InlineData("**.12.2000", "01.12.2000")]
    [InlineData("**.**.2022", "01.01.2022")]
    public void WhenValidDateSting_ShouldReturnRightDate(string dateString, string expectedDateString)
    {
        var expectedDate = DateOnly.ParseExact(expectedDateString, "dd.MM.yyyy");
        var date = LogantoUtil.ConvertLogantoDate(dateString);
        Assert.Equal(expectedDate, date);
    }

    [Theory]
    [InlineData("")]
    [InlineData("xx.**.2000")]
    [InlineData("12.31.2022")]
    [InlineData("29.02.2021")]
    public void WhenInvalidDateSting_ShouldReturnNull(string dateString)
    {
        var date = LogantoUtil.ConvertLogantoDate(dateString);
        Assert.Null(date);
    }

    [Theory]
    [InlineData("21.09.1960", true, 21, 9, 1960, false)]
    [InlineData("**.12.2000", true, 1, 12, 2000, true)]
    [InlineData("**.**.2000", true, 1, 1, 2000, true)]
    [InlineData("**.11.2000", false, 30, 11, 2000, true)]
    [InlineData("**.**.2000", false, 31, 12, 2000, true)]
    [InlineData("**.02.2024", false, 29, 02, 2024, true)] // Leap year
    [InlineData("**..2000", true, 1, 1, 0001, true)] // expecting DateOnly.MinValue when parsing fails
    [InlineData("..2000", true, 1, 1, 0001, true)] // expecting DateOnly.MinValue when parsing fails
    public void WhenPassStringDate_ShouldReturnRightDateAdjustedState(
        string dateString,
        bool useFirstDayOrMonth,
        int expectedDay,
        int expectedMonth,
        int expectedYear,
        bool expectedDateAdjustedState)
    {
        var expectedDateOnly = new DateOnly(expectedYear, expectedMonth, expectedDay);
        var parsedDateOnly = LogantoUtil.ConvertLogantoDateOfBirth(dateString, useFirstDayOrMonth, out var dateAdjusted);
        Assert.Equal(expectedDateOnly, parsedDateOnly);
        Assert.Equal(expectedDateAdjustedState, dateAdjusted);
    }

    [Theory]
    [InlineData("M", SexType.Male)]
    [InlineData("W", SexType.Female)]
    [InlineData("", SexType.Undefined)]
    [InlineData("MW", SexType.Undefined)]
    [InlineData(null, SexType.Undefined)]
    public void WhenLogantoSexCode_ShouldReturnRightSexEnum(string? sexString, SexType expectedSexEnum)
    {
        var convertedSexEnum = LogantoUtil.ConvertLogantoSex(sexString);
        Assert.Equal(convertedSexEnum, expectedSexEnum);
    }

    [Fact]
    public void WhenVotingAdditionRightCodeC_ShouldSendVotingCardsToDomainOfInfluenceReturnAddress_ShouldApplyRule1()
    {
        var record = new LogantoPersonCsvRecord
        {
            VotingRightAddition = "C",
        };
        LogantoUtil.ShouldSendVotingCardsToDomainOfInfluenceReturnAddress(record, new HashSet<int>(), new HashSet<int>())
            .Should()
            .BeTrue();
    }

    [Fact]
    public void WhenVotingAdditionRightCodeP_ShouldSendVotingCardsToDomainOfInfluenceReturnAddress_ShouldApplyRule2()
    {
        var record = new LogantoPersonCsvRecord
        {
            VotingRightAddition = "P",
        };
        LogantoUtil.ShouldSendVotingCardsToDomainOfInfluenceReturnAddress(record, new HashSet<int>(), new HashSet<int>())
            .Should()
            .BeTrue();
    }

    [Fact]
    public void WhenVotingAdditionRightNoResidenceAddress_ShouldSendVotingCardsToDomainOfInfluenceReturnAddress_ShouldApplyRule3()
    {
        var record = new LogantoPersonCsvRecord
        {
            DomainOfInfluenceId = 1234,
        };
        LogantoUtil.ShouldSendVotingCardsToDomainOfInfluenceReturnAddress(record, new HashSet<int>(), new HashSet<int>())
            .Should()
            .BeTrue();
    }

    [Fact]
    public void WhenVotingAdditionRightNoResidenceAndContactAddressAndBfs_ShouldSendVotingCardsToDomainOfInfluenceReturnAddress_ShouldApplyRule3()
    {
        var record = new LogantoPersonCsvRecord
        {
            MunicipalityId = 1234,
            DomainOfInfluenceId = 1234,
        };
        LogantoUtil.ShouldSendVotingCardsToDomainOfInfluenceReturnAddress(record, new HashSet<int>(), new HashSet<int> { 1234 })
            .Should()
            .BeTrue();
    }

    [Fact]
    public void WhenVotingAdditionRightNoResidenceButContactAddressAndBfs_ShouldSendVotingCardsToDomainOfInfluenceReturnAddress_ShouldApplyRule3()
    {
        var record = new LogantoPersonCsvRecord
        {
            MunicipalityId = 1234,
            DomainOfInfluenceId = 1234,
            ContactAddressStreet = "street",
        };
        LogantoUtil.ShouldSendVotingCardsToDomainOfInfluenceReturnAddress(record, new HashSet<int>(), new HashSet<int> { 1234 })
            .Should()
            .BeFalse();
    }

    [Fact]
    public void WhenVotingAdditionRightNoDoiIdButResidenceAddressAndBfs_ShouldSendVotingCardsToDomainOfInfluenceReturnAddress_ShouldNotApplyRule4()
    {
        var record = new LogantoPersonCsvRecord
        {
            MunicipalityId = 1234,
            ResidenceAddressStreet = "Unbekannt",
        };
        LogantoUtil.ShouldSendVotingCardsToDomainOfInfluenceReturnAddress(record, new HashSet<int> { 1234 }, new HashSet<int>())
            .Should()
            .BeFalse();
    }

    [Fact]
    public void WhenVotingAdditionRightNoDoiIdButResidenceAddressAndOtherBfs_ShouldSendVotingCardsToDomainOfInfluenceReturnAddress_ShouldApplyRule4()
    {
        var record = new LogantoPersonCsvRecord
        {
            MunicipalityId = 1234,
            ResidenceAddressStreet = "Unbekannt",
        };
        LogantoUtil.ShouldSendVotingCardsToDomainOfInfluenceReturnAddress(record, new HashSet<int>(), new HashSet<int>())
            .Should()
            .BeTrue();
    }

    [Fact]
    public void WhenVotingAdditionRightCodePAndDoiIdButNoResidenceAddress_ShouldSendVotingCardsToDomainOfInfluenceReturnAddress_ShouldApplyRule2()
    {
        var record = new LogantoPersonCsvRecord
        {
            VotingRightAddition = "P",
            DomainOfInfluenceId = 123,
        };
        LogantoUtil.ShouldSendVotingCardsToDomainOfInfluenceReturnAddress(record, new HashSet<int>(), new HashSet<int>())
            .Should()
            .BeTrue();
    }
}
