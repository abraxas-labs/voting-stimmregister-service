// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Voting.Lib.Testing.Mocks;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Core.Configuration;
using Voting.Stimmregister.Core.Services;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.Core.Unit.Tests.Services;

public class DatabaseCleanupServiceTest : BaseWriteableDbTest
{
    private readonly MockedClock _clockMock = new();
    private readonly Mock<ILogger<DatabaseCleanupService>> _loggerMock = new();

    public DatabaseCleanupServiceTest(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await PersonMockedData.Seed(RunScoped);
        await FilterMockedData.Seed(RunScoped);
        await FilterVersionMockedData.Seed(RunScoped, seedWithFilterMockedData: false);
        await FilterCriteriaMockedData.Seed(RunScoped, seedWithFilterMockedData: false);
    }

    [Fact]
    public async Task CleanUp_WhenMinimumLifetime2Days_ShouldDeleteData()
    {
        var config = new CleanupConfig
        {
            FilterVersionMinimumLifetimeInDays = 2,
            PersonVersionMinimumLifetimeInDays = 2,
        };
        var filterVersionRepository = GetService<IFilterVersionRepository>();
        var personRepository = GetService<IPersonRepository>();
        var cleanupService = new DatabaseCleanupService(config, filterVersionRepository, personRepository, _clockMock, _loggerMock.Object);

        var result = await cleanupService.RunCleanup();

        var filterVersionsAfterCleanup = await RunOnDb(
            db => db.FilterVersions
                .IgnoreQueryFilters()
                .ToListAsync());

        var filterCriteriasAfterCleanup = await RunOnDb(
            db => db.FilterCriteria
                .IgnoreQueryFilters()
                .ToListAsync());

        var personsAfterCleanup = await RunOnDb(
            db => db.Persons
                .IgnoreQueryFilters()
                .ToListAsync());

        var filterVersionPersonsAfterCleanup = await RunOnDb(
            db => db.FilterVersionPersons
                .IgnoreQueryFilters()
                .ToListAsync());

        var personDoisAfterCleanup = await RunOnDb(
            db => db.PersonDois
                .IgnoreQueryFilters()
                .ToListAsync());

        result.FilterVersionsDeleted.Should().Be(2);
        result.PersonVersionsDeleted.Should().Be(1);
        filterVersionsAfterCleanup.MatchSnapshot("FilterVersion");
        filterCriteriasAfterCleanup.MatchSnapshot("FilterCriteria");
        personsAfterCleanup.MatchSnapshot("Person");
        filterVersionPersonsAfterCleanup.MatchSnapshot("FilterVersionPerson");
        personDoisAfterCleanup.MatchSnapshot("PersonDois");
    }

    [Fact]
    public async Task CleanUp_WhenMinimumLifetime10Day_ShouldNotDeleteAnyData()
    {
        var config = new CleanupConfig
        {
            FilterVersionMinimumLifetimeInDays = 10,
            PersonVersionMinimumLifetimeInDays = 10,
        };
        var filterVersionRepository = GetService<IFilterVersionRepository>();
        var personRepository = GetService<IPersonRepository>();
        var cleanupService = new DatabaseCleanupService(config, filterVersionRepository, personRepository, _clockMock, _loggerMock.Object);

        var result = await cleanupService.RunCleanup();

        var filterVersionsAfterCleanup = await RunOnDb(
            db => db.FilterVersions
                .IgnoreQueryFilters()
                .ToListAsync());

        var filterCriteriasAfterCleanup = await RunOnDb(
            db => db.FilterCriteria
                .IgnoreQueryFilters()
                .ToListAsync());

        var personsAfterCleanup = await RunOnDb(
            db => db.Persons
                .IgnoreQueryFilters()
                .ToListAsync());

        var filterVersionPersonsAfterCleanup = await RunOnDb(
            db => db.FilterVersionPersons
                .IgnoreQueryFilters()
                .ToListAsync());

        var personDoisAfterCleanup = await RunOnDb(
            db => db.PersonDois
                .IgnoreQueryFilters()
                .ToListAsync());

        result.FilterVersionsDeleted.Should().Be(0);
        result.PersonVersionsDeleted.Should().Be(0);
        filterVersionsAfterCleanup.MatchSnapshot("FilterVersion");
        filterCriteriasAfterCleanup.MatchSnapshot("FilterCriteria");
        personsAfterCleanup.MatchSnapshot("Person");
        filterVersionPersonsAfterCleanup.MatchSnapshot("FilterVersionPerson");
        personDoisAfterCleanup.MatchSnapshot("PersonDois");
    }
}
