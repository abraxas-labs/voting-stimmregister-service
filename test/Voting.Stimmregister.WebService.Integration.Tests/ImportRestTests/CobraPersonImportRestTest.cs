﻿// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.ImportRestTests;

public class CobraPersonImportRestTest : BaseImportRestTest
{
    private readonly IImportStatisticRepository _importStatisticRepository;

    public CobraPersonImportRestTest(TestApplicationFactory factory)
        : base(factory)
    {
        _importStatisticRepository = GetService<IImportStatisticRepository>();
    }

    protected override string ImportEndpointUrl => "v1/import/cobra/persons";

    protected override string ValidFileName => "Cobra_Person_valid.csv";

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await AclDoiVotingBasisMockedData.Seed(RunScoped);
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
}
