// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.StaticFiles;
using Voting.Lib.Grpc;
using Voting.Stimmregister.Core.Services.SecondFactor;
using Voting.Stimmregister.Domain.Authorization;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Proto.V1.Services;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;
using static Voting.Stimmregister.Test.Utils.Helpers.RessourceHelper;

namespace Voting.Stimmregister.WebService.Integration.Tests.ImportRestTests;

public abstract class BaseImportRestTest : BaseWriteableDbRestTest
{
    private const string SecondFactorTransactionIdHeader = "X-Second-Factor-Transaction-Id";

    private static readonly string _testFilesPath = Path.Combine("ImportRestTests", "_files");

    private Guid? _verifiedTransactionId;

    protected BaseImportRestTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    protected abstract string ImportEndpointUrl { get; }

    protected abstract string ValidFileName { get; }

    /// <summary>
    /// Gets the tenant the import (and therefore the second factor transaction) is performed for.
    /// Must match the tenant of the client used to upload the file.
    /// </summary>
    protected virtual string ImportTenantId => VotingIamTenantIds.KTSG;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _verifiedTransactionId = await PrepareSecondFactorTransaction();
    }

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
        return await PostFile(client, file, Path.GetFileName(testFile), _verifiedTransactionId!.Value);
    }

    protected async Task<HttpResponseMessage> PostFile(HttpClient client, string testFile, Guid secondFactorTransactionId)
    {
        await using var file = File.OpenRead(testFile);
        return await PostFile(client, file, Path.GetFileName(testFile), secondFactorTransactionId);
    }

    protected string GetTestFilePath(string fileName)
        => GetFullPathToFile(Path.Combine(_testFilesPath, fileName));

    private async Task<Guid> PrepareSecondFactorTransaction()
    {
        var channel = CreateGrpcChannel(true, tenant: ImportTenantId, roles: [Roles.ManualImporter]);
        var client = new ImportService.ImportServiceClient(channel);
        var response = await client.PrepareManualImportAsync(ProtobufEmpty.Instance);
        var transactionId = Guid.Parse(response.SecondFactorTransaction.Id);

        // The pinned mock does not persist the created transaction. Seed a verified transaction manually,
        // bound to the action/tenant the import is performed for (matching the upload's action id hash).
        var actionIdHash = ManualImportSecondFactorActionId.Create(ImportTenantId).ComputeHash();
        await RunOnDb(db =>
        {
            db.SecondFactorTransactions.Add(new SecondFactorTransactionEntity
            {
                Id = transactionId,
                ActionIdHash = actionIdHash,
                Verified = true,
                Used = false,
            });
            return db.SaveChangesAsync();
        });

        return transactionId;
    }

    private async Task<HttpResponseMessage> PostFile(HttpClient client, Stream file, string fileName, Guid secondFactorTransactionId)
    {
        using var fileContent = new StreamContent(file);
        var provider = new FileExtensionContentTypeProvider();
        if (provider.TryGetContentType(fileName, out var contentType))
        {
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        }

        using var content = new MultipartFormDataContent();
        content.Add(fileContent, "file", fileName);

        using var request = new HttpRequestMessage(HttpMethod.Post, ImportEndpointUrl);
        request.Content = content;
        request.Headers.Add(SecondFactorTransactionIdHeader, secondFactorTransactionId.ToString());
        return await client.SendAsync(request);
    }
}
