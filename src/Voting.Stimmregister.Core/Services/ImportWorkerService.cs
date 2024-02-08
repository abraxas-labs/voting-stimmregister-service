// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Models;
using Voting.Stimmregister.Core.Services.Supporting.Signing;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Diagnostics;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Services;

/// <summary>
/// Service which works on pending data import jobs.
/// </summary>
public class ImportWorkerService
{
    private readonly IImportStatisticRepository _repo;
    private readonly IClock _clock;
    private readonly IStreamDecryptionService _streamDecryptionService;
    private readonly ImportsConfig _config;
    private readonly ImportFileService _importFileService;
    private readonly ImportServiceRegistry _registry;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDataContext _dataContext;

    public ImportWorkerService(
        IImportStatisticRepository repo,
        ImportsConfig config,
        ImportFileService importFileService,
        ImportServiceRegistry registry,
        IServiceScopeFactory scopeFactory,
        IClock clock,
        IStreamDecryptionService streamDecryptionService,
        IDataContext dataContext)
    {
        _repo = repo;
        _config = config;
        _importFileService = importFileService;
        _registry = registry;
        _scopeFactory = scopeFactory;
        _clock = clock;
        _streamDecryptionService = streamDecryptionService;
        _dataContext = dataContext;
    }

    internal async Task ProcessImports(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            if (!await ProcessJob())
            {
                return;
            }
        }
    }

    private async Task<bool> ProcessJob()
    {
        var job = await FetchJob();
        if (job == null)
        {
            return false;
        }

        DiagnosticsConfig.DecreaseQueuedJobs();

        await using var importScope = _scopeFactory.CreateAsyncScope();
        try
        {
            var importService = _registry.GetImportService(importScope.ServiceProvider, job.SourceSystem, job.ImportType);
            if (importService != null)
            {
                var aesCipherMetadata = new AesCipherMetadata(
                    job.AcmAesIv,
                    job.AcmEncryptedAesKey,
                    job.AcmEncryptedMacKey,
                    job.AcmHmac);

                await using var fileContent = await _importFileService.OpenFile(job.QueuedFileName, aesCipherMetadata);
                await importService.RunImport(new ImportDataModel(job.Id, job.AuditInfo.CreatedAt, job.FileName, fileContent, job.SourceSystem));
                return true;
            }

            await _repo.UpdateFinishedWithProcessingErrors(
                job.Id,
                $"No importer found for {job.SourceSystem}/{job.ImportType}",
                new List<RecordValidationErrorModel>(),
                new List<Guid>(),
                null,
                _clock.UtcNow);
        }
        catch (Exception ex)
        {
            await _repo.UpdateFinishedWithProcessingErrors(
                job.Id,
                $"Unexpected exception {ex}",
                new List<RecordValidationErrorModel>(),
                new List<Guid>(),
                null,
                _clock.UtcNow);
        }
        finally
        {
            _importFileService.TryDeleteFile(job.QueuedFileName);
        }

        return true;
    }

    private async Task<ImportStatisticEntity?> FetchJob()
    {
        await using var transaction = await _dataContext.BeginTransaction();
        var job = await _repo.FetchQueuedForUpdate();
        if (job == null)
        {
            await transaction.CommitAsync();
            return null;
        }

        job.ImportStatus = ImportStatus.Running;
        job.WorkerName = _config.WorkerName;
        job.StartedDate = _clock.UtcNow;
        await _repo.Update(job);

        await transaction.CommitAsync();
        return job;
    }
}
