// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;

namespace Voting.Stimmregister.Domain.Models;

/// <summary>
/// Audit information of an entity.
/// </summary>
public class AuditInfo
{
    /// <summary>
    /// Gets or sets the user's id of the creator of this entity.
    /// </summary>
    public string CreatedById { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user's name of the creator of this entity.
    /// </summary>
    public string CreatedByName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the timestamp when this entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the user's id of the latest modifier of this entity.
    /// </summary>
    public string? ModifiedById { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user's name of the latest modifier of this entity.
    /// </summary>
    public string? ModifiedByName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the timestamp when the last modification of this entity happened.
    /// </summary>
    public DateTime? ModifiedAt { get; set; } = null!;

    public AuditInfo Clone()
        => (AuditInfo)MemberwiseClone();
}
