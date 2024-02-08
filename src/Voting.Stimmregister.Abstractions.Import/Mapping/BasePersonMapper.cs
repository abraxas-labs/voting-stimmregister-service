// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Import.Models;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Import.Mapping;

public abstract class BasePersonMapper<TRecord> : IPersonRecordEntityMapper<TRecord>
{
    private readonly IClock _clock;
    private readonly ImportSourceSystem _sourceSystem;

    protected BasePersonMapper(IClock clock, ImportSourceSystem sourceSystem)
    {
        _clock = clock;
        _sourceSystem = sourceSystem;
    }

    public PersonEntity MapRecordToEntity(PersonImportStateModel importState, TRecord record)
    {
        var creationDate = _clock.UtcNow;
        var entity = new PersonEntity
        {
            Id = Guid.NewGuid(),
            RegisterId = Guid.NewGuid(),
            CreatedDate = creationDate,
            ModifiedDate = creationDate,
            IsLatest = true,
            MunicipalityId = importState.MunicipalityId ?? 0,
            MunicipalityName = importState.MunicipalityName ?? string.Empty,
            SourceSystemName = _sourceSystem,
            CantonBfs = importState.CantonBfs ?? 0,
        };
        MapRecordToEntity(importState, record, entity);
        return entity;
    }

    protected abstract void MapRecordToEntity(PersonImportStateModel state, TRecord record, PersonEntity entity);
}
