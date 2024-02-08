// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;

/// <summary>
/// Repository for domain of influences.
/// </summary>
public interface IDomainOfInfluenceRepository : IDbRepository<DbContext, DomainOfInfluenceEntity>
{
    /// <summary>
    /// Gets a <see cref="List{DomainOfInfluenceEntity}"/> which have the municipality id (BFS number) in common.
    /// </summary>
    /// <param name="municipalityId">The municipality id (BFS number).</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
    Task<List<DomainOfInfluenceEntity>> GetDomainOfInfluencesByBfsNumber(int municipalityId);

    /// <summary>
    /// Gets a <see cref="Dictionary{TKey, TValue}"/> of domain of influences by id, which have the passed municipality id (BFS number) in common.
    /// </summary>
    /// <param name="municipalityId">The municipality id (BFS number).</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
    Task<Dictionary<int, DomainOfInfluenceEntity>> GetDomainOfInfluencesByIdForBfsNumber(int municipalityId);
}
