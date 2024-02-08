// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Lib.Database.Models;
using Voting.Stimmregister.Abstractions.Import.Models;

namespace Voting.Stimmregister.Abstractions.Import.Mapping;

public interface IRecordEntityMapper<in TState, in TRecord, out TEntity>
    where TState : ImportStateModel<TEntity>
    where TEntity : BaseEntity
{
    TEntity MapRecordToEntity(TState importState, TRecord record);
}
