// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Voting.Lib.Common;
using Voting.Stimmregister.Core.Services.Supporting.Signing;
using Voting.Stimmregister.Core.Services.Supporting.Signing.Exceptions;
using Voting.Stimmregister.Domain.Configuration;

namespace Voting.Stimmregister.Core.Services;

public class ImportFileService
{
    private readonly ImportsConfig _config;
    private readonly ILogger<ImportFileService> _logger;
    private readonly IClock _clock;
    private readonly IStreamEncryptionService _streamEncryptionService;
    private readonly IStreamDecryptionService _streamDecryptionService;

    public ImportFileService(
        ImportsConfig config,
        ILogger<ImportFileService> logger,
        IClock clock,
        IStreamEncryptionService streamEncryptionService,
        IStreamDecryptionService streamDecryptionService)
    {
        _config = config;
        _logger = logger;
        _clock = clock;
        _streamEncryptionService = streamEncryptionService;
        _streamDecryptionService = streamDecryptionService;
        Directory.CreateDirectory(config.ImportFileQueueDirectory);
    }

    internal async Task<(string QueuedFileName, AesCipherMetadata AesCipherMetadata)> StoreDataInTempFile(Guid id, string fileName, Stream data)
    {
        var queuedFileName = BuildFileName(id, fileName);
        try
        {
            var file = CreateFile(queuedFileName);
            var (encryptedStream, aesCipherMetadata) = _streamEncryptionService.CreateAesMacEncryptCryptoStream(file);
            await using var compressionStream = new GZipStream(encryptedStream, CompressionLevel.Fastest);
            await data.CopyToAsync(compressionStream);
            return (queuedFileName, aesCipherMetadata);
        }
        catch (Exception)
        {
            TryDeleteFile(queuedFileName);
            throw;
        }
    }

    internal async Task<Stream> OpenFile(string queuedFileName, AesCipherMetadata aesCipherMetadata)
    {
        var encryptedFileContent = File.OpenRead(Path.Join(_config.ImportFileQueueDirectory, queuedFileName));

        if (!await _streamDecryptionService.VerifyHMAC(encryptedFileContent, aesCipherMetadata))
        {
            throw new DecryptionException("HMAC verification failed for file " + queuedFileName);
        }

        var compressedStream = _streamDecryptionService.CreateAesMacDecryptCryptoStream(encryptedFileContent, aesCipherMetadata);
        return new GZipStream(compressedStream, CompressionMode.Decompress);
    }

    internal void TryDeleteFile(string queuedFileName)
    {
        var file = Path.Join(_config.ImportFileQueueDirectory, queuedFileName);
        if (!File.Exists(file))
        {
            return;
        }

        try
        {
            File.Delete(file);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not delete temporary file");
        }
    }

    protected virtual string BuildFileName(Guid id, string importFileName)
        => $"{_clock.UtcNow:yyyyMMddHHmmss}-{id}{Path.GetExtension(importFileName)}.gz.encrypted";

    private Stream CreateFile(string queuedFileName)
        => File.Create(Path.Join(_config.ImportFileQueueDirectory, queuedFileName));
}
