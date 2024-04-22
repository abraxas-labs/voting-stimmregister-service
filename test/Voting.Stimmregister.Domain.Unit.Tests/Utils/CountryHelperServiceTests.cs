// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using FluentAssertions;
using Voting.Stimmregister.Domain.Utils;
using Xunit;

namespace Voting.Stimmregister.Domain.Unit.Tests.Utils;

public class CountryHelperServiceTests
{
    private readonly ICountryHelperService _countryHelperService = new CountryHelperService();

    [Theory]
    [InlineData("GERMANY", "7777", "DE")]
    [InlineData("Switzerland", "8100", "CH")]
    [InlineData("UNITED STATES OF AMERICA", "8439", "US")]
    [InlineData("SRI LANKA", null, "LK")]
    public void WhenPassingValidCountryOrBfs_ShouldResolveIso2(string countryInput, string? countryNumberInput, string expectedCountryOutput)
    {
        var countryOutput = _countryHelperService.GetCountryTwoLetterIsoCode(countryInput, countryNumberInput);
        Assert.Equal(expectedCountryOutput, countryOutput);
    }

    [Theory]
    [InlineData("UNITED STATES OF AMERICA", null)]
    [InlineData(null, "1")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void WhenPassingInvalidCountryAndBfs_ShouldReturnNull(string? countryInput, string? countryNumberInput)
    {
        var countryOutput = _countryHelperService.GetCountryTwoLetterIsoCode(countryInput, countryNumberInput);
        Assert.Null(countryOutput);
    }

    [Theory]
    [InlineData("CH", 8100)]
    [InlineData("AQ", 8701)]
    public void WhenPassingInvalidCountryIso2Code_ShouldReturnNull(string countryTwoLetterCode, int expectedBfs)
    {
        var countryInfo = _countryHelperService.GetCountryInfo(countryTwoLetterCode);
        Assert.Equal(expectedBfs, countryInfo!.Id);
    }

    [Theory]
    [InlineData("AA")]
    [InlineData(null)]
    [InlineData("")]
    public void WhenPassingValidCountryIso2Code_ShouldReturnCountryInfo(string? countryTwoLetterCode)
    {
        var countryInfo = _countryHelperService.GetCountryInfo(countryTwoLetterCode!);
        Assert.Null(countryInfo);
    }

    [Theory]
    [InlineData("IRL", "IE")]
    [InlineData("KNI", "")]
    [InlineData("DK", "DK")]
    [InlineData("I", "IT")]
    [InlineData("", null)]
    public void WhenValidDataFromLoganto_ShouldReturnRightDateLetterIsoCode(
        string countryIdInput, string? expectedCountryOutput)
    {
        var countryOutput = _countryHelperService.GetLogantoCountryTwoLetterIsoCode(countryIdInput);
        Assert.Equal(expectedCountryOutput, countryOutput);
    }

    [Theory]
    [InlineData("IRL", "IE", "Irland")]
    [InlineData("SU", "", "Union der Sozialistischen Sowjetrepubliken")]
    [InlineData("MON", "", "Monserrat")]
    [InlineData("BRN", "BH", "Bahrain")]
    [InlineData("YYY", "", "Staatenlos")]
    [InlineData("NL", "NL", "Niederlande")]
    public void WhenValidDataFromLoganto_ShouldReturnRightDateIsoAndShortNameDe(string countryIdInput, string expactedIso2Code, string expactedName)
    {
        var countryOutput = _countryHelperService.GetLogantoCountryTwoLetterIsoAndShortNameDe(countryIdInput);
        countryOutput.Should().NotBeNull();
        countryOutput!.Iso2Id.Should().Be(expactedIso2Code);
        countryOutput!.ShortNameDe.Should().Be(expactedName);
    }
}
