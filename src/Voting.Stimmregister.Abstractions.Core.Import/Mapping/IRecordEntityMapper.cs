// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Lib.Database.Models;
using Voting.Stimmregister.Abstractions.Core.Import.Models;

namespace Voting.Stimmregister.Abstractions.Core.Import.Mapping;

public interface IRecordEntityMapper<in TState, in TRecord, in TEntity>
    where TState : ImportStateModel<TEntity>
    where TEntity : BaseEntity
{
    void MapRecordToEntity(TEntity entity, TState importState, TRecord record);

    void MapEntityLifecycleMetadata(TEntity entity, TEntity? existingEntity);
}
