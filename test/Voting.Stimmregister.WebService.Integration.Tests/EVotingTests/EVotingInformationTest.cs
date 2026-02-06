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
using Voting.Stimmregister.Domain.Models;
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

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true,
    };

    public EVotingInformationTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await PersonMockedData.Seed(RunScoped);
        await EVotingUserMockedData.Seed(RunScoped);
        await BfsStatisticMockedData.Seed(RunScoped);
    }

    [Fact]
    public async Task ShouldReturnValidInformationWhenPersonIsEVoter()
    {
        var resp = await ApiEVotingClient.PostAsJsonAsync(InformationApiUrl, new RegistrationStatusRequest
        {
            Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid4Formatted,
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        var content = await resp.Content.ReadFromJsonAsync<GetRegistrationInformationResponse>(_jsonOptions);

        content.Should().NotBeNull();
        content.MatchSnapshot();
    }

    [Fact]
    public async Task ShouldReturnEmailWhenPersonHasEVotingEmail()
    {
        await ModifyDbEntities<PersonEntity>(
            p => p.Vn == EVotingAhvn13MockedData.Ahvn13Valid4,
            p => p.EVotingEmail = "test@example.invalid");
        await ModifyDbEntities<EVoterEntity>(
            p => p.Ahvn13 == EVotingAhvn13MockedData.Ahvn13Valid4,
            p => p.EVotingEmail = "test@example.invalid");

        var resp = await ApiEVotingClient.PostAsJsonAsync(InformationApiUrl, new RegistrationStatusRequest
        {
            Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid4Formatted,
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        var content = await resp.Content.ReadFromJsonAsync<GetRegistrationInformationResponse>(_jsonOptions);

        content.Should().NotBeNull();
        content.MatchSnapshot();
    }

    [Fact]
    public async Task ShouldReturnEmailWhenPersonHasLocalEVotingData()
    {
        await ModifyDbEntities<EVoterEntity>(
            p => p.Ahvn13 == EVotingAhvn13MockedData.Ahvn13Valid4,
            p => p.EVotingEmail = "test@example.invalid");

        var resp = await ApiEVotingClient.PostAsJsonAsync(InformationApiUrl, new RegistrationStatusRequest
        {
            Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid4Formatted,
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        var content = await resp.Content.ReadFromJsonAsync<GetRegistrationInformationResponse>(_jsonOptions);

        content.Should().NotBeNull();
        content.MatchSnapshot();
    }

    [Fact]
    public async Task ShouldNotReturnEmailWhenPersonIsLocallyUnregistered()
    {
        // Has email on person (not yet synced)
        await ModifyDbEntities<PersonEntity>(
            p => p.Vn == EVotingAhvn13MockedData.Ahvn13Valid4,
            p => p.EVotingEmail = "test@example.invalid");

        // No email in local e-voting data (e.g. unregistered recently)
        await ModifyDbEntities<EVoterEntity>(
            p => p.Ahvn13 == EVotingAhvn13MockedData.Ahvn13Valid4,
            p => p.EVotingEmail = null);

        var resp = await ApiEVotingClient.PostAsJsonAsync(InformationApiUrl, new RegistrationStatusRequest
        {
            Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid4Formatted,
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        var content = await resp.Content.ReadFromJsonAsync<GetRegistrationInformationResponse>(_jsonOptions);

        content.Should().NotBeNull();
        content.MatchSnapshot();
    }

    [Fact]
    public async Task ShouldReturnValidInformationWhenPersonIsNotEVoter()
    {
        var resp = await ApiEVotingClient.PostAsJsonAsync(InformationApiUrl, new RegistrationStatusRequest
        {
            Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid3Formatted,
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        var content = await resp.Content.ReadFromJsonAsync<GetRegistrationInformationResponse>(_jsonOptions);

        content.Should().NotBeNull();
        content.MatchSnapshot();
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
