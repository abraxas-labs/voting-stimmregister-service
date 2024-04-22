// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Voting.Lib.Ech.Ech0045_4_0.Schemas;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Domain.Authorization;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.ExportRestTests;

public class ApiExportEchRestTest : BaseReadOnlyRestTest
{
    private const string Route = "v1/export/ech-0045";

    public ApiExportEchRestTest(TestReadOnlyApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task WhenFilterByName_ShouldReturnUnauthorized()
    {
        var request = new PersonSearchParametersExportModel();
        request.Criteria.Add(new()
        {
            ReferenceId = FilterReference.OfficialName,
            FilterValue = PersonMockedData.Person_3203_StGallen_1.OfficialName,
            FilterOperator = FilterOperatorType.Equals,
            FilterType = FilterDataType.String,
        });

        var response = await SgApiExporterClient.PostAsJsonAsync(Route, request);
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task WhenNoFilter_ShouldThrowBadRequest()
    {
        var request = new PersonSearchParametersExportModel();
        var response = await SgApiExporterClient.PostAsJsonAsync(Route, request);
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenFilterIdAndNoVersionId_ShouldReturnUnauthorized()
    {
        var request = new PersonSearchParametersExportModel
        {
            FilterId = FilterMockedData.SomeFilter_MunicipalityIdOther2.Id,
        };

        var response = await SgApiExporterClient.PostAsJsonAsync(Route, request);
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task WhenVersionId_ShouldResolvePersonsExportEch()
    {
        var request = new PersonSearchParametersExportModel
        {
            VersionId = FilterVersionMockedData.SomeFilterVersion_MunicipalityIdOther2.Id,
        };

        var xmlContent = await Export(request);
        xmlContent.MatchFormattedXmlSnapshot();
    }

    [Fact]
    public async Task WhenRoleUnauthorized_ShouldReturnForbidden()
    {
        var request = new PersonSearchParametersExportModel();
        var rolesArray = new[]
        {
            Roles.Reader,
            Roles.Manager,
            Roles.ManualImporter,
            Roles.ApiImporter,
            Roles.ImportObserver,
        };

        var client = CreateHttpClient(true, tenant: VotingIamTenantIds.KTSG, roles: rolesArray);
        var response = await client.PostAsJsonAsync(Route, request);
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Forbidden);
    }

    protected override Task<HttpResponseMessage> AuthorizationTestCall(HttpClient httpClient)
        => httpClient.PostAsJsonAsync(Route, new PersonSearchParametersExportModel { VersionId = FilterVersionMockedData.SomeFilterVersion_MunicipalityId.Id });

    private async Task<string> Export(PersonSearchParametersExportModel request)
    {
        var response = await SgApiExporterClient.PostAsJsonAsync(Route, request);
        response.EnsureSuccessStatusCode();

        var responseXml = await response.Content.ReadAsStringAsync();
        XmlUtil.ValidateSchema(responseXml, Ech0045Schemas.LoadEch0045Schemas());
        return responseXml;
    }
}
