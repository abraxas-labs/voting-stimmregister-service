// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Linq;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Import.Models;

public class DomainOfInfluenceImportStateModel : ImportStateModel<DomainOfInfluenceEntity>
{
    /// <summary>
    /// Gets or sets a dictionary containing the existing domain of influence entities by DomainOfInfluenceId.
    /// </summary>
    private Dictionary<int, DomainOfInfluenceEntity> UnprocessedExistingDoisByDoiId { get; set; } = new();

    /// <summary>
    /// Initializes the existing domain of influences dictionaries.
    /// </summary>
    /// <param name="existingEntities">A dictionary containing the existing domain of influence entities by DomainOfInfluenceId.</param>
    public void InitializeExistingDomainOfInfluences(IEnumerable<DomainOfInfluenceEntity> existingEntities)
    {
        UnprocessedExistingDoisByDoiId = existingEntities.ToDictionary(x => x.DomainOfInfluenceId);
    }

    /// <summary>
    /// Finds a domain of influence by the passed id number.
    /// </summary>
    /// <param name="domainOfInfluenceId">The domain of influence id number.</param>
    /// <returns>A <see cref="DomainOfInfluenceEntity"/> if existing, or null if not.</returns>
    public DomainOfInfluenceEntity? FindDomainOfInfluence(int domainOfInfluenceId)
    {
        return domainOfInfluenceId == 0 || !UnprocessedExistingDoisByDoiId.TryGetValue(domainOfInfluenceId, out var domainOfInfluence)
            ? null
            : domainOfInfluence;
    }

    /// <summary>
    /// Deletes domain of influences, which are not part of the import records anymore.
    /// </summary>
    public void DeleteNotImported()
    {
        foreach (var entity in UnprocessedExistingDoisByDoiId.Values)
        {
            EntitiesToDelete.Add(entity);
            DeleteCount++;
        }
    }

    public override void SetProcessed(DomainOfInfluenceEntity entity)
        => UnprocessedExistingDoisByDoiId.Remove(entity.DomainOfInfluenceId);

    public override void Update(DomainOfInfluenceEntity updatedEntity, DomainOfInfluenceEntity oldEntity)
    {
        updatedEntity.Id = oldEntity.Id;
        EntitiesToUpdate.Add(updatedEntity);
        UpdateCount++;
    }
}
