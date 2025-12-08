// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper.Internal;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.ImportRestTests;

public class CobraPersonImportRestTest : BaseImportRestTest
{
    private readonly IImportStatisticRepository _importStatisticRepository;
    private readonly ImportsConfig _importConfig;

    public CobraPersonImportRestTest(TestApplicationFactory factory)
        : base(factory)
    {
        _importStatisticRepository = GetService<IImportStatisticRepository>();
        _importConfig = GetService<ImportsConfig>();
    }

    protected override string ImportEndpointUrl => "v1/import/cobra/persons";

    protected override string ValidFileName => "Cobra_Person_valid.csv";

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
        _importConfig.SupportedImportSourceSystemByCanton[Canton.SG].TryAdd(ImportSourceSystem.Cobra);
    }

    [Fact]
    public async Task ShouldQueueImport()
    {
        using var response = await PostFile(ManualImporterClient, GetTestFilePath(ValidFileName));
        response.EnsureSuccessStatusCode();

        var importStats = await _importStatisticRepository.Query()
            .Where(i => i.FileName.Equals(ValidFileName))
            .SingleAsync();

        importStats.AcmEncryptedAesKey = Array.Empty<byte>();
        importStats.AcmAesIv = Array.Empty<byte>();
        importStats.AcmEncryptedMacKey = Array.Empty<byte>();
        importStats.AcmHmac = Array.Empty<byte>();

        importStats.MatchSnapshot(i => i.Id, i => i.QueuedFileName);
    }

    [Fact]
    public async Task ShouldForbidUnsupportedSourceSystemImport()
    {
        _importConfig.SupportedImportSourceSystemByCanton[Canton.SG].Remove(ImportSourceSystem.Cobra);
        using var response = await PostFile(ManualImporterClient, GetTestFilePath(ValidFileName));
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
