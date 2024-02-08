// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Domain.Authorization;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.FilterGrpcServiceTests;

public class GetSingleVersionTest : BaseWriteableDbGrpcTest<FilterService.FilterServiceClient>
{
    public GetSingleVersionTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await ResetDb();
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
        await PersonMockedData.Seed(RunScoped);
        await FilterVersionMockedData.Seed(RunScoped);
    }

    [Fact]
    public async Task GetSingleVersion_SgManager()
    {
        var response = await SgManagerClient.GetSingleVersionAsync(new FilterServiceGetSingleVersionRequest
        {
            FilterVersionId = FilterVersionMockedData.SomeFilterVersion_MunicipalityIdOther2.Id.ToString(),
        });
        response.MatchSnapshot();
    }

    [Fact]
    public async Task GetSingleVersion_SgReader()
    {
        var response = await SgReaderClient.GetSingleVersionAsync(new FilterServiceGetSingleVersionRequest
        {
            FilterVersionId = FilterVersionMockedData.SomeFilterVersion_MunicipalityIdOther2.Id.ToString(),
        });
        response.MatchSnapshot();
    }

    [Fact]
    public async Task GetSingleVersion_UnknownId()
    {
        await AssertStatus(
            async () => await SgReaderClient.GetSingleVersionAsync(new FilterServiceGetSingleVersionRequest
            {
                FilterVersionId = "70cc5b6f-46c1-48c4-9345-b7487d298e12",
            }),
            StatusCode.NotFound);
    }

    protected override async Task AuthorizationTestCall(FilterService.FilterServiceClient service)
    {
        await service.GetSingleVersionAsync(new FilterServiceGetSingleVersionRequest { FilterVersionId = FilterVersionMockedData.SomeFilterVersion_MunicipalityIdOther2.Id.ToString() });
    }

    protected override IEnumerable<string> UnauthorizedRoles()
    {
        yield return NoRole;
        yield return Roles.ManualExporter;
    }
}
