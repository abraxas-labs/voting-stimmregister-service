// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using FluentAssertions;
using Voting.Stimmregister.Domain.Mapping;
using Voting.Stimmregister.Domain.Utils;
using Xunit;

namespace Voting.Stimmregister.Domain.Unit.Tests.Utils;

public class CountryHelperServiceTests
{
    private readonly ICountryHelperService _countryHelperService;

    public CountryHelperServiceTests()
    {
        var mapper = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<EchMappingProfile>()).CreateMapper();
        _countryHelperService = new CountryHelperService(mapper);
    }

    [Theory]
    [InlineData("GERMANY", "7777", null, "DE")]
    [InlineData("Switzerland", "8100", null, "CH")]
    [InlineData("UNITED STATES OF AMERICA", "8439", null, "US")]
    [InlineData("UNITED STATES OF AMERICA", null, "US", "US")]
    [InlineData("SRI LANKA", null, null, "LK")]
    [InlineData(null, null, "LK", "LK")]
    public void WhenPassingValidCountryOrBfs_ShouldResolveIso2(string? countryInput, string? countryNumberInput, string? iso2Input, string expectedCountryOutput)
    {
        var countryOutput = _countryHelperService.GetCountryTwoLetterIsoCode(countryInput, countryNumberInput, iso2Input);
        Assert.Equal(expectedCountryOutput, countryOutput);
    }

    [Theory]
    [InlineData("8551", "AE")]
    public void WhenPassingCountryNumberNotRecognizedByCh_ShouldResolveIso2(string countryNumberInput, string expectedIso2)
    {
        var iso2Code = _countryHelperService.GetCountryTwoLetterIsoCode(null, countryNumberInput);
        Assert.Equal(expectedIso2, iso2Code);
    }

    [Theory]
    [InlineData("UNITED STATES OF AMERICA", null, null)]
    [InlineData(null, "1", null)]
    [InlineData(null, null, "XY")]
    [InlineData("", "", "")]
    [InlineData(null, null, null)]
    public void WhenPassingInvalidCountryAndBfs_ShouldReturnNull(string? countryInput, string? countryNumberInput, string? iso2Input)
    {
        var countryOutput = _countryHelperService.GetCountryTwoLetterIsoCode(countryInput, countryNumberInput, iso2Input);
        Assert.Null(countryOutput);
    }

    [Theory]
    [InlineData("CH", 8100)]
    [InlineData("AQ", 8701)]
    public void WhenPassingInvalidCountryIso2Code_ShouldReturnNull(string countryTwoLetterCode, int expectedBfs)
    {
        var countryInfo = _countryHelperService.GetCountryInfo(countryTwoLetterCode);
        Assert.Equal(expectedBfs, countryInfo!.BfsId);
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
        countryOutput!.Iso2.Should().Be(expactedIso2Code);
        countryOutput!.ShortNameDe.Should().Be(expactedName);
    }
}
