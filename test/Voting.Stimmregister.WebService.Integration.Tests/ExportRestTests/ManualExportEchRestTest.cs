// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Ech.Ech0045_4_0.Schemas;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.ExportRestTests;

public class ManualExportEchRestTest : BaseWriteableDbRestTest
{
    private const string Route = "v1/export/ech-0045";

    public ManualExportEchRestTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await MockDataSeeder.Seed(RunScoped);
    }

    [Fact]
    public async Task WhenFilterByName_ShouldResolvePersonExportEch()
    {
        var request = new PersonSearchParametersExportModel();

        request.Criteria.Add(new()
        {
            ReferenceId = FilterReference.AllianceName,
            FilterValue = PersonMockedData.Person_3203_StGallen_1.AllianceName!,
            FilterOperator = FilterOperatorType.Equals,
            FilterType = FilterDataType.String,
        });

        var xmlContent = await Export(request);
        xmlContent.MatchFormattedXmlSnapshot();
    }

    [Fact]
    public async Task WhenFilterByNameWithManipulatedPerson_ShouldThrow()
    {
        await ManipulatePerson();

        var request = new PersonSearchParametersExportModel();

        request.Criteria.Add(new()
        {
            ReferenceId = FilterReference.OfficialName,
            FilterValue = PersonMockedData.Person_3203_StGallen_1.OfficialName,
            FilterOperator = FilterOperatorType.Equals,
            FilterType = FilterDataType.String,
        });

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => Export(request));
        ex.InnerException?.InnerException?.Message.Should().Be("The application aborted the request.");
    }

    [Fact]
    public async Task WhenFilterByName_NotFound_ShouldReturnNoContent()
    {
        var request = new PersonSearchParametersExportModel();

        request.Criteria.Add(new()
        {
            ReferenceId = FilterReference.OfficialName,
            FilterValue = "No one has this name",
            FilterOperator = FilterOperatorType.Equals,
            FilterType = FilterDataType.String,
        });

        var response = await SgManualExporterClient.PostAsJsonAsync(Route, request);
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task WhenNoFilter_ShouldThrowBadRequest()
    {
        var request = new PersonSearchParametersExportModel();
        var response = await SgManualExporterClient.PostAsJsonAsync(Route, request);
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenFilterIdAndNoVersionId_ShouldResolvePersonsExportEch()
    {
        var request = new PersonSearchParametersExportModel
        {
            FilterId = FilterMockedData.SomeFilter_MunicipalityIdOther2.Id,
        };

        var xmlContent = await Export(request);
        xmlContent.MatchFormattedXmlSnapshot();
    }

    [Fact]
    public async Task WhenFilterIdAndNoVersionIdWithSwissAbroadVotingPlace_ShouldResolvePersonsExportEch()
    {
        var request = new PersonSearchParametersExportModel
        {
            FilterId = FilterMockedData.SomeFilter_MunicipalityId9170_SwissAbroad.Id,
        };

        var xmlContent = await Export(request);
        xmlContent.MatchFormattedXmlSnapshot();
    }

    [Fact]
    public async Task WhenFilterIdAndNoVersionIdWithManipulatedPerson_ShouldThrow()
    {
        await ManipulatePerson();

        var request = new PersonSearchParametersExportModel
        {
            FilterId = FilterMockedData.SomeFilter_MunicipalityIdOther2.Id,
        };

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => Export(request));
        ex.InnerException?.InnerException?.Message.Should().Be("The application aborted the request.");
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
    public async Task WhenVersionIdWithManipulatedPerson_ShouldThrow()
    {
        await ManipulatePerson();

        var request = new PersonSearchParametersExportModel
        {
            VersionId = FilterVersionMockedData.SomeFilterVersion_MunicipalityIdOther2.Id,
        };

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => Export(request));
        ex.InnerException?.InnerException?.Message.Should().Be("The application aborted the request.");
    }

    protected override Task<HttpResponseMessage> AuthorizationTestCall(HttpClient httpClient)
        => httpClient.PostAsJsonAsync(Route, new PersonSearchParametersExportModel { VersionId = FilterVersionMockedData.SomeFilterVersion_MunicipalityId.Id });

    private async Task<string> Export(PersonSearchParametersExportModel request)
    {
        var response = await SgManualExporterClient.PostAsJsonAsync(Route, request);
        response.EnsureSuccessStatusCode();

        var responseXml = await response.Content.ReadAsStringAsync();
        XmlUtil.ValidateSchema(responseXml, Ech0045Schemas.LoadEch0045Schemas());
        return responseXml;
    }

    private Task ManipulatePerson()
    {
        return RunOnDb(async db =>
        {
            var person = await db.FilterVersionPersons
                .IgnoreQueryFilters()
                .AsTracking()
                .Where(x => x.Person!.RegisterId == PersonMockedData.Person_3203_StGallen_1.RegisterId)
                .Select(x => x.Person)
                .FirstAsync();
            person!.FirstName += "-modified";
            await db.SaveChangesAsync();
        });
    }
}
