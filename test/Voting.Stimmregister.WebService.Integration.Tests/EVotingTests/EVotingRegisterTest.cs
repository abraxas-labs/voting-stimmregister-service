// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentAssertions;
using Voting.Lib.Common;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Constants.EVoting;
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
public class EVotingRegisterTest : BaseWriteableDbRestTest
{
    private const string RegisterApiUrl = "v2/evoting/register";

    private readonly EVotingConfig _config;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true,
    };

    public EVotingRegisterTest(TestApplicationFactory factory)
        : base(factory)
    {
        _config = GetService<EVotingConfig>();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _config.EnableKewrAndLoganto = true;
        _config.SkipForwardingEVoterFlag = [];
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenLogantoReturnsHttpErrorCode()
    {
        HttpClientFactoryMocked.KewrSearchResponse = HttpClientFactoryMocked.KewrSearchValidResult;
        HttpClientFactoryMocked.KewrGetResponse = HttpClientFactoryMocked.KewrGetWithVotingPermission;
        HttpClientFactoryMocked.LogantoSetFlagResponse = HttpClientFactoryMocked.LogantoSetFlagHttpError;

        var resp = await ApiEVotingClient.PostAsJsonAsync(RegisterApiUrl, new CreateRegistrationRequest
        {
            Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        var content = await resp.Content.ReadFromJsonAsync<ProcessStatusResponseBase>(_jsonOptions);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content!.ProcessStatusCode.Should().Be(ProcessStatusCode.LogantoServiceRequestError);
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenLogantoReturnsBusinessErrorStatus()
    {
        HttpClientFactoryMocked.KewrSearchResponse = HttpClientFactoryMocked.KewrSearchValidResult;
        HttpClientFactoryMocked.KewrGetResponse = HttpClientFactoryMocked.KewrGetWithVotingPermission;
        HttpClientFactoryMocked.LogantoSetFlagResponse = HttpClientFactoryMocked.LogantoServiceBusinessError;

        var resp = await ApiEVotingClient.PostAsJsonAsync(RegisterApiUrl, new CreateRegistrationRequest
        {
            Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        var content = await resp.Content.ReadFromJsonAsync<ProcessStatusResponseBase>(_jsonOptions);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content!.ProcessStatusCode.Should().Be(ProcessStatusCode.LogantoServiceBusinessError);
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenLogantoReturnsDataErrorStatus()
    {
        HttpClientFactoryMocked.KewrSearchResponse = HttpClientFactoryMocked.KewrSearchValidResult;
        HttpClientFactoryMocked.KewrGetResponse = HttpClientFactoryMocked.KewrGetWithVotingPermission;
        HttpClientFactoryMocked.LogantoSetFlagResponse = HttpClientFactoryMocked.LogantoServiceDataError;

        var resp = await ApiEVotingClient.PostAsJsonAsync(RegisterApiUrl, new CreateRegistrationRequest
        {
            Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        var content = await resp.Content.ReadFromJsonAsync<ProcessStatusResponseBase>(_jsonOptions);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content!.ProcessStatusCode.Should().Be(ProcessStatusCode.LogantoServiceRequestError);
    }

    [Fact]
    public async Task ShouldPersistEVotingStatusWhenLogantoRegistrationSucceeds()
    {
        HttpClientFactoryMocked.KewrSearchResponse = HttpClientFactoryMocked.KewrSearchValidResult;
        HttpClientFactoryMocked.KewrGetResponse = HttpClientFactoryMocked.KewrGetWithVotingPermission;
        HttpClientFactoryMocked.LogantoSetFlagResponse = HttpClientFactoryMocked.LogantoSetFlagSuccess;

        var resp = await ApiEVotingClient.PostAsJsonAsync(RegisterApiUrl, new CreateRegistrationRequest
        {
            Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        await RunOnDb(context =>
        {
            var eVoter = context.Set<EVoterEntity>().FirstOrDefault(v => v.Ahvn13 == EVotingAhvn13MockedData.Ahvn13Valid1);
            var audits = context.Set<EVoterAuditEntity>().Where(v => v.EVoterId == eVoter!.Id);
            eVoter.Should().NotBeNull();
            eVoter!.BfsCanton.Should().Be(EVotingBfsCantonMockedData.BfsCantonValid);
            eVoter.ContextId.Should().BeNull();
            eVoter.EVoterFlag.Should().BeTrue();
            eVoter.AuditInfo.CreatedById.Should().Be("default-user-id");
            audits.Should().ContainSingle();
            var audit = audits.First();
            audit.ContextId.Should().BeNull();
            audit.BfsCanton.Should().Be(EVotingBfsCantonMockedData.BfsCantonValid);
            audit.EVoterFlag.Should().BeTrue();
            audit.EVoterId.Should().Be(eVoter.Id);
            audit.StatusCode.Should().Be((short?)ProcessStatusCode.Success);
            audit.EVoterAuditInfo.CreatedById.Should().Be("default-user-id");

            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task ShouldPersistEVotingStatusWhenLogantoAndKewrIsDisabled()
    {
        _config.EnableKewrAndLoganto = false;

        await ResetDb();
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
        await PersonMockedData.Seed(RunScoped);
        await BfsIntegrityMockedData.Seed(RunScoped);

        var ahvNumber = PersonMockedData.Person_3213_Goldach_1.Vn!.Value;

        var resp = await ApiEVotingClient.PostAsJsonAsync(RegisterApiUrl, new CreateRegistrationRequest
        {
            Ahvn13 = Ahvn13.Parse(ahvNumber).ToString(),
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        await RunOnDb(context =>
        {
            var eVoter = context.Set<EVoterEntity>().FirstOrDefault(v => v.Ahvn13 == ahvNumber);
            var audits = context.Set<EVoterAuditEntity>().Where(v => v.EVoterId == eVoter!.Id);
            eVoter.Should().NotBeNull();
            eVoter!.BfsCanton.Should().Be(EVotingBfsCantonMockedData.BfsCantonValid);
            eVoter.BfsMunicipality.Should().Be((short)PersonMockedData.Person_3213_Goldach_1.MunicipalityId);
            eVoter.ContextId.Should().BeNull();
            eVoter.EVoterFlag.Should().BeTrue();
            eVoter.AuditInfo.CreatedById.Should().Be("default-user-id");
            audits.Should().ContainSingle();
            var audit = audits.First();
            audit.ContextId.Should().BeNull();
            audit.BfsCanton.Should().Be(EVotingBfsCantonMockedData.BfsCantonValid);
            audit.BfsMunicipality.Should().Be((short)PersonMockedData.Person_3213_Goldach_1.MunicipalityId);
            audit.EVoterFlag.Should().BeTrue();
            audit.EVoterId.Should().Be(eVoter.Id);
            audit.StatusCode.Should().BeNull();
            audit.EVoterAuditInfo.CreatedById.Should().Be("default-user-id");

            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task ShouldPersistEVotingStatusWhenOnlyLogantoIsDisabled()
    {
        HttpClientFactoryMocked.KewrSearchResponse = HttpClientFactoryMocked.KewrSearchValidResult;
        HttpClientFactoryMocked.KewrGetResponse = HttpClientFactoryMocked.KewrGetWithVotingPermission;

        _config.SkipForwardingEVoterFlag = [3203, 3213];

        await ResetDb();
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
        await PersonMockedData.Seed(RunScoped);
        await BfsIntegrityMockedData.Seed(RunScoped);

        var ahvNumber = PersonMockedData.Person_3213_Goldach_1.Vn!.Value;

        var resp = await ApiEVotingClient.PostAsJsonAsync(RegisterApiUrl, new CreateRegistrationRequest
        {
            Ahvn13 = Ahvn13.Parse(ahvNumber).ToString(),
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        await RunOnDb(context =>
        {
            var eVoter = context.Set<EVoterEntity>().FirstOrDefault(v => v.Ahvn13 == ahvNumber);
            var audits = context.Set<EVoterAuditEntity>().Where(v => v.EVoterId == eVoter!.Id);
            eVoter.Should().NotBeNull();
            eVoter!.BfsCanton.Should().Be(EVotingBfsCantonMockedData.BfsCantonValid);
            eVoter.ContextId.Should().BeNull();
            eVoter.EVoterFlag.Should().BeTrue();
            eVoter.AuditInfo.CreatedById.Should().Be("default-user-id");
            audits.Should().ContainSingle();
            var audit = audits.First();
            audit.ContextId.Should().BeNull();
            audit.BfsCanton.Should().Be(EVotingBfsCantonMockedData.BfsCantonValid);
            audit.EVoterFlag.Should().BeTrue();
            audit.EVoterId.Should().Be(eVoter.Id);
            audit.StatusCode.Should().BeNull();
            audit.EVoterAuditInfo.CreatedById.Should().Be("default-user-id");

            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPassingEmptyAhvn13()
    {
        await AssertStatus(
            () => ApiEVotingClient.PostAsJsonAsync(RegisterApiUrl, new CreateRegistrationRequest
            {
                Ahvn13 = string.Empty,
                BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
            }),
            HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPassingInvalidAhvn13Checksum()
    {
        await AssertStatus(
            () => ApiEVotingClient.PostAsJsonAsync(RegisterApiUrl, new CreateRegistrationRequest
            {
                Ahvn13 = EVotingAhvn13MockedData.Ahvn13InvalidChecksumFormatted,
                BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
            }),
            HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPersonNotFound()
    {
        _config.EnableKewrAndLoganto = false;

        await AssertStatus(
            () => ApiEVotingClient.PostAsJsonAsync(RegisterApiUrl, new CreateRegistrationRequest
            {
                Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
                BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
            }),
            HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPassingNoContent()
    {
        await AssertStatus(
            () => ApiEVotingClient.PostAsJsonAsync(RegisterApiUrl, new { }),
            HttpStatusCode.BadRequest);
    }

    protected override Task<HttpResponseMessage> AuthorizationTestCall(HttpClient httpClient)
    {
        return httpClient.PostAsync(RegisterApiUrl, null);
    }
}
