// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.Repositories;

public class EVoterRepository : DbRepository<DataContext, EVoterEntity>, IEVoterRepository
{
    public EVoterRepository(DataContext context)
        : base(context)
    {
    }

    public async Task<HashSet<long>> GetEnabledAhvN13(short cantonBfs)
    {
        var enabled = await Set
            .Where(x => x.BfsCanton == cantonBfs && x.EVoterFlag == true)
            .Select(x => x.Ahvn13)
            .ToListAsync();
        return enabled.ToHashSet();
    }
}
