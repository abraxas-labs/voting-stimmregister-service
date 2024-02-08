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

public class FilterVersionAclQueryFilterTests : BaseWriteableDbTest
{
    public FilterVersionAclQueryFilterTests(TestApplicationFactory factory)
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
        var filterVersionEntities = await GetEntitiesByAccessControlList();

        filterVersionEntities.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldReturnEmptyListOfFilterWhenNonExistingAclIsSet()
    {
        var filterVersionEntities = await GetEntitiesByAccessControlList("123456789");

        filterVersionEntities.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldReturnFilterForOneMunicipalityWhenSingleAclIsSet()
    {
        var filterVersionEntities = await GetEntitiesByAccessControlList(FilterVersionMockedData.MunicipalityId.ToString());

        filterVersionEntities.MatchSnapshot(e => e.Id!);
    }

    [Fact]
    public async Task ShouldReturnFilterForMultipleMunicipalitiesWhenManyAclsAreSet()
    {
        var filterVersionEntities = await GetEntitiesByAccessControlList(
            FilterVersionMockedData.MunicipalityId.ToString(),
            FilterVersionMockedData.MunicipalityIdOther.ToString());

        filterVersionEntities.MatchSnapshot(e => e.Id!);
    }

    [Fact]
    public async Task ShouldReturnAllFilterWhenGlobalQueryFiltersAreIgnored()
    {
        using var scope = GetImpersonatedServiceScope(FilterVersionMockedData.MunicipalityId.ToString());
        var filterVersionRepository = scope.ServiceProvider.GetRequiredService<IFilterVersionRepository>();
        var filterVersionEntities = await filterVersionRepository.Query().IgnoreQueryFilters().OrderBy(d => d.Name).ToListAsync();

        filterVersionEntities.MatchSnapshot(e => e.Id!);
    }

    [Fact]
    public async Task ShouldReturnNullWhenUsingGetByIdForUnauthorizedAcl()
    {
        using var scope = GetImpersonatedServiceScope();
        var filterVersionRepository = scope.ServiceProvider.GetRequiredService<IFilterVersionRepository>();

        var filterVersionEntities = await filterVersionRepository.Query().IgnoreQueryFilters().ToListAsync();
        var filterVersionEntity = await filterVersionRepository.GetByKey(filterVersionEntities[0].Id);

        filterVersionEntity.Should().BeNull();
    }

    private async Task<IEnumerable<FilterVersionEntity>> GetEntitiesByAccessControlList(params string[] acls)
    {
        using var scope = GetImpersonatedServiceScope(acls);

        var filterVersionRepository = scope.ServiceProvider.GetRequiredService<IFilterVersionRepository>();
        var result = await filterVersionRepository.Query().OrderBy(d => d.FilterId).ToListAsync();
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
