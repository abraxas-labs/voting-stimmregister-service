// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.VotingBasis;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.VotingBasis.Services;

/// <inheritdoc />
internal class AccessControlListDoiService : IAccessControlListDoiService
{
    private readonly IAccessControlListDoiRepository _aclDoiRepo;
    private readonly ILogger<AccessControlListDoiService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessControlListDoiService"/> class.
    /// </summary>
    /// <param name="aclDoiRepo">The access control list repository.</param>
    /// <param name="logger">The logger.</param>
    public AccessControlListDoiService(
        IAccessControlListDoiRepository aclDoiRepo,
        ILogger<AccessControlListDoiService> logger)
    {
        _aclDoiRepo = aclDoiRepo;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<string>> GetBfsNumberAccessControlListByTenantId(string tenantId, DomainOfInfluenceType[] doiTypes)
    {
        var bfsNumbers = (await GetDoiAccessControlListByTenantId(tenantId))
            .Where(b => !string.IsNullOrWhiteSpace(b.Bfs) && doiTypes.Contains(b.Type) && b.IsValid)
            .Select(b => b.Bfs!)
            .ToList();

        _logger.LogDebug(
            "Found {count} ({bfsNumbers}) inherited access control list BFS numbers for tenant {tenantId}",
            bfsNumbers.Count,
            string.Join(',', bfsNumbers),
            tenantId);

        return bfsNumbers;
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<AccessControlListDoiEntity>> GetDoiAccessControlListByTenantId(string tenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            throw new ArgumentNullException(nameof(tenantId));
        }

        return GetDoiAccessControlListByTenantIdInternal(tenantId);
    }

    /// <inheritdoc />
    public async Task<AccessControlListDoiEntity?> GetValidEntryForBfs(string bfs)
        => await _aclDoiRepo.Query().FirstOrDefaultAsync(x => x.Bfs == bfs && x.IsValid);

    public async Task<IReadOnlyCollection<AccessControlListDoiEntity>> GetEntriesForBfsIncludingParents(string bfs)
    {
        var entries = await _aclDoiRepo.Query().ToListAsync();
        BuildTree(entries);
        var entry = entries.Find(x => x.Bfs == bfs && x.IsValid);

        return entry == null
            ? Array.Empty<AccessControlListDoiEntity>()
            : GetFlattenParentsInclSelf(entry).ToList();
    }

    private async Task<IReadOnlyCollection<AccessControlListDoiEntity>> GetDoiAccessControlListByTenantIdInternal(string tenantId)
    {
        var allAcls = await _aclDoiRepo.Query().ToListAsync();
        BuildTree(allAcls);

        var assignedAcls = allAcls.Where(a => a.TenantId == tenantId);
        var inheritedAclsWithDuplicates = assignedAcls.SelectMany(GetFlattenChildrenInclSelf);
        var inheritedAcls = inheritedAclsWithDuplicates.GroupBy(a => a.Id).Select(a => a.First()).ToList();

        _logger.LogDebug("Found {count} inherited access control list entities for tenant {tenantId}", inheritedAcls.Count, tenantId);

        return inheritedAcls!;
    }

    private IEnumerable<AccessControlListDoiEntity> GetFlattenChildrenInclSelf(AccessControlListDoiEntity acl)
    {
        yield return acl;
        foreach (var childDoi in acl.Children.SelectMany(GetFlattenChildrenInclSelf))
        {
            yield return childDoi;
        }
    }

    private IEnumerable<AccessControlListDoiEntity> GetFlattenParentsInclSelf(AccessControlListDoiEntity acl)
    {
        yield return acl;
        acl.Children.Clear();
        while (acl.Parent != null)
        {
            yield return acl.Parent;
            acl.Parent.Children.Clear();
            acl = acl.Parent;
        }
    }

    private void BuildTree(IEnumerable<AccessControlListDoiEntity> acls)
    {
        if (acls == null)
        {
            throw new ArgumentNullException(nameof(acls));
        }

        foreach (var acl in acls)
        {
            var parent = acls.FirstOrDefault(a => a.Id == acl.ParentId);
            acl.Parent = parent;
            parent?.Children.Add(acl);
        }
    }
}
