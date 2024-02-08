// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Test.Utils.Helpers;
using Xunit;

namespace Voting.Stimmregister.WebService.Integration.Tests.ImportRestTests;

public class LogantoDomainOfInfluenceImportRestTest : BaseImportRestTest
{
    private readonly IImportStatisticRepository _importStatisticRepository;

    public LogantoDomainOfInfluenceImportRestTest(TestApplicationFactory factory)
        : base(factory)
    {
        _importStatisticRepository = GetService<IImportStatisticRepository>();
    }

    protected override string ImportEndpointUrl => "v1/import/loganto/doi";

    protected override string ValidFileName => "Loganto_DomainOfInfluence_valid.csv";

    [Fact]
    public async Task ShouldQueueImport()
    {
        using var response = await PostFile(ApiImporterClient, GetTestFilePath(ValidFileName));
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
