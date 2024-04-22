// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.Repositories;

public class BfsStatisticRepository : DbRepository<DataContext, BfsStatisticEntity>, IBfsStatisticRepository
{
    public BfsStatisticRepository(DataContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public async Task CreateOrUpdate(BfsStatisticEntity bfsStatisticEntity)
    {
        var existingEntity = await Query()
            .SingleOrDefaultAsync(x => x.Bfs == bfsStatisticEntity.Bfs);

        if (existingEntity != null)
        {
            existingEntity.EVoterRegistrationCount = bfsStatisticEntity.EVoterRegistrationCount;
            existingEntity.EVoterDeregistrationCount = bfsStatisticEntity.EVoterDeregistrationCount;
            existingEntity.EVoterTotalCount = bfsStatisticEntity.EVoterTotalCount;
            existingEntity.VoterTotalCount = bfsStatisticEntity.VoterTotalCount;
            existingEntity.AuditInfo.ModifiedAt = bfsStatisticEntity.AuditInfo.CreatedAt;
            existingEntity.AuditInfo.ModifiedById = bfsStatisticEntity.AuditInfo.CreatedById;
            existingEntity.AuditInfo.ModifiedByName = bfsStatisticEntity.AuditInfo.CreatedByName;
            await Update(existingEntity);
        }
        else
        {
            await Create(bfsStatisticEntity);
        }
    }
}
