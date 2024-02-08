// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Iam.Exceptions;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Core.Services;
using Voting.Stimmregister.Core.Services.Supporting.Signing;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Diagnostics;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Core.Queues;

/// <summary>
/// Import queue to enqueue data import jobs.
/// </summary>
public class ImportQueue
{
    private readonly ImportsConfig _importConfig;
    private readonly IImportStatisticRepository _importRepository;
    private readonly IPermissionService _permissionService;
    private readonly ImportFileService _importFileService;
    private readonly ImportWorkerTrigger _trigger;
    private readonly ImportServiceRegistry _importServiceRegistry;
    private readonly IServiceScopeFactory _scopeFactory;

    public ImportQueue(
        ImportsConfig importConfig,
        IImportStatisticRepository importRepository,
        IPermissionService permissionService,
        ImportFileService importFileService,
        ImportWorkerTrigger trigger,
        ImportServiceRegistry importServiceRegistry,
        IServiceScopeFactory scopeFactory)
    {
        _importConfig = importConfig;
        _importRepository = importRepository;
        _permissionService = permissionService;
        _importFileService = importFileService;
        _trigger = trigger;
        _importServiceRegistry = importServiceRegistry;
        _scopeFactory = scopeFactory;
    }

    internal async Task<ImportStatisticEntity> Enqueue(
        ImportType type,
        ImportSourceSystem sourceSystem,
        string fileName,
        Stream data)
    {
        var id = Guid.NewGuid();

        var (queuedFileName, aesCipherMetadata) = await _importFileService.StoreDataInTempFile(id, fileName, data);

        var municipalityId = await GetMunicipalityIdAndAssertPermission(type, sourceSystem, queuedFileName, aesCipherMetadata);

        var import = new ImportStatisticEntity
        {
            Id = id,
            MunicipalityId = municipalityId,
            FileName = fileName,
            QueuedFileName = queuedFileName,
            ImportStatus = ImportStatus.Queued,
            ImportType = type,
            SourceSystem = sourceSystem,
            AcmEncryptedAesKey = aesCipherMetadata.EncryptedAesKey,
            AcmAesIv = aesCipherMetadata.AesIv,
            AcmEncryptedMacKey = aesCipherMetadata.EncryptedMacKey,
            AcmHmac = aesCipherMetadata.Hmac,
            IsManualUpload = !_permissionService.IsServiceUser(),
            IsLatest = true,
        };

        _permissionService.SetCreated(import);
        await _importRepository.CreateAndUpdateIsLatest(import);

        DiagnosticsConfig.IncreaseQueuedJobs();
        _trigger.TriggerImportWorker();
        return import;
    }

    private async Task<int> GetMunicipalityIdAndAssertPermission(
        ImportType type,
        ImportSourceSystem sourceSystem,
        string queuedFileName,
        AesCipherMetadata aesCipherMetadata)
    {
        try
        {
            var municipalityId = await PeekMunicipalityIdFromFile(type, sourceSystem, queuedFileName, aesCipherMetadata);

            if (!IsUserAuthorized(municipalityId, sourceSystem))
            {
                throw new ForbiddenException("User is not allowed to import this file");
            }

            return municipalityId;
        }
        catch (Exception e)
        {
            _importFileService.TryDeleteFile(queuedFileName);
            throw new ForbiddenException(e.Message);
        }
    }

    private async Task<int> PeekMunicipalityIdFromFile(ImportType type, ImportSourceSystem sourceSystem, string fileName, AesCipherMetadata aesCipherMetadata)
    {
        await using var fileContent = await _importFileService.OpenFile(fileName, aesCipherMetadata);

        await using var importScope = _scopeFactory.CreateAsyncScope();
        var importService = _importServiceRegistry.GetImportService(importScope.ServiceProvider, sourceSystem, type);
        ArgumentNullException.ThrowIfNull(importService);

        var municipalityId = await importService.PeekMunicipalityIdFromStream(fileContent);
        if (municipalityId is null or 0)
        {
            throw new ForbiddenException("Unable to find BFS number from file");
        }

        return municipalityId.Value;
    }

    /// <summary>
    /// Indicates whether the import user is authorized for the municipality id by checking
    /// the Access Control List and
    /// the imports config whitelist in case of swiss foreigners source system.
    /// </summary>
    /// <param name="municipalityId">The municipality id to lookup in the users ACL.</param>
    /// <param name="sourceSystem">The source system.</param>
    /// <returns>True if the user is authorized.</returns>
    private bool IsUserAuthorized(int municipalityId, ImportSourceSystem sourceSystem)
    {
        if (_permissionService.IsServiceUser())
        {
            return true;
        }

        return _permissionService.BfsAccessControlList.Contains(municipalityId.ToString())
            && (sourceSystem != ImportSourceSystem.Cobra || _importConfig.SwissAbroadMunicipalityIdWhitelist.Contains(municipalityId.ToString()));
    }
}
