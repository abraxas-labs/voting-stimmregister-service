// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using FluentAssertions;
using Voting.Stimmregister.Domain.Utils;
using Xunit;

namespace Voting.Stimmregister.Domain.Unit.Tests.Utils;

public class PersonUtilTests
{
    [Theory]
    [InlineData("Street1", true)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void ShouldReturnExpectedResidenceAddressAvailability(string? residenceStreet, bool expectedAvailability)
    {
        var result = PersonUtil.HasResidenceAddress(new()
        {
            ResidenceAddressStreet = residenceStreet,
        });

        result.Should().Be(expectedAvailability);
    }

    [Theory]
    [InlineData("Street1", null, true)]
    [InlineData(null, "Street1", true)]
    [InlineData("", "", false)]
    [InlineData(null, null, false)]
    public void ShouldReturnExpectedContactAddressAvailability(string? contactStreet, string? addressLine1, bool expectedAvailability)
    {
        var result = PersonUtil.HasContactAddress(new()
        {
            ContactAddressStreet = contactStreet,
            ContactAddressLine1 = addressLine1,
        });

        result.Should().Be(expectedAvailability);
    }

    [Theory]
    [InlineData("Street1", null, null, null)]
    [InlineData(null, "PostOfficeBox1", null, null)]
    [InlineData(null, null, "Line1", null)]
    [InlineData(null, null, null, "Line2")]
    [InlineData("Street1", "PostOfficeBox1", "Line1", "Line2")]
    public void ShouldReturnTrueWhenResidenceAddressComponentIsValid(string? street, string? postOfficeBoxText, string? line1, string? line2)
    {
        var result = PersonUtil.HasValidResidenceAddressComponent(new()
        {
            ResidenceAddressStreet = street,
            ResidenceAddressPostOfficeBoxText = postOfficeBoxText,
            ResidenceAddressExtensionLine1 = line1,
            ResidenceAddressExtensionLine2 = line2,
        });

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("Street1", null, null, null)]
    [InlineData(null, "PostOfficeBox1", null, null)]
    [InlineData(null, null, "Line1", null)]
    [InlineData(null, null, null, "Line2")]
    [InlineData("Street1", "PostOfficeBox1", "Line1", "Line2")]
    public void ShouldReturnTrueWhenContactAddressComponentIsValid(string? street, string? postOfficeBoxText, string? line1, string? line2)
    {
        var result = PersonUtil.HasValidContactAddressComponent(new()
        {
            ContactAddressStreet = street,
            ContactAddressPostOfficeBoxText = postOfficeBoxText,
            ContactAddressExtensionLine1 = line1,
            ContactAddressExtensionLine2 = line2,
        });

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(null, null, null, null)]
    [InlineData("", "", "", "")]
    public void ShouldReturnFalseWhenResidenceAddressComponentIsInvalid(string? street, string? postOfficeBoxText, string? line1, string? line2)
    {
        var result = PersonUtil.HasValidResidenceAddressComponent(new()
        {
            ResidenceAddressStreet = street,
            ResidenceAddressPostOfficeBoxText = postOfficeBoxText,
            ResidenceAddressExtensionLine1 = line1,
            ResidenceAddressExtensionLine2 = line2,
        });

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(null, null, null, null)]
    [InlineData("", "", "", "")]
    public void ShouldReturnFalseWhenContactAddressComponentIsInvalid(string? street, string? postOfficeBoxText, string? line1, string? line2)
    {
        var result = PersonUtil.HasValidContactAddressComponent(new()
        {
            ContactAddressStreet = street,
            ContactAddressPostOfficeBoxText = postOfficeBoxText,
            ContactAddressExtensionLine1 = line1,
            ContactAddressExtensionLine2 = line2,
        });

        result.Should().BeFalse();
    }
}
