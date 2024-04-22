// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmregister.Abstractions.Core.Import.Models;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Core.Import.Mapping;

public interface IPersonRecordEntityMapper<in TRecord> : IRecordEntityMapper<PersonImportStateModel, TRecord, PersonEntity>;
