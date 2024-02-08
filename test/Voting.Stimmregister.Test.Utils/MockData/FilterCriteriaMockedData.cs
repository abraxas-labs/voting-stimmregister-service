// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Test.Utils.MockData;

/// <summary>
/// ImportAcl mock data seeder. Use this class to add some static seeding data.
/// </summary>
public static class FilterCriteriaMockedData
{
    public static int MunicipalityId => FilterMockedData.MunicipalityId;

    public static int MunicipalityIdOther => FilterMockedData.MunicipalityIdOther;

    public static FilterCriteriaEntity SomeFilterCriteria_MunicipalityId
        => new()
        {
            Id = Guid.Parse("64493335-CDA0-4268-B2FE-A6C1A3C7FFF2"),
            AuditInfo = MockedAuditInfo.Get(),
            FilterId = FilterMockedData.SomeFilter_MunicipalityId.Id,
            FilterOperator = FilterOperatorType.Contains,
            FilterType = FilterDataType.String,
            FilterValue = "Wert",
        };

    public static FilterCriteriaEntity SomeFilterCriteria_MunicipalityIdOther
        => new()
        {
            Id = Guid.Parse("EA57D4BD-EB1E-4726-8EA5-888715169A3F"),
            AuditInfo = MockedAuditInfo.Get(),
            FilterId = FilterMockedData.SomeFilter_MunicipalityIdOther.Id,
            FilterOperator = FilterOperatorType.Contains,
            FilterType = FilterDataType.String,
            FilterValue = "Wert",
        };

    public static FilterCriteriaEntity SomeFilterCriteria_MunicipalityIdOther2
        => new()
        {
            Id = Guid.Parse("3337D4BD-EB1E-4726-8EA5-888715169A3F"),
            ReferenceId = FilterReference.FirstName,
            AuditInfo = MockedAuditInfo.Get(),
            FilterId = FilterMockedData.SomeFilter_MunicipalityIdOther2.Id,
            FilterOperator = FilterOperatorType.Contains,
            FilterType = FilterDataType.String,
            FilterValue = "Natalie",
        };

    public static FilterCriteriaEntity SomeFilterCriteria_MunicipalityIdOther3
        => new()
        {
            Id = Guid.Parse("001a05cf-83f2-4eac-9bde-cf7422e26345"),
            ReferenceId = FilterReference.MunicipalityId,
            AuditInfo = MockedAuditInfo.Get(),
            FilterId = FilterMockedData.SomeFilter_MunicipalityIdOther2.Id,
            FilterOperator = FilterOperatorType.Equals,
            FilterType = FilterDataType.Numeric,
            FilterValue = FilterMockedData.SomeFilter_MunicipalityIdOther2.MunicipalityId.ToString(),
        };

    public static FilterCriteriaEntity SomeFilterCriteria_MunicipalityId9170_SwissAbroad
        => new()
        {
            Id = Guid.Parse("39eb3023-41cc-4f0d-a96e-81dc501c5528"),
            ReferenceId = FilterReference.MunicipalityId,
            AuditInfo = MockedAuditInfo.Get(),
            FilterId = FilterMockedData.SomeFilter_MunicipalityId9170_SwissAbroad.Id,
            FilterOperator = FilterOperatorType.Equals,
            FilterType = FilterDataType.Numeric,
            FilterValue = FilterMockedData.SomeFilter_MunicipalityId9170_SwissAbroad.MunicipalityId.ToString(),
        };

    public static IEnumerable<FilterCriteriaEntity> All
    {
        get
        {
            yield return SomeFilterCriteria_MunicipalityId;
            yield return SomeFilterCriteria_MunicipalityIdOther;
            yield return SomeFilterCriteria_MunicipalityIdOther2;
            yield return SomeFilterCriteria_MunicipalityIdOther3;
            yield return SomeFilterCriteria_MunicipalityId9170_SwissAbroad;
        }
    }

    /// <summary>
    /// Seeds mock data defined in this task.
    /// </summary>
    /// <param name="runScoped">The run scoped action.</param>
    /// <param name="seedWithFilterMockedData">seed process should include seeding of filters.</param>
    /// <returns>A <see cref="Task"/> from the run scoped action where data is seeded async.</returns>
    public static Task Seed(Func<Func<IServiceProvider, Task>, Task> runScoped, bool seedWithFilterMockedData = true)
    {
        return runScoped(async sp =>
        {
            if (seedWithFilterMockedData)
            {
                await FilterMockedData.Seed(runScoped);
            }

            var db = sp.GetRequiredService<IDataContext>();
            db.FilterCriteria.AddRange(All);
            await db.SaveChangesAsync();
        });
    }
}
