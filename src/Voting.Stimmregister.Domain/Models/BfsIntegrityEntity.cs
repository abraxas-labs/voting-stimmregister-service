// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Stimmregister.Domain.Enums;

namespace Voting.Stimmregister.Domain.Models;

public class BfsIntegrityEntity : BaseEntityWithSignature, IAuditedEntity
{
    /// <summary>
    /// Gets or sets the bfs / municipality id.
    /// </summary>
    public string Bfs { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the import type.
    /// </summary>
    public ImportType ImportType { get; set; }

    /// <summary>
    /// Gets or sets the audit info of this entity.
    /// </summary>
    public AuditInfo AuditInfo { get; set; } = new();

    /// <summary>
    /// Gets or sets the last updated date.
    /// </summary>
    public DateTime LastUpdated { get; set; }
}
