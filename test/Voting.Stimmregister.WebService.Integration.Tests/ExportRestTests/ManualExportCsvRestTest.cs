// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Voting.Lib.Testing.Mocks;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Domain.Authorization;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.ExportRestTests;

public class ManualExportCsvRestTest : BaseWriteableDbRestTest
{
    private const string RouteController = "v1/export";
    private const string RouteExportCsv = "csv";

    public ManualExportCsvRestTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await MockDataSeeder.Seed(RunScoped);
    }

    [Fact]
    public async Task WhenFilterByName_ShouldResolvePersonExportCsv()
    {
        var request = new PersonSearchParametersExportModel();

        request.Criteria.Add(new()
        {
            ReferenceId = FilterReference.OfficialName,
            FilterValue = PersonMockedData.Person_3203_StGallen_1.OfficialName,
            FilterOperator = FilterOperatorType.Equals,
            FilterType = FilterDataType.String,
        });

        request.Criteria.Add(new()
        {
            ReferenceId = FilterReference.FirstName,
            FilterValue = PersonMockedData.Person_3203_StGallen_1.FirstName,
            FilterOperator = FilterOperatorType.Equals,
            FilterType = FilterDataType.String,
        });

        var records = await GetRecords(request);
        records.Any(e =>
                e.OfficialName == PersonMockedData.Person_3203_StGallen_1.OfficialName
                && e.FirstName == PersonMockedData.Person_3203_StGallen_1.FirstName)
            .Should()
            .BeTrue();
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

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => GetRecords(request));
        ex.InnerException?.InnerException?.Message.Should().Be("The application aborted the request.");
    }

    [Fact]
    public async Task WhenFilterByName_NotFound_ShouldThrowBadRequest()
    {
        var personSearchParametersCsvExportModel = new PersonSearchParametersExportModel
        {
            FilterId = Guid.Parse("{11111111-2222-3333-4444-555555555555}"), // invalid random guid
        };

        personSearchParametersCsvExportModel.Criteria.Add(new()
        {
            ReferenceId = FilterReference.OfficialName,
            FilterValue = "Muster",
            FilterOperator = FilterOperatorType.Equals,
            FilterType = FilterDataType.String,
        });

        var jsonContent = JsonConvert.SerializeObject(personSearchParametersCsvExportModel);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await SgManualExporterClient.PostAsync(Path.Combine(RouteController, RouteExportCsv), httpContent);
        Assert.Equal(StatusCodes.Status404NotFound, (int)response.StatusCode);
    }

    [Fact]
    public async Task WhenNoFilter_ShouldThrowBadRequest()
    {
        var response = await SgManualExporterClient.PostAsJsonAsync(Path.Combine(RouteController, RouteExportCsv), new PersonSearchParametersExportModel());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenFilterIDAndNoVersionID_ShouldResolvePersonsExportCsv()
    {
        var request = new PersonSearchParametersExportModel
        {
            FilterId = FilterMockedData.SomeFilter_MunicipalityIdOther2.Id,
        };

        var records = await GetRecords(request);
        records.MatchSnapshot();
    }

    [Fact]
    public async Task WhenFilterIDAndNoVersionIDWithManipulatedPerson_ShouldThrow()
    {
        await ManipulatePerson();

        var request = new PersonSearchParametersExportModel
        {
            FilterId = FilterMockedData.SomeFilter_MunicipalityIdOther2.Id,
        };

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => GetRecords(request));
        ex.InnerException?.InnerException?.Message.Should().Be("The application aborted the request.");
    }

    [Fact]
    public async Task WhenFilterIDAndVersionID_ShouldResolvePersonsExportCsv()
    {
        var personSearchParametersCsvExportModel = new PersonSearchParametersExportModel
        {
            FilterId = Guid.NewGuid(), // ignored since version id has higher precedence
            VersionId = FilterVersionMockedData.SomeFilterVersion_MunicipalityIdOther2.Id,
        };

        var records = await GetRecords(personSearchParametersCsvExportModel);
        records.MatchSnapshot();
    }

    [Fact]
    public async Task WhenVersionIdWithManipulatedPerson_ShouldThrow()
    {
        await ManipulatePerson();

        var request = new PersonSearchParametersExportModel
        {
            VersionId = FilterVersionMockedData.SomeFilterVersion_MunicipalityIdOther2.Id,
        };

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => GetRecords(request));
        ex.InnerException?.InnerException?.Message.Should().Be("The application aborted the request.");
    }

    [Fact]
    public async Task WhenUnauthorizedRole_ShouldReturnForbidden()
    {
        var request = new PersonSearchParametersExportModel();
        var rolesArray = new[]
        {
            Roles.Reader,
            Roles.Manager,
            Roles.ManualImporter,
            Roles.ApiImporter,
            Roles.ImportObserver,
            Roles.ApiExporter,
        };

        var client = CreateHttpClient(true, tenant: VotingIamTenantIds.KTSG, roles: rolesArray);
        var response = await client.PostAsJsonAsync(Path.Combine(RouteController, RouteExportCsv), request);
        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Forbidden);
    }

    protected override Task<HttpResponseMessage> AuthorizationTestCall(HttpClient httpClient)
    {
        return httpClient.PostAsJsonAsync(Path.Combine(RouteController, RouteExportCsv), new PersonSearchParametersExportModel());
    }

    private async Task<List<PersonCsvExportModel>> GetRecords(PersonSearchParametersExportModel request)
    {
        using var response = await SgManualExporterClient.PostAsJsonAsync(Path.Combine(RouteController, RouteExportCsv), request);
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentDisposition!.FileName.Should().Be("stimmregister-" + MockedClock.UtcNowDate.ToString("yyyy-MM-dd") + ".csv");

        await using var stream = await response.Content.ReadAsStreamAsync();
        var fileStreamResult = new FileStreamResult(stream, new MediaTypeHeaderValue("text/csv"));

        using var textReader = new StreamReader(fileStreamResult.FileStream, Encoding.UTF8);
        using var csvReader = new CsvReader(textReader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            Encoding = Encoding.UTF8,
        });

        return await csvReader.GetRecordsAsync<PersonCsvExportModel>().ToListAsync();
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
