// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Voting.Lib.Common;
using Voting.Stimmregister.Core.Services;
using Voting.Stimmregister.Core.Services.Supporting.Signing;
using Voting.Stimmregister.Domain.Configuration;

namespace Voting.Stimmregister.Test.Utils.Mocks;

public class MockImportFileService : ImportFileService
{
    private static int _counter;

    public MockImportFileService(
        ImportsConfig config,
        ILogger<MockImportFileService> logger,
        IClock clock,
        IStreamEncryptionService streamEncryptionService,
        IStreamDecryptionService streamDecryptionService)
        : base(config, logger, clock, streamEncryptionService, streamDecryptionService)
    {
    }

    protected override string BuildFileName(Guid id, string importFileName)
        => $"{_counter++}{Path.GetExtension(importFileName)}";
}
