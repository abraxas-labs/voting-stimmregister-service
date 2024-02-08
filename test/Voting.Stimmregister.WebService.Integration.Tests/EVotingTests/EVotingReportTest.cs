// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentAssertions;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Domain.Models.EVoting;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Voting.Stimmregister.Test.Utils.MockData.EVoting;
using Voting.Stimmregister.WebService.Models.EVoting.Request;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.EVotingTests;

/// <summary>
/// Test for <see cref="EVotingController"/>.
/// </summary>
public class EVotingReportTest : BaseWriteableDbRestTest
{
    private const string RegisterApiUrl = "v2/evoting/register";
    private const string ReportApiUrl = "v2/evoting/report";

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true,
    };

    public EVotingReportTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ShouldReturnValidReportWhenPersonHasRegisteredForEVoting()
    {
        HttpClientFactoryMocked.KewrSearchResponse = HttpClientFactoryMocked.KewrSearchValidResult;
        HttpClientFactoryMocked.KewrGetResponse = HttpClientFactoryMocked.KewrGetWithVotingPermission;
        HttpClientFactoryMocked.LogantoSetFlagResponse = HttpClientFactoryMocked.LogantoSetFlagSuccess;

        await ApiEVotingClient.PostAsJsonAsync(RegisterApiUrl, new CreateRegistrationRequest
        {
            Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        var reportResponse = await ApiEVotingClient.PostAsJsonAsync(ReportApiUrl, new RegistrationReportRequest
        {
            Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
        });

        var reportObj = await reportResponse.Content.ReadFromJsonAsync<EVotingReportModel>(_jsonOptions);

        reportObj.Should().NotBeNull();
        reportObj.MatchSnapshot();
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPassingEmptyAhvn13()
    {
        await AssertStatus(
            () => ApiEVotingClient.PostAsJsonAsync(ReportApiUrl, new RegistrationReportRequest
            {
                Ahvn13 = string.Empty,
            }),
            HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPassingInvalidAhvn13Checksum()
    {
        await AssertStatus(
            () => ApiEVotingClient.PostAsJsonAsync(ReportApiUrl, new RegistrationReportRequest
            {
                Ahvn13 = EVotingAhvn13MockedData.Ahvn13InvalidChecksumFormatted,
            }),
            HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenNotPassingAhvn13()
    {
        await AssertStatus(
            () => ApiEVotingClient.PostAsJsonAsync(ReportApiUrl, new RegistrationReportRequest()),
            HttpStatusCode.BadRequest);
    }

    protected override Task<HttpResponseMessage> AuthorizationTestCall(HttpClient httpClient)
    {
        return httpClient.PostAsJsonAsync(ReportApiUrl, new RegistrationReportRequest());
    }
}
