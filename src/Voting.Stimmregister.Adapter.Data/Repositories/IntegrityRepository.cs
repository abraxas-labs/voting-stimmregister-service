// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Voting.Lib.Database.Repositories;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.Repositories;

/// <inheritdoc cref="IIntegrityRepository"/>
public class IntegrityRepository : DbRepository<DataContext, BfsIntegrityEntity>, IIntegrityRepository
{
    public IntegrityRepository(DataContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public async Task CreateOrUpdate(BfsIntegrityEntity integrityEntity)
    {
        if (string.IsNullOrWhiteSpace(integrityEntity.Bfs))
        {
            integrityEntity.Bfs = string.Empty;
        }

        var existingEntity = await Query()
            .Where(x => x.Bfs == integrityEntity.Bfs)
            .Where(x => x.ImportType == integrityEntity.ImportType)
            .SingleOrDefaultAsync();

        if (existingEntity != null)
        {
            existingEntity.LastUpdated = integrityEntity.LastUpdated;
            existingEntity.SignatureVersion = integrityEntity.SignatureVersion;
            existingEntity.SignatureKeyId = integrityEntity.SignatureKeyId;
            existingEntity.Signature = integrityEntity.Signature;
            existingEntity.AuditInfo.ModifiedAt = integrityEntity.AuditInfo.CreatedAt;
            existingEntity.AuditInfo.ModifiedById = integrityEntity.AuditInfo.CreatedById;
            existingEntity.AuditInfo.ModifiedByName = integrityEntity.AuditInfo.CreatedByName;
            await Update(existingEntity);
        }
        else
        {
            await Create(integrityEntity);
        }
    }
}
