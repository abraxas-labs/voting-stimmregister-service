// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;

/// <summary>
/// Repository for import statistics.
/// </summary>
public interface IImportStatisticRepository : IDbRepository<DbContext, ImportStatisticEntity>
{
    /// <summary>
    /// Finds the <see cref="ImportStatisticEntity"/> matching the 'importId' and updates the <see cref="ImportStatisticEntity.MunicipalityId"/> with the passed 'importStautsId'.
    /// </summary>
    /// <param name="importId">The import id for resolving the import entity.</param>
    /// <param name="municipalityId">The municipality id to set.</param>
    /// <param name="municipalityName">The municipality name to set.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task UpdateMunicipalityId(Guid importId, int? municipalityId, string? municipalityName);

    /// <summary>
    /// Fetches a queued import and locks it for an update.
    /// </summary>
    /// <returns>The found entity or null if none was found.</returns>
    Task<ImportStatisticEntity?> FetchQueuedForUpdate();

    /// <summary>
    /// Stores a new <see cref="ImportStatisticEntity"/>
    /// and sets <see cref="ImportStatisticEntity.IsLatest"/>
    /// of the old entry to false.
    /// </summary>
    /// <param name="import">The entity to create.</param>
    /// <returns>An <see cref="Task"/> representing the async operation.</returns>
    Task CreateAndUpdateIsLatest(ImportStatisticEntity import);

    /// <summary>
    /// Updates the import statistics for an <see cref="ImportStatisticEntity"/> matching the 'importId' in case of processing errors.
    /// </summary>
    /// <param name="importId">The import id for resolving the import entity.</param>
    /// <param name="processingErrors">Processing errors description.</param>
    /// <param name="recordValidationErrors">The record validations dictionary.</param>
    /// <param name="entityIdsWithValidationErrors">The ids of entities with validation errors.</param>
    /// <param name="municipalityId">The municipality id.</param>
    /// <param name="errorTimestamp">Timestamp when the error occured.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task UpdateFinishedWithProcessingErrors(
        Guid importId,
        string processingErrors,
        List<RecordValidationErrorModel> recordValidationErrors,
        List<Guid> entityIdsWithValidationErrors,
        int? municipalityId,
        DateTime errorTimestamp);
}
