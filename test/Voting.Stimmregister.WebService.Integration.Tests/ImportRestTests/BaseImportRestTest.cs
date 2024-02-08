﻿// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Voting.Stimmregister.Test.Utils.Helpers;
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
        using var content = new MultipartFormDataContent();
        content.Add(fileContent, "file", fileName);
        return await client.PostAsync(ImportEndpointUrl, content);
    }
}
