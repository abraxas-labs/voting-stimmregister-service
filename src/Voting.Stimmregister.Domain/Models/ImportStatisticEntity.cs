// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Domain.Models;

/// <summary>
/// ImportAcl statistics entity which will be created for each import request.
/// </summary>
public class ImportStatisticEntity : AuditedEntity
{
    /// <summary>
    /// Gets or sets the file name as provided by the client.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the file in the temporary directory of files to be imported.
    /// </summary>
    public string QueuedFileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source system name.
    /// </summary>
    public ImportSourceSystem SourceSystem { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the import was a manual upload or not.
    /// </summary>
    public bool IsManualUpload { get; set; }

    /// <summary>
    /// Gets or sets the count of delivered import records within the import.
    /// </summary>
    public int ImportRecordsCountTotal { get; set; }

    /// <summary>
    /// Gets or sets the count of created datasets within the import.
    /// </summary>
    public int DatasetsCountCreated { get; set; }

    /// <summary>
    /// Gets or sets the count of updated datasets within the import.
    /// </summary>
    public int DatasetsCountUpdated { get; set; }

    /// <summary>
    /// Gets or sets the count of deleted datasets within this import.
    /// </summary>
    public int DatasetsCountDeleted { get; set; }

    /// <summary>
    /// Gets or sets the finished date, which represents when the import has been completed.
    /// </summary>
    public DateTime? FinishedDate { get; set; }

    /// <summary>
    /// Gets or sets the started date, when the import job has been started.
    /// </summary>
    public DateTime? StartedDate { get; set; }

    /// <summary>
    /// Gets or sets the total elapsed miliseconds during the import.
    /// </summary>
    public double? TotalElapsedMilliseconds
    {
        get => (FinishedDate - StartedDate)?.TotalMilliseconds;
        set => _ = value;
    }

    /// <summary>
    /// Gets or sets the municipality id, which represents the import scope for a domain of influence.
    /// </summary>
    public int? MunicipalityId { get; set; }

    /// <summary>
    /// Gets or sets the municipality name.
    /// </summary>
    public string? MunicipalityName { get; set; }

    /// <summary>
    /// Gets or sets processing error reasons if any occur.
    /// </summary>
    public string? ProcessingErrors { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the import has validation errors.
    /// </summary>
    public bool HasValidationErrors { get; set; }

    /// <summary>
    /// Gets or sets a list of entity ids which contain validation errors.
    /// </summary>
    public List<Guid> EntitiesWithValidationErrors { get; set; } = new();

    /// <summary>
    /// Gets or sets a list of record / row numbers which contain validation errors and therefore were skipped during import.
    /// </summary>
    public List<int> RecordNumbersWithValidationErrors { get; set; } = new();

    /// <summary>
    /// Gets or sets a json serialized value containing record validation errors if any exist.
    /// </summary>
    public string? RecordValidationErrors { get; set; }

    /// <summary>
    /// Gets or sets the import state.
    /// </summary>
    public ImportStatus ImportStatus { get; set; }

    /// <summary>
    /// Gets or sets the import type.
    /// </summary>
    public ImportType ImportType { get; set; }

    /// <summary>
    /// Gets or sets the name of the machine which worked on this import.
    /// </summary>
    public string WorkerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this entity is the latest of a given
    /// municipality id, import type and source system combination.
    /// </summary>
    public bool IsLatest { get; set; }

    /// <summary>
    /// Gets or sets the imported persons references.
    /// </summary>
    public ICollection<PersonEntity> Persons { get; set; } = new HashSet<PersonEntity>();

    /// <summary>
    /// Gets or sets the imported domain of influences references.
    /// </summary>
    public ICollection<DomainOfInfluenceEntity> DomainOfInfluences { get; set; } = new HashSet<DomainOfInfluenceEntity>();

    /// <summary>
    /// Gets or sets the access control DOI references.
    /// </summary>
    public ICollection<AccessControlListDoiEntity> AccessControlListDois { get; set; } = new HashSet<AccessControlListDoiEntity>();

    /// <summary>
    /// Gets or sets the encrypted aes key for data encryption.
    /// </summary>
    public byte[] AcmEncryptedAesKey { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Gets or sets the iv for aes encryption.
    /// </summary>
    public byte[] AcmAesIv { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Gets or sets the encrypted key for the MAC.
    /// </summary>
    public byte[] AcmEncryptedMacKey { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Gets or sets the MAC.
    /// </summary>
    public byte[] AcmHmac { get; set; } = Array.Empty<byte>();
}
