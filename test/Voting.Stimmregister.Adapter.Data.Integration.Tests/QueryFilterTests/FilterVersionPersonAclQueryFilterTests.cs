// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Testing.Utils;
using Voting.Stimmregister.Abstractions.Adapter.Data.Repositories;
using Voting.Stimmregister.Core.Extensions;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.Adapter.Data.Integration.Tests.QueryFilterTests;

public class FilterVersionPersonAclQueryFilterTests : BaseWriteableDbTest
{
    public FilterVersionPersonAclQueryFilterTests(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await ResetDb();
        await PersonMockedData.Seed(RunScoped);
        await FilterVersionMockedData.Seed(RunScoped);
    }

    [Fact]
    public async Task ShouldReturnEmptyListOfFilterWhenAclIsNotSet()
    {
        var filterVersionPersonEntities = await GetEntitiesByAccessControlList();

        filterVersionPersonEntities.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldReturnEmptyListOfFilterWhenNonExistingAclIsSet()
    {
        var filterVersionPersonEntities = await GetEntitiesByAccessControlList("123456789");

        filterVersionPersonEntities.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldReturnFilterForOneMunicipalityWhenSingleAclIsSet()
    {
        var filterVersionPersonEntities = await GetEntitiesByAccessControlList(FilterVersionPersonMockedData.MunicipalityId.ToString());

        filterVersionPersonEntities.MatchSnapshot(e => e.Id!);
    }

    [Fact]
    public async Task ShouldReturnFilterForMultipleMunicipalitiesWhenManyAclsAreSet()
    {
        var filterVersionPersonEntities = await GetEntitiesByAccessControlList(
            FilterVersionPersonMockedData.MunicipalityId.ToString(),
            FilterVersionPersonMockedData.MunicipalityIdOther.ToString());

        filterVersionPersonEntities.MatchSnapshot(e => e.Id!);
    }

    [Fact]
    public async Task ShouldReturnAllFilterWhenGlobalQueryFiltersAreIgnored()
    {
        using var scope = GetImpersonatedServiceScope(FilterVersionPersonMockedData.MunicipalityId.ToString());
        var filterVersionPersonRepository = scope.ServiceProvider.GetRequiredService<IFilterVersionPersonRepository>();
        var filterVersionPersonEntities = await filterVersionPersonRepository.Query().IgnoreQueryFilters().OrderBy(d => d.Id).ToListAsync();

        filterVersionPersonEntities.MatchSnapshot(e => e.Id!);
    }

    [Fact]
    public async Task ShouldReturnNullWhenUsingGetByIdForUnauthorizedAcl()
    {
        using var scope = GetImpersonatedServiceScope();
        var filterVersionPersonRepository = scope.ServiceProvider.GetRequiredService<IFilterVersionPersonRepository>();

        var filterVersionPersonEntities = await filterVersionPersonRepository.Query().IgnoreQueryFilters().ToListAsync();
        var filterVersionPersonEntity = await filterVersionPersonRepository.GetByKey(filterVersionPersonEntities[0].Id);

        filterVersionPersonEntity.Should().BeNull();
    }

    private async Task<IEnumerable<FilterVersionPersonEntity>> GetEntitiesByAccessControlList(params string[] acls)
    {
        using var scope = GetImpersonatedServiceScope(acls);

        var filterVersionPersonRepository = scope.ServiceProvider.GetRequiredService<IFilterVersionPersonRepository>();
        var result = await filterVersionPersonRepository.Query().OrderBy(d => d.Id).ToListAsync();
        return result;
    }

    private IServiceScope GetImpersonatedServiceScope(params string[] acls)
    {
        return GetService<IServiceScopeFactory>().CreateImpersonationScope(
            new Lib.Iam.Models.User { Loginid = "1234" },
            new Lib.Iam.Models.Tenant { Id = "1234" },
            doiAcl: acls);
    }
}
