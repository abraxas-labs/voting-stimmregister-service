// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.Repositories;

public class FilterVersionPersonRepository : DbRepository<DataContext, FilterVersionPersonEntity>, IFilterVersionPersonRepository
{
    public FilterVersionPersonRepository(DataContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyCollection<int>> ListBfsIgnoreAcl(Guid filterVersionId)
    {
        // ignore acl, this is usually not fetched to be returned to the user, but only for internal signature checks
        return await Set
            .IgnoreQueryFilters()
            .Where(x => x.FilterVersionId == filterVersionId)
            .Select(x => x.Person!.MunicipalityId)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();
    }

    public async Task<(int Count, int InvalidCount)> CountIgnoreAcl(Guid filterVersionId)
    {
        // ignore acl, the acl is validated at the time of adding the persons to the filter version
        // this query is only performed once to store the count of the inserted persons into the filter version
        var query = Set
            .IgnoreQueryFilters()
            .Where(x => x.FilterVersionId == filterVersionId);
        var count = await query.CountAsync();
        var invalidCount = await query.CountAsync(x => !x.Person!.IsValid);
        return (count, invalidCount);
    }
}
