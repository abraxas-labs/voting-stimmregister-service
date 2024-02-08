// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

namespace Voting.Stimmregister.Domain.Models;

public class ImportedEntity : BaseEntityWithSignature
{
    /// <summary>
    /// Gets or sets the municipality id (BFS number), i.e. '3214'.
    /// </summary>
    public int MunicipalityId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity data has passed validation or not.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets validation error reasons if any occur.
    /// </summary>
    public string? ValidationErrors { get; set; }
}
