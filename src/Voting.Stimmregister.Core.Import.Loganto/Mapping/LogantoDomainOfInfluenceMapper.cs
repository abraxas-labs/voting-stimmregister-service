// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmregister.Abstractions.Core.Import.Mapping;
using Voting.Stimmregister.Abstractions.Core.Import.Models;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Import;

namespace Voting.Stimmregister.Core.Import.Loganto.Mapping;

public class LogantoDomainOfInfluenceMapper : IDomainOfInfluenceRecordEntityMapper<LogantoDomainOfInfluenceCsvRecord>
{
    /// <summary>
    /// Maps the record properties to the entity properties.
    /// Due to performance reasons, a static mapping has been preferred instead of AutoMapper.
    /// </summary>
    /// <param name="entity">The entity to apply mappigns from the record.</param>
    /// <param name="importState">The import state.</param>
    /// <param name="record">The csv record.</param>
    public void MapRecordToEntity(
        DomainOfInfluenceEntity entity,
        DomainOfInfluenceImportStateModel importState,
        LogantoDomainOfInfluenceCsvRecord record)
    {
        entity.MunicipalityId = record.MunicipalityId;
        entity.DomainOfInfluenceId = record.DomainOfInfluenceId;
        entity.Street = record.Street;
        entity.HouseNumber = record.HouseNumber;
        entity.HouseNumberAddition = record.HouseNumberAddition;
        entity.SwissZipCode = record.SwissZipCode;
        entity.Town = record.Town;
        entity.IsPartOfPoliticalMunicipality = record.IsPartOfPoliticalMunicipality;
        entity.PoliticalCircleId = record.PoliticalCircleId;
        entity.PoliticalCircleName = record.PoliticalCircleName;
        entity.CatholicChurchCircleId = record.CatholicChurchCircleId;
        entity.CatholicChurchCircleName = record.CatholicChurchCircleName;
        entity.EvangelicChurchCircleId = record.EvangelicChurchCircleId;
        entity.EvangelicChurchCircleName = record.EvangelicChurchCircleName;
        entity.SchoolCircleId = record.SchoolCircleId;
        entity.SchoolCircleName = record.SchoolCircleName;
        entity.TrafficCircleId = record.TrafficCircleId;
        entity.TrafficCircleName = record.TrafficCircleName;
        entity.ResidentialDistrictCircleId = record.ResidentialDistrictCircleId;
        entity.ResidentialDistrictCircleName = record.ResidentialDistrictCircleName;
        entity.PeopleCouncilCircleId = record.PeopleCouncilCircleId;
        entity.PeopleCouncilCircleName = record.PeopleCouncilCircleName;
        entity.ImportStatisticId = importState.ImportStatisticId;
    }

    public void MapEntityLifecycleMetadata(DomainOfInfluenceEntity newEntity, DomainOfInfluenceEntity? existingEntity)
    {
        // The domain of influence entities are not versioned an thus do not apply a lifecycle management similar to person entities.
    }
}
