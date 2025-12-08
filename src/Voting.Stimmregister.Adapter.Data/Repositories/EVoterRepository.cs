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

    public async Task<Dictionary<long, string?>> GetEnabledAhvN13WithEmail(short cantonBfs)
    {
        return await Set
            .Where(x => x.BfsCanton == cantonBfs && x.EVoterFlag == true)
            .ToDictionaryAsync(x => x.Ahvn13, x => x.EVotingEmail);
    }
}
