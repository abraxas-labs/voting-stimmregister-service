// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Threading.Tasks;

namespace Voting.Stimmregister.Test.Utils.MockData;

/// <summary>
/// Main mock data seeder to seed static mock data into the data context.
/// </summary>
public static class MockDataSeeder
{
    /// <summary>
    /// Seeds mock data defined in this task.
    /// </summary>
    /// <param name="runScoped">The run scoped action.</param>
    /// <returns>A <see cref="Task"/> from the run scoped action where data is seeded async.</returns>
    public static async Task Seed(Func<Func<IServiceProvider, Task>, Task> runScoped)
    {
        await AclDoiVotingBasisMockedData.Seed(runScoped);
        await ImportMockedData.Seed(runScoped);
        await PersonMockedData.Seed(runScoped);
        await DomainOfInfluenceMockedData.Seed(runScoped);
        await FilterVersionMockedData.Seed(runScoped);
        await FilterCriteriaMockedData.Seed(runScoped, false);
        await BfsIntegrityMockedData.Seed(runScoped);
    }
}
