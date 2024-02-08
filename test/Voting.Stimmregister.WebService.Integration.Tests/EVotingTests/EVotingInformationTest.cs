// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentAssertions;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Constants.EVoting;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Voting.Stimmregister.Test.Utils.MockData.EVoting;
using Voting.Stimmregister.WebService.Controllers;
using Voting.Stimmregister.WebService.Models.EVoting.Request;
using Voting.Stimmregister.WebService.Models.EVoting.Response;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.EVotingTests;

/// <summary>
/// Test for <see cref="EVotingController"/>.
/// </summary>
public class EVotingInformationTest : BaseWriteableDbRestTest
{
    private const string InformationApiUrl = "v2/evoting/information";

    private readonly EVotingConfig _config;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true,
    };

    public EVotingInformationTest(TestApplicationFactory factory)
        : base(factory)
    {
        _config = GetService<EVotingConfig>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _config.EnableKewrAndLoganto = true;
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenPassingUnsupportedOrganisationUnit()
    {
        var resp = await ApiEVotingClient.PostAsJsonAsync(InformationApiUrl, new RegistrationStatusRequest
        {
            Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonMissing,
        });

        var content = await resp.Content.ReadFromJsonAsync<GetRegistrationInformationResponse>(_jsonOptions);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content!.ProcessStatusCode.Should().Be(ProcessStatusCode.LogantoOrganisationUnitNotFound);
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenKewrSearchReturnsHttpStatusErrorCode()
    {
        HttpClientFactoryMocked.KewrSearchResponse = HttpClientFactoryMocked.KewrSearchWithHttpStatusErrorCode;

        var resp = await ApiEVotingClient.PostAsJsonAsync(InformationApiUrl, new RegistrationStatusRequest
        {
            Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        var content = await resp.Content.ReadFromJsonAsync<GetRegistrationInformationResponse>(_jsonOptions);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content!.ProcessStatusCode.Should().Be(ProcessStatusCode.KewrServiceRequestError);
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenKewrSearchResultHasNoActivePersons()
    {
        HttpClientFactoryMocked.KewrSearchResponse = HttpClientFactoryMocked.KewrSearchWithoutActivePerson;

        var resp = await ApiEVotingClient.PostAsJsonAsync(InformationApiUrl, new RegistrationStatusRequest
        {
            Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        var content = await resp.Content.ReadFromJsonAsync<GetRegistrationInformationResponse>(_jsonOptions);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content!.ProcessStatusCode.Should().Be(ProcessStatusCode.KewrServicePersonError);
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenKewrSearchResultHasNoMainResidence()
    {
        HttpClientFactoryMocked.KewrSearchResponse = HttpClientFactoryMocked.KewrSearchWithoutMainResidence;

        var resp = await ApiEVotingClient.PostAsJsonAsync(InformationApiUrl, new RegistrationStatusRequest
        {
            Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        var content = await resp.Content.ReadFromJsonAsync<GetRegistrationInformationResponse>(_jsonOptions);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content!.ProcessStatusCode.Should().Be(ProcessStatusCode.KewrServicePersonError);
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenKewrSearchResultHasMultipleMatches()
    {
        HttpClientFactoryMocked.KewrSearchResponse = HttpClientFactoryMocked.KewrSearchWithMultipleMatches;

        var resp = await ApiEVotingClient.PostAsJsonAsync(InformationApiUrl, new RegistrationStatusRequest
        {
            Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        var content = await resp.Content.ReadFromJsonAsync<GetRegistrationInformationResponse>(_jsonOptions);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content!.ProcessStatusCode.Should().Be(ProcessStatusCode.KewrServicePersonError);
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenKewrGetReturnsHttpStatusErrorCode()
    {
        HttpClientFactoryMocked.KewrSearchResponse = HttpClientFactoryMocked.KewrSearchValidResult;
        HttpClientFactoryMocked.KewrGetResponse = HttpClientFactoryMocked.KewrGetWithHttpStatusErrorCode;

        var resp = await ApiEVotingClient.PostAsJsonAsync(InformationApiUrl, new RegistrationStatusRequest
        {
            Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        var content = await resp.Content.ReadFromJsonAsync<GetRegistrationInformationResponse>(_jsonOptions);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content!.ProcessStatusCode.Should().Be(ProcessStatusCode.KewrServiceRequestError);
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenKewrGetReturnsNoAhvn13()
    {
        HttpClientFactoryMocked.KewrSearchResponse = HttpClientFactoryMocked.KewrSearchValidResult;
        HttpClientFactoryMocked.KewrGetResponse = HttpClientFactoryMocked.KewrGetWithoutAhvn13;

        var resp = await ApiEVotingClient.PostAsJsonAsync(InformationApiUrl, new RegistrationStatusRequest
        {
            Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        var content = await resp.Content.ReadFromJsonAsync<GetRegistrationInformationResponse>(_jsonOptions);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content!.ProcessStatusCode.Should().Be(ProcessStatusCode.KewrServiceDataError);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenNotPassingAhvn13()
    {
        await AssertStatus(
            () => ApiEVotingClient.PostAsJsonAsync(InformationApiUrl, new RegistrationStatusRequest
            {
                BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
            }),
            HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPassingInvalidAhvn13Checksum()
    {
        await AssertStatus(
            () => ApiEVotingClient.PostAsJsonAsync(InformationApiUrl, new RegistrationStatusRequest
            {
                Ahvn13 = EVotingAhvn13MockedData.Ahvn13InvalidChecksumFormatted,
                BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
            }),
            HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPassingEmptyAhvn13()
    {
        await AssertStatus(
            () => ApiEVotingClient.PostAsJsonAsync(InformationApiUrl, new RegistrationStatusRequest
            {
                Ahvn13 = string.Empty,
                BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
            }),
            HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPersonNotFound()
    {
        _config.EnableKewrAndLoganto = false;

        await AssertStatus(
            () => ApiEVotingClient.PostAsJsonAsync(InformationApiUrl, new RegistrationStatusRequest
            {
                Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
                BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
            }),
            HttpStatusCode.BadRequest);
    }

    protected override Task<HttpResponseMessage> AuthorizationTestCall(HttpClient httpClient)
    {
        return httpClient.PostAsJsonAsync(InformationApiUrl, new RegistrationStatusRequest
        {
            Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });
    }
}
