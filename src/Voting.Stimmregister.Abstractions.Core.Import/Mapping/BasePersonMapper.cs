// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Voting.Lib.Common;
using Voting.Stimmregister.Abstractions.Core.Import.Models;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Utils;

namespace Voting.Stimmregister.Abstractions.Core.Import.Mapping;

public abstract class BasePersonMapper<TRecord> : IPersonRecordEntityMapper<TRecord>
{
    private readonly IClock _clock;
    private readonly ImportSourceSystem _sourceSystem;
    private readonly ICountryHelperService _countryHelperService;

    protected BasePersonMapper(IClock clock, ImportSourceSystem sourceSystem, ICountryHelperService countryHelperService)
    {
        _clock = clock;
        _sourceSystem = sourceSystem;
        _countryHelperService = countryHelperService;
    }

    public void MapRecordToEntity(PersonEntity entity, PersonImportStateModel state, TRecord record)
    {
        entity.Id = Guid.NewGuid();
        entity.RegisterId = Guid.NewGuid();
        entity.IsLatest = true;
        entity.MunicipalityId = state.MunicipalityId ?? 0;
        entity.MunicipalityName = state.MunicipalityName ?? string.Empty;
        entity.SourceSystemName = _sourceSystem;
        entity.CantonBfs = state.CantonBfs ?? 0;

        MapRecordToPersonEntity(state, record, entity);
    }

    public void MapEntityLifecycleMetadata(PersonEntity newEntity, PersonEntity? existingEntity)
    {
        var currentDate = _clock.UtcNow;
        newEntity.CreatedDate = existingEntity?.CreatedDate ?? currentDate;
        newEntity.ModifiedDate = currentDate;
    }

    /// <summary>
    /// If available, the municipality address should be set as a fallback address in case of a "no correspondence" person which doesn't have a contact address or residence address.
    /// This prevents the person from being marked as invalid.
    /// </summary>
    /// <param name="entity">The person entity.</param>
    /// <param name="state">The import state.</param>
    protected void MapFallbackAddress(PersonEntity entity, PersonImportStateModel state)
    {
        if (!entity.SendVotingCardsToDomainOfInfluenceReturnAddress)
        {
            return;
        }

        if (state.MunicipalityAddress == null)
        {
            return;
        }

        if (PersonUtil.HasContactAddress(entity) || PersonUtil.HasResidenceAddress(entity))
        {
            return;
        }

        entity.ContactAddressExtensionLine1 = state.MunicipalityAddress.AddressLine1;
        entity.ContactAddressExtensionLine2 = state.MunicipalityAddress.AddressLine2;
        entity.ContactAddressTown = state.MunicipalityAddress.City;
        entity.ContactAddressCountryIdIso2 =
            _countryHelperService.GetCountryTwoLetterIsoCode(state.MunicipalityAddress.Country, null) ?? "CH";
        entity.ContactAddressZipCode = state.MunicipalityAddress.ZipCode;

        // VOTING Basis provides the street name and number only in a concatenated form. Therefore, we save the whole string in one field as well.
        entity.ContactAddressStreet = state.MunicipalityAddress.Street;
    }

    protected abstract void MapRecordToPersonEntity(PersonImportStateModel state, TRecord record, PersonEntity entity);
}
