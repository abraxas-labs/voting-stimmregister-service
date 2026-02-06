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
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Common;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Voting.Stimmregister.Test.Utils.MockData.EVoting;
using Voting.Stimmregister.WebService.Models.EVoting.Request;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.EVotingTests;

/// <summary>
/// Test for <see cref="EVotingController"/>.
/// </summary>
public class EVotingUnregisterTest : BaseWriteableDbRestTest
{
    private const string UnregisterApiUrl = "v2/evoting/unregister";

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true,
    };

    public EVotingUnregisterTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
    }

    [Fact]
    public async Task ShouldPersistEVotingStatusWhenLogantoIsDisabled()
    {
        await ResetDb();
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
        await PersonMockedData.Seed(RunScoped);
        await BfsIntegrityMockedData.Seed(RunScoped);

        var ahvNumber = PersonMockedData.Person_3213_Goldach_Swiss_Abroad.Vn!.Value;

        var resp = await ApiEVotingClient.PostAsJsonAsync(UnregisterApiUrl, new CreateUnregistrationRequest
        {
            Ahvn13 = Ahvn13.Parse(ahvNumber).ToString(),
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        await RunOnDb(async context =>
        {
            var eVoter = await context.Set<EVoterEntity>().FirstOrDefaultAsync(v => v.Ahvn13 == ahvNumber);
            var audits = context.Set<EVoterAuditEntity>().Where(v => v.EVoterId == eVoter!.Id);
            eVoter.Should().NotBeNull();
            eVoter!.BfsCanton.Should().Be(EVotingBfsCantonMockedData.BfsCantonValid);
            eVoter.ContextId.Should().BeNull();
            eVoter.EVoterFlag.Should().BeFalse();
            eVoter.AuditInfo.CreatedById.Should().Be("default-user-id");
            audits.Should().ContainSingle();
            audits.Should().ContainSingle();
            var audit = audits.First();
            audit.ContextId.Should().BeNull();
            audit.BfsCanton.Should().Be(EVotingBfsCantonMockedData.BfsCantonValid);
            audit.EVoterFlag.Should().BeFalse();
            audit.EVoterId.Should().Be(eVoter.Id);
            audit.StatusCode.Should().Be(null);
            audit.EVoterAuditInfo.CreatedById.Should().Be("default-user-id");
        });
    }

    [Fact]
    public async Task ShouldRemoveEVotingEmail()
    {
        await ResetDb();
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
        await PersonMockedData.Seed(RunScoped);
        await BfsIntegrityMockedData.Seed(RunScoped);

        var ahvNumber = PersonMockedData.Person_3213_Goldach_Swiss_Abroad.Vn!.Value;

        await ModifyDbEntities<EVoterEntity>(
            eVoter => eVoter.Ahvn13 == ahvNumber,
            eVoter => eVoter.EVotingEmail = "test@example.invalid");

        var resp = await ApiEVotingClient.PostAsJsonAsync(UnregisterApiUrl, new CreateUnregistrationRequest
        {
            Ahvn13 = Ahvn13.Parse(ahvNumber).ToString(),
            BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
        });

        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        await RunOnDb(async context =>
        {
            var eVoter = await context.Set<EVoterEntity>().FirstOrDefaultAsync(v => v.Ahvn13 == ahvNumber);
            eVoter.Should().NotBeNull();
            eVoter!.EVotingEmail.Should().BeNull();
        });
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPassingEmptyAhvn13()
    {
        await AssertStatus(
            () => ApiEVotingClient.PostAsJsonAsync(UnregisterApiUrl, new CreateUnregistrationRequest
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
            () => ApiEVotingClient.PostAsJsonAsync(UnregisterApiUrl, new CreateUnregistrationRequest
            {
                Ahvn13 = EVotingAhvn13MockedData.Ahvn13InvalidChecksumFormatted,
                BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
            }),
            HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPassingNoContent()
    {
        await AssertStatus(
            () => ApiEVotingClient.PostAsJsonAsync(UnregisterApiUrl, new { }),
            HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenPersonNotFound()
    {
        await AssertStatus(
            () => ApiEVotingClient.PostAsJsonAsync(UnregisterApiUrl, new CreateUnregistrationRequest
            {
                Ahvn13 = EVotingAhvn13MockedData.Ahvn13Valid1Formatted,
                BfsCanton = EVotingBfsCantonMockedData.BfsCantonValid,
            }),
            HttpStatusCode.BadRequest);
    }

    protected override Task<HttpResponseMessage> AuthorizationTestCall(HttpClient httpClient)
    {
        return httpClient.PostAsync(UnregisterApiUrl, null);
    }
}
