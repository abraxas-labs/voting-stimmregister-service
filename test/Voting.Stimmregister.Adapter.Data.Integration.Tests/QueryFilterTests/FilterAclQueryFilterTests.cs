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

public class FilterAclQueryFilterTests : BaseWriteableDbTest
{
    public FilterAclQueryFilterTests(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await ResetDb();
        await FilterMockedData.Seed(RunScoped);
    }

    [Fact]
    public async Task ShouldReturnEmptyListOfFilterWhenAclIsNotSet()
    {
        var filterEntities = await GetEntitiesByAccessControlList();

        filterEntities.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldReturnEmptyListOfFilterWhenNonExistingAclIsSet()
    {
        var filterEntities = await GetEntitiesByAccessControlList("123456789");

        filterEntities.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldReturnFilterForOneMunicipalityWhenSingleAclIsSet()
    {
        var filterEntities = await GetEntitiesByAccessControlList(FilterMockedData.MunicipalityId.ToString());

        filterEntities.MatchSnapshot(e => e.Id!);
    }

    [Fact]
    public async Task ShouldReturnFilterForMultipleMunicipalitiesWhenManyAclsAreSet()
    {
        var filterEntities = await GetEntitiesByAccessControlList(
            FilterMockedData.MunicipalityId.ToString(),
            FilterMockedData.MunicipalityIdOther.ToString());

        filterEntities.MatchSnapshot(e => e.Id!);
    }

    [Fact]
    public async Task ShouldReturnAllFilterWhenGlobalQueryFiltersAreIgnored()
    {
        using var scope = GetImpersonatedServiceScope(FilterMockedData.MunicipalityId.ToString());
        var filterRepository = scope.ServiceProvider.GetRequiredService<IFilterRepository>();
        var filterEntities = await filterRepository.Query().IgnoreQueryFilters().OrderBy(d => d.Name).ToListAsync();

        filterEntities.MatchSnapshot(e => e.Id);
    }

    [Fact]
    public async Task ShouldReturnNullWhenUsingGetByIdForUnauthorizedAcl()
    {
        using var scope = GetImpersonatedServiceScope();
        var filterRepository = scope.ServiceProvider.GetRequiredService<IFilterRepository>();

        var filterEntities = await filterRepository.Query().IgnoreQueryFilters().ToListAsync();
        var filterEntity = await filterRepository.GetByKey(filterEntities[0].Id);

        filterEntity.Should().BeNull();
    }

    private async Task<IEnumerable<FilterEntity>> GetEntitiesByAccessControlList(params string[] acls)
    {
        using var scope = GetImpersonatedServiceScope(acls);

        var filterRepository = scope.ServiceProvider.GetRequiredService<IFilterRepository>();
        var result = await filterRepository.Query().OrderBy(d => d.Id).ToListAsync();
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
