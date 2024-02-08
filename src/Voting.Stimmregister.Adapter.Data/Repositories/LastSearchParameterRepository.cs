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

public class LastSearchParameterRepository : DbRepository<DataContext, LastSearchParameterEntity>,
    ILastSearchParameterRepository
{
    public LastSearchParameterRepository(DataContext context)
        : base(context)
    {
    }

    public async Task CreateOrReplace(LastSearchParameterEntity entity)
    {
        var existing = await Fetch(entity.SearchType, entity.UserId, entity.TenantId);
        if (existing == null)
        {
            await Create(entity);
            return;
        }

        if (LastSearchParameterEntity.ExcludeIdComparer.Equals(entity, existing))
        {
            return;
        }

        await DeleteByKey(existing.Id);
        await Create(entity);
    }

    public async Task<LastSearchParameterEntity?> Fetch(PersonSearchType searchType, string userId, string tenantId)
    {
        // ignore query filters to simplify query
        // they are not needed since only the entry from the current user/tenant is selected
        return await Set
            .IgnoreQueryFilters()
            .Include(x => x.FilterCriteria.OrderBy(fc => fc.SortIndex))
            .FirstOrDefaultAsync(x => x.SearchType == searchType && x.UserId == userId && x.TenantId == tenantId);
    }
}
