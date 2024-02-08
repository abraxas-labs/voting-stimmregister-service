// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Voting.Stimmregister.Abstractions.Import.Mapping;
using Voting.Stimmregister.Abstractions.Import.Models;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Domain.Models.Import;

namespace Voting.Stimmregister.Import.Loganto.Mapping;

public class LogantoDomainOfInfluenceMapper : IDomainOfInfluenceRecordEntityMapper<LogantoDomainOfInfluenceCsvRecord>
{
    /// <summary>
    /// Maps the record properties to the entity properties.
    /// Due to performance reasons, a static mapping has been preferred instead of AutoMapper.
    /// </summary>
    /// <param name="state">The import state.</param>
    /// <param name="record">The csv record.</param>
    /// <returns>The created entity.</returns>
    public DomainOfInfluenceEntity MapRecordToEntity(
        DomainOfInfluenceImportStateModel state,
        LogantoDomainOfInfluenceCsvRecord record)
    {
        return new DomainOfInfluenceEntity
        {
            MunicipalityId = record.MunicipalityId,
            DomainOfInfluenceId = record.DomainOfInfluenceId,
            Street = record.Street,
            HouseNumber = record.HouseNumber,
            HouseNumberAddition = record.HouseNumberAddition,
            SwissZipCode = record.SwissZipCode,
            Town = record.Town,
            IsPartOfPoliticalMunicipality = record.IsPartOfPoliticalMunicipality,
            PoliticalCircleId = record.PoliticalCircleId,
            PoliticalCircleName = record.PoliticalCircleName,
            CatholicChurchCircleId = record.CatholicChurchCircleId,
            CatholicChurchCircleName = record.CatholicChurchCircleName,
            EvangelicChurchCircleId = record.EvangelicChurchCircleId,
            EvangelicChurchCircleName = record.EvangelicChurchCircleName,
            SchoolCircleId = record.SchoolCircleId,
            SchoolCircleName = record.SchoolCircleName,
            TrafficCircleId = record.TrafficCircleId,
            TrafficCircleName = record.TrafficCircleName,
            ResidentialDistrictCircleId = record.ResidentialDistrictCircleId,
            ResidentialDistrictCircleName = record.ResidentialDistrictCircleName,
            PeopleCouncilCircleId = record.PeopleCouncilCircleId,
            PeopleCouncilCircleName = record.PeopleCouncilCircleName,
            ImportStatisticId = state.ImportStatisticId,
        };
    }
}
