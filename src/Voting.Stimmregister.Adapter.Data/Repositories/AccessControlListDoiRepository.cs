// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.Repositories;

/// <inheritdoc cref="IAccessControlListDoiRepository"/>
public class AccessControlListDoiRepository : DbRepository<DataContext, AccessControlListDoiEntity>, IAccessControlListDoiRepository
{
    public AccessControlListDoiRepository(DataContext context)
        : base(context)
    {
    }

    public async Task<Canton> GetCantonByBfsNumber(int municipalityId)
    {
        return await Set
            .Where(acl => acl.Bfs == municipalityId.ToString() && acl.Type == DomainOfInfluenceType.Mu)
            .Select(acl => acl.Canton)
            .Distinct()
            .SingleOrDefaultAsync();
    }

    public async Task<string?> GetCantonBfsByCanton(Canton canton)
    {
        return await Set
            .Where(acl => acl.Type == DomainOfInfluenceType.Ct && acl.Canton == canton)
            .Select(x => x.Bfs)
            .Where(x => x != null)
            .FirstOrDefaultAsync();
    }
}
