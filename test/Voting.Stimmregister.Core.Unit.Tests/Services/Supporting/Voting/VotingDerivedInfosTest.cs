// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using FluentAssertions;
using Voting.Stimmregister.Core.Services.Supporting.Voting;
using Voting.Stimmregister.Domain.Models;
using Xunit;

namespace Voting.Stimmregister.Core.Unit.Tests.Services.Supporting.Voting;

public class VotingDerivedInfosTest
{
    private readonly VotingDerivedInfos _testee;

    public VotingDerivedInfosTest()
    {
        _testee = new VotingDerivedInfos();
    }

    [Theory]
    [InlineData("CH")]
    [InlineData("ch")]
    public void ComputeInfos_ShouldSetIsNationalityValidForVotingRightsCorrectlyWhenInputIsRight(string country)
    {
        // Arrange
        var person = new PersonEntityModel { Country = country };
        var referenceKeyDate = new DateOnly(2025, 1, 1);

        // Act
        _testee.ComputeInfos(person, referenceKeyDate);

        // Assert
        person.IsNationalityValidForVotingRights.Should().BeTrue();
        person.IsBirthDateValidForVotingRights.Should().BeFalse();
        person.IsVotingAllowed.Should().BeFalse();
    }

    [Theory]
    [InlineData("DE")] // Invalid nationality
    [InlineData(null)] // Null nationality
    [InlineData("")] // Empty string nationality
    public void ComputeInfos_ShouldSetIsNationalityValidForVotingRightsCorrectlyWhenInputIsWrong(string? country)
    {
        // Arrange
        var person = new PersonEntityModel { Country = country };
        var referenceKeyDate = new DateOnly(2025, 1, 1);

        // Act
        _testee.ComputeInfos(person, referenceKeyDate);

        // Assert
        person.IsNationalityValidForVotingRights.Should().BeFalse();
        person.IsBirthDateValidForVotingRights.Should().BeFalse();
        person.IsVotingAllowed.Should().BeFalse();
    }

    [Theory]
    [InlineData("2000-01-01")]
    [InlineData("1999-12-31")]
    [InlineData("1950-01-01")]
    public void ComputeInfos_ShouldSetIsBirthDateValidForVotingRightsWhen18AndOlder(string dateOfBirth)
    {
        // Arrange
        var person = new PersonEntityModel { DateOfBirth = DateOnly.Parse(dateOfBirth, System.Globalization.CultureInfo.InvariantCulture) };

        var referenceKeyDate = new DateOnly(2018, 1, 1);

        // Act
        _testee.ComputeInfos(person, referenceKeyDate);

        // Assert
        person.IsBirthDateValidForVotingRights.Should().BeTrue();
        person.IsNationalityValidForVotingRights.Should().BeFalse();
        person.IsVotingAllowed.Should().BeFalse();
    }

    [Theory]
    [InlineData("2000-01-02")]
    [InlineData("2010-12-31")]
    [InlineData("2050-01-01")]
    public void ComputeInfos_ShouldSetIsBirthDateValidForVotingRightsWhenYoungerThan18(string dateOfBirth)
    {
        // Arrange
        var person = new PersonEntityModel { DateOfBirth = DateOnly.Parse(dateOfBirth, System.Globalization.CultureInfo.InvariantCulture) };

        var referenceKeyDate = new DateOnly(2018, 1, 1);

        // Act
        _testee.ComputeInfos(person, referenceKeyDate);

        // Assert
        person.IsBirthDateValidForVotingRights.Should().BeFalse();
        person.IsNationalityValidForVotingRights.Should().BeFalse();
        person.IsVotingAllowed.Should().BeFalse();
    }

    [Fact]
    public void ComputeInfos_ShouldSetIsVotingAllowedWhenAllConditionsAreMet()
    {
        // Arrange
        var person = new PersonEntityModel
        {
            Country = "CH", // Valid nationality
            DateOfBirth = new DateOnly(2000, 1, 1), // Person is 18 years old on the reference date
            MoveInArrivalDate = new DateOnly(2017, 1, 1), // Valid move-in date
            RestrictedVotingAndElectionRightFederation = false, // No restrictions
        };
        var referenceKeyDate = new DateOnly(2018, 1, 1);

        // Act
        _testee.ComputeInfos(person, referenceKeyDate);

        // Assert
        person.IsVotingAllowed.Should().BeTrue();
        person.IsNationalityValidForVotingRights.Should().BeTrue();
        person.IsBirthDateValidForVotingRights.Should().BeTrue();
    }

    [Theory]
    [InlineData("CH", "2005-01-01", "2017-01-01", false)] // Underage
    [InlineData("DE", "2000-01-01", "2017-01-01", false)] // Invalid nationality
    [InlineData("CH", "2000-01-01", "2019-01-01", false)] // Move-in date after reference date
    [InlineData("CH", "2000-01-01", "2017-01-01", true)] // RestrictedVotingAndElectionRightFederation
    public void ComputeInfos_ShouldSetIsVotingAllowedCorrectlyWhenOneConditionIsNotMet(string country, string dateOfBirth, string moveInArrivalDate, bool restrictedVotingAndElectionRightFederation)
    {
        // Arrange
        var person = new PersonEntityModel
        {
            Country = country,
            DateOfBirth = DateOnly.Parse(dateOfBirth, System.Globalization.CultureInfo.InvariantCulture),
            MoveInArrivalDate = DateOnly.Parse(moveInArrivalDate, System.Globalization.CultureInfo.InvariantCulture),
            RestrictedVotingAndElectionRightFederation = restrictedVotingAndElectionRightFederation,
        };
        var referenceKeyDate = new DateOnly(2018, 1, 1);

        // Act
        _testee.ComputeInfos(person, referenceKeyDate);

        // Assert
        person.IsVotingAllowed.Should().BeFalse();
    }
}
