// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Grpc.Core;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.ECollectingGrpcServiceTest;

public class GetPersonIdByAhvn13Test : BaseWriteableDbGrpcTest<EcollectingService.EcollectingServiceClient>
{
    public GetPersonIdByAhvn13Test(TestApplicationFactory factory)
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
        var request = NewECollectingServiceGetPersonIdByAhvn13Request();
        var response = await ECollectingCitizenReaderClient.EcollectingServiceGetPersonIdByAhvn13Async(request);
        response.MatchSnapshot(x => x.Age); // Because age will change over time.
    }

    [Fact]

    public async Task WhenCalledForValidPersonInCanton_ShouldResolvePerson()
    {
        var request = new EcollectingServiceGetPersonIdByAhvn13Request
        {
            Vn = 7561234567873,
            CantonBfs = 17,
        };
        var response = await ECollectingCitizenReaderClient.EcollectingServiceGetPersonIdByAhvn13Async(request);
        response.MatchSnapshot(x => x.Age); // Because age will change over time.
    }

    [Fact]

    public async Task WhenUnknownAhv_ShouldReturnNotFound()
    {
        await AssertStatus(
            async () => await ECollectingCitizenReaderClient.EcollectingServiceGetPersonIdByAhvn13Async(NewECollectingServiceGetPersonIdByAhvn13Request(x => x.Vn = 7565528412733)),
            StatusCode.NotFound);
    }

    [Fact]

    public async Task WhenOtherMunicipality_ShouldReturnNotFound()
    {
        await AssertStatus(
            async () => await ECollectingCitizenReaderClient.EcollectingServiceGetPersonIdByAhvn13Async(NewECollectingServiceGetPersonIdByAhvn13Request(x => x.Vn = 7563568281678)),
            StatusCode.NotFound);
    }

    [Fact]

    public async Task WhenInvalid_ShouldReturnNotFound()
    {
        await AssertStatus(
            async () => await ECollectingCitizenReaderClient.EcollectingServiceGetPersonIdByAhvn13Async(NewECollectingServiceGetPersonIdByAhvn13Request(x => x.Vn = 7564895123447)),
            StatusCode.NotFound);
    }

    [Fact]

    public async Task WhenNoVotingRight_ShouldReturnNotFound()
    {
        await AssertStatus(
            async () => await ECollectingCitizenReaderClient.EcollectingServiceGetPersonIdByAhvn13Async(NewECollectingServiceGetPersonIdByAhvn13Request(x => x.Vn = 7568093905969)),
            StatusCode.NotFound);
    }

    [Fact]

    public async Task WhenCalledWithMissingParameters_ShouldReturnValidationException()
    {
        await AssertStatus(
            async () => await ECollectingCitizenReaderClient.EcollectingServiceGetPersonIdByAhvn13Async(new EcollectingServiceGetPersonIdByAhvn13Request()),
            StatusCode.InvalidArgument,
            nameof(ValidationException));
    }

    protected override async Task AuthorizationTestCall(EcollectingService.EcollectingServiceClient service)
    {
        await service.EcollectingServiceGetPersonIdByAhvn13Async(NewECollectingServiceGetPersonIdByAhvn13Request());
    }

    private static EcollectingServiceGetPersonIdByAhvn13Request NewECollectingServiceGetPersonIdByAhvn13Request(Action<EcollectingServiceGetPersonIdByAhvn13Request>? customizer = null)
    {
        var request = new EcollectingServiceGetPersonIdByAhvn13Request
        {
            Vn = 7561234567873,
            MunicipalityId = PersonMockedData.MunicipalityIdStGallen,
        };

        customizer?.Invoke(request);
        return request;
    }
}
