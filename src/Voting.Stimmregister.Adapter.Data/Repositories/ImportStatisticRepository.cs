// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Common;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Domain.Diagnostics;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Exceptions;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.Repositories;

/// <inheritdoc cref="IImportStatisticRepository"/>
public class ImportStatisticRepository : DbRepository<DataContext, ImportStatisticEntity>, IImportStatisticRepository
{
    private readonly IClock _clock;

    public ImportStatisticRepository(
        DataContext context,
        IClock clock)
        : base(context)
    {
        _clock = clock;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "EF1002:Risk of vulnerability to SQL injection.", Justification = "Referencing hardened inerpolated string parameters.")]
    public async Task<ImportStatisticEntity?> FetchQueuedForUpdate()
    {
        var importStatusColName = Set.GetDelimitedColumnName(x => x.ImportStatus);
        return await Set
            .FromSqlRaw($"SELECT * FROM {DelimitedSchemaAndTableName} WHERE {importStatusColName} = {{0}} LIMIT 1 FOR UPDATE SKIP LOCKED", nameof(ImportStatus.Queued))
            .FirstOrDefaultAsync();
    }

    public async Task UpdateMunicipalityId(Guid importId, int? municipalityId, string? municipalityName)
    {
        var importEntity = await GetByKey(importId)
            ?? throw new EntityNotFoundException(typeof(ImportStatisticEntity), importId);

        importEntity.MunicipalityId = municipalityId;
        importEntity.MunicipalityName = municipalityName;
        importEntity.AuditInfo.ModifiedAt = _clock.UtcNow;

        await Update(importEntity);
    }

    public async Task CreateAndUpdateIsLatest(ImportStatisticEntity import)
    {
        var currentLatest = await Set
            .Where(x => x.IsLatest && x.MunicipalityId == import.MunicipalityId && x.ImportType == import.ImportType && x.SourceSystem == import.SourceSystem)
            .AsTracking()
            .SingleOrDefaultAsync();

        if (currentLatest != null)
        {
            currentLatest.IsLatest = false;
        }

        import.IsLatest = true;
        Set.Add(import);
        await Context.SaveChangesAsync();
    }

    public async Task UpdateFinishedWithProcessingErrors(
        Guid importId,
        string processingErrors,
        List<RecordValidationErrorModel> recordValidationErrors,
        List<Guid> entityIdsWithValidationErrors,
        int? municipalityId,
        DateTime errorTimestamp)
    {
        var importEntity = await GetByKey(importId)
            ?? throw new EntityNotFoundException(typeof(ImportStatisticEntity), importId);

        importEntity.ImportStatus = ImportStatus.Failed;
        importEntity.ProcessingErrors = processingErrors;
        importEntity.AuditInfo.ModifiedAt = _clock.UtcNow;
        importEntity.HasValidationErrors = entityIdsWithValidationErrors.Count > 0;
        importEntity.EntitiesWithValidationErrors = entityIdsWithValidationErrors;
        importEntity.FinishedDate = errorTimestamp;
        importEntity.RecordValidationErrors = recordValidationErrors.Count > 0 ?
            JsonSerializer.Serialize(recordValidationErrors) :
            null;

        if (municipalityId.HasValue)
        {
            importEntity.MunicipalityId = municipalityId;
        }

        await Update(importEntity);

        DiagnosticsConfig.IncreaseProcessedImportJobs(
            importEntity.ImportType.ToString(),
            importEntity.ImportStatus.ToString());
    }
}
