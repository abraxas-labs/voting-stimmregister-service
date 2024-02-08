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

/// <inheritdoc cref="IDomainOfInfluenceRepository"/>
public class DomainOfInfluenceRepository : DbRepository<DataContext, DomainOfInfluenceEntity>, IDomainOfInfluenceRepository
{
    public DomainOfInfluenceRepository(DataContext context)
        : base(context)
    {
    }

    public async Task<List<DomainOfInfluenceEntity>> GetDomainOfInfluencesByBfsNumber(int municipalityId)
    {
        return await Set
            .Where(x => x.MunicipalityId.Equals(municipalityId))
            .ToListAsync();
    }

    public async Task<Dictionary<int, DomainOfInfluenceEntity>> GetDomainOfInfluencesByIdForBfsNumber(int municipalityId)
    {
        return await Set
            .Where(doi => doi.MunicipalityId.Equals(municipalityId))
            .ToDictionaryAsync(p => p.DomainOfInfluenceId);
    }
}
