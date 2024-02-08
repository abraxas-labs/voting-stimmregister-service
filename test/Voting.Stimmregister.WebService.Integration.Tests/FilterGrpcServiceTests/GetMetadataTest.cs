// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Grpc.Core;
using Voting.Lib.Testing.Mocks;
using Voting.Stimmregister.Domain.Authorization;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.FilterGrpcServiceTests;

public class GetMetadataTest : BaseReadOnlyGrpcTest<FilterService.FilterServiceClient>
{
    public GetMetadataTest(TestReadOnlyApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ShouldReturnCounts()
    {
        var response = await SgReaderClient.GetMetadataAsync(new FilterServicePreviewMetadataRequest
        {
            FilterId = FilterMockedData.SomeFilter_MunicipalityIdOther3.Id.ToString(),
            Deadline = MockedClock.UtcNowTimestamp,
        });

        response.CountOfPersons.Should().Be(19);
        response.CountOfInvalidPersons.Should().Be(3);
    }

    [Fact]
    public async Task UnknownIdShouldReturnNotFound()
    {
        await AssertStatus(
            async () =>
            {
                await SgReaderClient.GetMetadataAsync(new FilterServicePreviewMetadataRequest
                {
                    FilterId = "b10d9217-db04-4435-acdd-e7fc14898789",
                    Deadline = MockedClock.UtcNowTimestamp,
                });
            },
            StatusCode.NotFound);
    }

    [Fact]
    public async Task OtherTenantIdShouldReturnNotFound()
    {
        await AssertStatus(
            async () =>
            {
                await SgReaderClient.GetMetadataAsync(new FilterServicePreviewMetadataRequest
                {
                    FilterId = FilterMockedData.SomeFilter_MunicipalityIdOther.Id.ToString(),
                    Deadline = MockedClock.UtcNowTimestamp,
                });
            },
            StatusCode.NotFound);
    }

    protected override async Task AuthorizationTestCall(FilterService.FilterServiceClient service)
        => await service.GetMetadataAsync(new FilterServicePreviewMetadataRequest
        {
            FilterId = FilterMockedData.SomeFilter_MunicipalityIdOther3.Id.ToString(),
            Deadline = MockedClock.UtcNowTimestamp,
        });

    protected override IEnumerable<string> UnauthorizedRoles()
    {
        yield return Roles.ApiImporter;
        yield return Roles.ManualImporter;
        yield return Roles.ImportObserver;
        yield return Roles.EVoting;
        yield return Roles.ManualExporter;
        yield return Roles.ApiExporter;
    }
}
