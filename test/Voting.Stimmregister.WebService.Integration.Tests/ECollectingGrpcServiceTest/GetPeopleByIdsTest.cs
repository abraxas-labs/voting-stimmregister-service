// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.ECollectingGrpcServiceTest;

public class GetPeopleByIdsTest : BaseWriteableDbGrpcTest<EcollectingService.EcollectingServiceClient>
{
    public GetPeopleByIdsTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await ResetDb();
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
        await PersonMockedData.Seed(RunScoped);
        await BfsIntegrityMockedData.Seed(RunScoped);
    }

    [Fact]

    public async Task WhenCalledForValidPerson_ShouldResolvePerson()
    {
        var request = NewECollectingServiceGetPeopleByIdsRequest();
        var response = await SgReaderClient.EcollectingServiceGetPeopleByIdsAsync(request);
        response.People.MatchSnapshot();
    }

    [Fact]

    public async Task WhenCalledForUnknownRegisterId_ShouldReturnEmptyList()
    {
        var request = NewECollectingServiceGetPeopleByIdsRequest();
        request.RegisterIds.Clear();
        request.RegisterIds.Add("01cccc16-561d-4d01-a076-2adf168a2a77");
        var response = await SgReaderClient.EcollectingServiceGetPeopleByIdsAsync(request);
        response.People.Should().BeEmpty();
    }

    [Fact]

    public async Task WhenCalledWithMissingParameters_ShouldReturnValidationException()
    {
        await AssertStatus(
            async () => await SgReaderClient.EcollectingServiceGetPeopleByIdsAsync(new EcollectingServiceGetPeopleByIdsRequest()),
            StatusCode.InvalidArgument,
            nameof(ValidationException));
    }

    protected override async Task AuthorizationTestCall(EcollectingService.EcollectingServiceClient service)
    {
        await service.EcollectingServiceGetPeopleByIdsAsync(NewECollectingServiceGetPeopleByIdsRequest());
    }

    private static EcollectingServiceGetPeopleByIdsRequest NewECollectingServiceGetPeopleByIdsRequest(Action<EcollectingServiceGetPeopleByIdsRequest>? customizer = null)
    {
        var request = new EcollectingServiceGetPeopleByIdsRequest
        {
            MunicipalityId = PersonMockedData.MunicipalityIdStGallen,
            RegisterIds = { "cff2b057-decc-4905-ae09-679bea44a0fc" },
            ActualityDate = Timestamp.FromDateTime(new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
        };

        customizer?.Invoke(request);
        return request;
    }
}
