// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentAssertions;
using Grpc.Core;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Models;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;
using Timestamp = Google.Protobuf.WellKnownTypes.Timestamp;

namespace Voting.Stimmregister.WebService.Integration.Tests.ECollectingGrpcServiceTest;

public class GetPeopleByNameTest : BaseWriteableDbGrpcTest<EcollectingService.EcollectingServiceClient>
{
    public GetPeopleByNameTest(TestApplicationFactory factory)
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
        var request = NewECollectingServiceGetPeopleByNameRequest();
        var response = await SgReaderClient.EcollectingServiceGetPeopleByNameAsync(request);
        response.MatchSnapshot();
    }

    [Fact]

    public async Task WhenCalledForValidPersonWithFirstNameOnly_ShouldResolvePerson()
    {
        var request = new EcollectingServiceGetPeopleByNameRequest
        {
            MunicipalityId = PersonMockedData.MunicipalityIdStGallen,
            FirstName = "meghana",
            ActualityDate = Timestamp.FromDateTime(new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
        };
        var response = await SgReaderClient.EcollectingServiceGetPeopleByNameAsync(request);
        response.MatchSnapshot();
    }

    [Fact]

    public async Task WhenCalledForValidPersonWithFirstNamePrefixOnly_ShouldResolvePerson()
    {
        var request = new EcollectingServiceGetPeopleByNameRequest
        {
            MunicipalityId = PersonMockedData.MunicipalityIdStGallen,
            FirstName = "megh",
            ActualityDate = Timestamp.FromDateTime(new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
        };
        var response = await SgReaderClient.EcollectingServiceGetPeopleByNameAsync(request);
        response.MatchSnapshot();
    }

    [Fact]

    public async Task WhenCalledForValidPersonWithFirstNameSuffixOnly_ShouldNotFound()
    {
        var request = new EcollectingServiceGetPeopleByNameRequest
        {
            MunicipalityId = PersonMockedData.MunicipalityIdStGallen,
            FirstName = "ghana",
            ActualityDate = Timestamp.FromDateTime(new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
        };
        var response = await SgReaderClient.EcollectingServiceGetPeopleByNameAsync(request);
        response.People.Should().BeEmpty();
    }

    [Fact]
    public async Task WhenCalledWithMissingParameters_ShouldReturnValidationException()
    {
        await AssertStatus(
            async () => await SgReaderClient.EcollectingServiceGetPeopleByNameAsync(new EcollectingServiceGetPeopleByNameRequest()),
            StatusCode.InvalidArgument,
            nameof(ValidationException));
    }

    [Fact]
    public async Task WhenCalledWithTooLargePageSize_ShouldReturnValidationException()
    {
        var req = NewECollectingServiceGetPeopleByNameRequest();
        req.Paging = new PagingModel { PageSize = 11, PageIndex = 0 };
        await AssertStatus(
            async () => await SgReaderClient.EcollectingServiceGetPeopleByNameAsync(req),
            StatusCode.InvalidArgument,
            nameof(ValidationException));
    }

    [Fact]

    public async Task WhenCalledForNotFound_ShouldReturnEmptyList()
    {
        var request = NewECollectingServiceGetPeopleByNameRequest();
        request.OfficialName = "foobarbaz";
        var response = await SgReaderClient.EcollectingServiceGetPeopleByNameAsync(request);
        response.People.Should().BeEmpty();
    }

    protected override async Task AuthorizationTestCall(EcollectingService.EcollectingServiceClient service)
    {
        await service.EcollectingServiceGetPeopleByNameAsync(NewECollectingServiceGetPeopleByNameRequest());
    }

    private static EcollectingServiceGetPeopleByNameRequest NewECollectingServiceGetPeopleByNameRequest(Action<EcollectingServiceGetPeopleByNameRequest>? customizer = null)
    {
        var request = new EcollectingServiceGetPeopleByNameRequest
        {
            MunicipalityId = PersonMockedData.MunicipalityIdStGallen,
            FirstName = "meghana",
            OfficialName = "bal",
            DateOfBirth = Timestamp.FromDateTime(new DateTime(1990, 11, 15, 0, 0, 0, DateTimeKind.Utc)),
            AddressStreet = "sturzeneggstr.",
            AddressHouseNumber = "31",
            ActualityDate = Timestamp.FromDateTime(new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
        };

        customizer?.Invoke(request);
        return request;
    }
}
