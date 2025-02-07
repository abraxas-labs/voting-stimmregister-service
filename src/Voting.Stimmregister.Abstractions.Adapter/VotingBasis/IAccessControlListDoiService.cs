// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Threading.Tasks;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Adapter.VotingBasis;

/// <summary>
/// Provides services to evaluate domain of influence based access control list.
/// </summary>
public interface IAccessControlListDoiService
{
    /// <summary>
    /// Gets a list of all inherited access control list entities for a specific tenant id.
    /// </summary>
    /// <param name="tenantId">The tenant id filter.</param>
    /// <returns>List of access control entities.</returns>
    Task<IReadOnlyCollection<AccessControlListDoiEntity>> GetDoiAccessControlListByTenantId(string tenantId);

    /// <summary>
    /// Gets a list of all inherited bfs access control list numbers for a specific tenant id and one or more domain of influence type filters.
    /// </summary>
    /// <param name="tenantId">The tenant id filter.</param>
    /// <param name="doiTypes">The domain of influence types to be filtered by.</param>
    /// <returns>List of authorized bfs numbers filtered by tenant id and doi types.</returns>
    Task<IReadOnlyCollection<string>> GetBfsNumberAccessControlListByTenantId(string tenantId, params DomainOfInfluenceType[] doiTypes);

    /// <summary>
    /// Returns a list of acl entries for a given bfs.
    /// </summary>
    /// <param name="bfs">The bfs.</param>
    /// <returns>A list of acl entries.</returns>
    Task<IReadOnlyCollection<AccessControlListDoiEntity>> GetValidEntriesForBfs(string bfs);

    /// <summary>
    /// Returns the acl entry for a given bfs number including all its parent nodes.
    /// The parent nodes represent domain of influences at a higher level, where the
    /// parent root node is equal to the political superior authority (de: Oberbehörde).
    /// </summary>
    /// <param name="bfs">The bfs number.</param>
    /// <returns>A list of acl entries.</returns>
    Task<IReadOnlyCollection<AccessControlListDoiEntity>> GetEntriesForBfsIncludingParents(string bfs);
}
