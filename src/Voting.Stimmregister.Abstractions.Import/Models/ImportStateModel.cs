// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Voting.Lib.Database.Models;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Import.Models;

public abstract class ImportStateModel<TEntity>
    where TEntity : BaseEntity
{
    /// <summary>
    /// Gets the import id.
    /// </summary>
    public Guid ImportStatisticId { get; init; }

    /// <summary>
    /// Gets or sets the municipality id, which must be identical for every entity within the import file.
    /// </summary>
    public int? MunicipalityId { get; set; }

    /// <summary>
    /// Gets or sets the municipality name.
    /// </summary>
    public string? MunicipalityName { get; set; }

    /// <summary>
    /// Gets or sets the canton bfs number.
    /// </summary>
    public short? CantonBfs { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the import state is initialized.
    /// </summary>
    public bool Initialized { get; set; }

    /// <summary>
    /// Gets the timestamp when the import data was received (eg. when the import statistics was created).
    /// </summary>
    public DateTime DataReceivedTimestamp { get; init; }

    /// <summary>
    /// Gets or sets the counter for processed records.
    /// </summary>
    public int RecordsCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether any records have validation errors.
    /// </summary>
    public bool HasRecordValidationErrors { get; set; }

    /// <summary>
    /// Gets or sets the Count of the new imported entities.
    /// </summary>
    public int CreateCount { get; protected set; }

    /// <summary>
    /// Gets or sets the Count of the updated entities.
    /// </summary>
    public int UpdateCount { get; protected set; }

    /// <summary>
    /// Gets or sets the Count of the deleted entities.
    /// </summary>
    public int DeleteCount { get; protected set; }

    /// <summary>
    /// Gets a list of record ids containing validation errors.
    /// </summary>
    public List<int> RecordIdsWithValidationErrors { get; } = new();

    /// <summary>
    /// Gets a list of record validation error models.
    /// </summary>
    public List<RecordValidationErrorModel> RecordValidationErrors { get; } = new();

    /// <summary>
    /// Gets a list of entity ids containing validation errors.
    /// </summary>
    public List<Guid> EntityIdsWithValidationErrors { get; } = new();

    /// <summary>
    /// Gets a list of new entities which should be created in the db set.
    /// </summary>
    public HashSet<TEntity> EntitiesToCreate { get; } = new();

    /// <summary>
    /// Gets a list of entities which should be updated in the db set.
    /// </summary>
    public HashSet<TEntity> EntitiesToUpdate { get; } = new();

    /// <summary>
    /// Gets a list of entities which should be deleted in the db set.
    /// </summary>
    public HashSet<TEntity> EntitiesToDelete { get; } = new();

    /// <summary>
    /// Gets a list of entities which were not changed during import.
    /// These must be part of the integrity checksum.
    /// </summary>
    public HashSet<TEntity> EntitiesUnchanged { get; } = new();

    /// <summary>
    /// Gets or sets the import source system.
    /// </summary>
    public ImportSourceSystem ImportSourceSystem { get; set; }

    /// <summary>
    /// Creates an entity by adding it to the new persons list.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    public void Create(TEntity entity)
    {
        EntitiesToCreate.Add(entity);
        CreateCount++;
    }

    /// <summary>
    /// Adds the passed entity to the unchanged list, which is required for the integrity checksum.
    /// </summary>
    /// <param name="entity">The entity to add to the unchanged list.</param>
    public void SetUnchanged(TEntity entity)
        => EntitiesUnchanged.Add(entity);

    public abstract void SetProcessed(TEntity entity);

    public abstract void Update(TEntity updatedEntity, TEntity oldEntity);
}
