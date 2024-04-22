// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using Grpc.Core;
using Voting.Stimmregister.Domain.Authorization;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.PersonGrpcServiceTests;

public class GetByFilterVersionIdTest : BaseWriteableDbGrpcTest<PersonService.PersonServiceClient>
{
    public GetByFilterVersionIdTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task WhenRoleUnauthorizedShouldPermissionDenied()
    {
        var rolesArray = new[]
        {
            Roles.ApiImporter,
            Roles.ApiExporter,
            Roles.ManualImporter,
            Roles.ManualExporter,
            Roles.ImportObserver,
        };

        var request = NewValidGetByFilterVersionIdRequest();
        var client = CreateGrpcService(CreateGrpcChannel(true, tenant: VotingIamTenantIds.KTSG, roles: rolesArray));

        var exception = await Assert.ThrowsAsync<RpcException>(async () =>
            await client.GetByFilterVersionIdAsync(request));
        Assert.Equal(StatusCode.PermissionDenied, exception.StatusCode);
    }

    protected override async Task AuthorizationTestCall(PersonService.PersonServiceClient service)
    {
        await service.GetByFilterVersionIdAsync(NewValidGetByFilterVersionIdRequest());
    }

    private static PersonServiceGetByFilterVersionIdRequest NewValidGetByFilterVersionIdRequest(Action<PersonServiceGetByFilterVersionIdRequest>? customizer = null)
    {
        var request = new PersonServiceGetByFilterVersionIdRequest();
        customizer?.Invoke(request);
        return request;
    }
}
