// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using FluentAssertions;
using Voting.Stimmregister.WebService.Configuration;
using Xunit;

namespace Voting.Stimmregister.WebService.Unit.Tests.Configuration;

public class AppConfigTests
{
    private readonly AppConfig _config;

    public AppConfigTests()
    {
        _config = GetAppConfigMock();
    }

    [Fact]
    public void DefaultConfigShouldReturnDefaultPort()
    {
        _config.Database.Port.Should().Be(5432);
    }

    private AppConfig GetAppConfigMock()
    {
        return new AppConfig();
    }
}
