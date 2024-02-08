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
    public void ShouldReturnExpectedResidenceAddressAvailability(string residenceStreet, bool expectedAvailability)
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
    public void ShouldReturnExpectedContactAddressAvailability(string contactStreet, string addressLine1, bool expectedAvailability)
    {
        var result = PersonUtil.HasContactAddress(new()
        {
            ContactAddressStreet = contactStreet,
            ContactAddressLine1 = addressLine1,
        });

        result.Should().Be(expectedAvailability);
    }
}
