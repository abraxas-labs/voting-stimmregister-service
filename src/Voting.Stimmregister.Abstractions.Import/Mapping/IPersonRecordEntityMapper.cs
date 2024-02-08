// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmregister.Abstractions.Import.Models;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Import.Mapping;

public interface IPersonRecordEntityMapper<in TRecord> : IRecordEntityMapper<PersonImportStateModel, TRecord, PersonEntity>
{
}
