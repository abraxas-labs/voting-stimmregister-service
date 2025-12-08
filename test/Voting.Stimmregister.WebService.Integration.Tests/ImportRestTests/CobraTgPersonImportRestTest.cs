// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

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

public class CobraTgPersonImportRestTest : BaseImportRestTest
{
    private readonly IImportStatisticRepository _importStatisticRepository;
    private readonly ImportsConfig _importConfig;

    public CobraTgPersonImportRestTest(TestApplicationFactory factory)
        : base(factory)
    {
        _importStatisticRepository = GetService<IImportStatisticRepository>();
        _importConfig = GetService<ImportsConfig>();
    }

    protected override string ImportEndpointUrl => "v1/import/cobra-tg/persons";

    protected override string ValidFileName => "Cobra_Tg_Person_valid.csv";

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
        _importConfig.SupportedImportSourceSystemByCanton[Canton.TG].TryAdd(ImportSourceSystem.CobraTg);
    }

    [Fact]
    public async Task ShouldQueueImport()
    {
        using var response = await PostFile(TgManualImporterClient, GetTestFilePath(ValidFileName));
        response.EnsureSuccessStatusCode();

        var importStats = await _importStatisticRepository.Query()
            .Where(i => i.FileName.Equals(ValidFileName))
            .SingleAsync();

        importStats.AcmEncryptedAesKey = [];
        importStats.AcmAesIv = [];
        importStats.AcmEncryptedMacKey = [];
        importStats.AcmHmac = [];

        importStats.MatchSnapshot(i => i.Id, i => i.QueuedFileName);
    }

    [Fact]
    public async Task ShouldForbidUnsupportedSourceSystemImport()
    {
        _importConfig.SupportedImportSourceSystemByCanton[Canton.TG].Remove(ImportSourceSystem.CobraTg);
        using var response = await PostFile(ManualImporterClient, GetTestFilePath(ValidFileName));
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
