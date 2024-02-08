// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Proto.V1.Services.Models;
using Voting.Stimmregister.Proto.V1.Services.Requests;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.PersonGrpcServiceTests;

public class GetLastUsedParametersTest : BaseWriteableDbGrpcTest<PersonService.PersonServiceClient>
{
    public GetLastUsedParametersTest(TestApplicationFactory factory)
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
    public async Task FilterWithName_ShouldStoreParameters()
    {
        var request = new PersonServiceGetAllRequest
        {
            Paging = new PagingModel
            {
                PageIndex = 0,
                PageSize = 10,
            },
            Criteria =
            {
                new FilterCriteriaModel
                {
                    Id = Guid.Empty.ToString(),
                    ReferenceId = FilterReference.FirstName,
                    FilterValue = PersonMockedData.Person_3203_StGallen_1.FirstName,
                    FilterDataType = FilterDataType.String,
                    FilterOperator = FilterOperator.Equals,
                },
            },
            SearchType = PersonSearchType.Person,
        };

        var response = await SgManagerClient.GetAllAsync(request);
        response.People.Should().HaveCount(2);

        var parameters = await SgManagerClient.GetLastUsedParametersAsync(new PersonServiceGetLastUsedParametersRequest
        {
            SearchType = PersonSearchType.Person,
        });
        parameters.SearchType.Should().Be(PersonSearchType.Person);
        parameters.PageInfo.PageIndex.Should().Be(0);
        parameters.PageInfo.PageSize.Should().Be(10);
        parameters.Criteria.Should().HaveCount(1);
        parameters.Criteria[0].ReferenceId.Should().Be(FilterReference.FirstName);
        parameters.Criteria[0].FilterValue.Should().Be(PersonMockedData.Person_3203_StGallen_1.FirstName);
        parameters.Criteria[0].FilterDataType.Should().Be(FilterDataType.String);
        parameters.Criteria[0].FilterOperator.Should().Be(FilterOperator.Equals);
    }

    protected override async Task AuthorizationTestCall(PersonService.PersonServiceClient service)
        => await service.GetLastUsedParametersAsync(new PersonServiceGetLastUsedParametersRequest
        {
            SearchType = PersonSearchType.Person,
        });
}
