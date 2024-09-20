// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.StaticFiles;
using Voting.Stimmregister.Domain.Authorization;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;
using static Voting.Stimmregister.Test.Utils.Helpers.RessourceHelper;

namespace Voting.Stimmregister.WebService.Integration.Tests.ImportRestTests;

public abstract class BaseImportRestTest : BaseWriteableDbRestTest
{
    private static readonly string _testFilesPath = Path.Combine("ImportRestTests", "_files");

    protected BaseImportRestTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    protected abstract string ImportEndpointUrl { get; }

    protected abstract string ValidFileName { get; }

    [Fact]
    public async Task ShouldThrowNoFile()
    {
        using var content = new MultipartFormDataContent();
        using var response = await ApiImporterClient.PostAsync(ImportEndpointUrl, content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenRoleUnauthorizedShouldReturnForbidden()
    {
        var rolesArray = new[]
        {
            Roles.Reader,
            Roles.Manager,
            Roles.ApiExporter,
            Roles.ManualExporter,
            Roles.ImportObserver,
        };
        var client = CreateHttpClient(true, tenant: VotingIamTenantIds.KTSG, roles: rolesArray);
        using var content = new MultipartFormDataContent();
        using var response = await client.PostAsync(ImportEndpointUrl, content);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    protected override Task<HttpResponseMessage> AuthorizationTestCall(HttpClient httpClient)
        => PostFile(httpClient, GetTestFilePath(ValidFileName));

    protected async Task<HttpResponseMessage> PostFile(HttpClient client, string testFile)
    {
        await using var file = File.OpenRead(testFile);
        return await PostFile(client, file, Path.GetFileName(testFile));
    }

    protected string GetTestFilePath(string fileName)
        => GetFullPathToFile(Path.Combine(_testFilesPath, fileName));

    private async Task<HttpResponseMessage> PostFile(HttpClient client, Stream file, string fileName)
    {
        using var fileContent = new StreamContent(file);
        var provider = new FileExtensionContentTypeProvider();
        if (provider.TryGetContentType(fileName, out var contentType))
        {
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        }

        using var content = new MultipartFormDataContent();
        content.Add(fileContent, "file", fileName);
        return await client.PostAsync(ImportEndpointUrl, content);
    }
}
