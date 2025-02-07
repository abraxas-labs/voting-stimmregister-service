// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Testing.Mocks;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Test.Utils.MockData;

/// <summary>
/// ImportAcl mock data seeder. Use this class to add some static seeding data.
/// </summary>
public static class ImportMockedData
{
    public const int MunicipalityId = 3203;
    public const string IdDomainOfInfluenceImportStatisticLoganto = "79425de7-a2d2-47e4-b34f-b4b0f9b5a762";
    public const string IdDomainOfInfluenceImportStatisticInnosolv = "89425de7-a2d2-47e4-b34f-b4b0f9b5a763";

    public static ImportStatisticEntity DomainOfInfluenceImportStatisticSuccessLoganto
        => new()
        {
            Id = Guid.Parse(IdDomainOfInfluenceImportStatisticLoganto),
            AuditInfo = MockedAuditInfo.Get(),
            FileName = "DomainOfInfluenceSuccessMock.csv",
            ImportType = ImportType.DomainOfInfluence,
            SourceSystem = ImportSourceSystem.Loganto,
            ImportStatus = ImportStatus.FinishedSuccessfully,
            ImportRecordsCountTotal = 10,
            DatasetsCountCreated = 7,
            DatasetsCountUpdated = 2,
            DatasetsCountDeleted = 1,
            FinishedDate = MockedClock.GetDate(),
            HasValidationErrors = false,
            TotalElapsedMilliseconds = 100,
            MunicipalityId = MunicipalityId,
            IsLatest = true,
        };

    public static ImportStatisticEntity DomainOfInfluenceImportStatisticSuccessInnosolv
        => new()
        {
            Id = Guid.Parse(IdDomainOfInfluenceImportStatisticInnosolv),
            AuditInfo = MockedAuditInfo.Get(),
            FileName = "DomainOfInfluenceSuccessMock.csv",
            ImportType = ImportType.DomainOfInfluence,
            SourceSystem = ImportSourceSystem.Innosolv,
            ImportStatus = ImportStatus.FinishedSuccessfully,
            ImportRecordsCountTotal = 10,
            DatasetsCountCreated = 7,
            DatasetsCountUpdated = 2,
            DatasetsCountDeleted = 1,
            FinishedDate = MockedClock.GetDate(),
            HasValidationErrors = false,
            TotalElapsedMilliseconds = 100,
            MunicipalityId = MunicipalityId,
            IsLatest = true,
        };

    public static IEnumerable<ImportStatisticEntity> All
    {
        get
        {
            yield return DomainOfInfluenceImportStatisticSuccessLoganto;
            yield return DomainOfInfluenceImportStatisticSuccessInnosolv;
        }
    }

    /// <summary>
    /// Seeds mock data defined in this task.
    /// </summary>
    /// <param name="runScoped">The run scoped action.</param>
    /// <returns>A <see cref="Task"/> from the run scoped action where data is seeded async.</returns>
    public static Task Seed(Func<Func<IServiceProvider, Task>, Task> runScoped)
    {
        return runScoped(async sp =>
        {
            var db = sp.GetRequiredService<IDataContext>();
            db.ImportStatistics.AddRange(All);
            await db.SaveChangesAsync();
        });
    }
}
