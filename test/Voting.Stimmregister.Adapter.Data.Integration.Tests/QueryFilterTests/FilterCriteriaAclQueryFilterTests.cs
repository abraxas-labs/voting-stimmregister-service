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

public class FilterCriteriaAclQueryFilterTests : BaseWriteableDbTest
{
    public FilterCriteriaAclQueryFilterTests(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await ResetDb();
        await PersonMockedData.Seed(RunScoped);
        await FilterCriteriaMockedData.Seed(RunScoped);
    }

    [Fact]
    public async Task ShouldReturnEmptyListOfFilterWhenAclIsNotSet()
    {
        var filterCriteriaEntities = await GetEntitiesByAccessControlList();

        filterCriteriaEntities.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldReturnEmptyListOfFilterWhenNonExistingAclIsSet()
    {
        var filterCriteriaEntities = await GetEntitiesByAccessControlList("123456789");

        filterCriteriaEntities.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldReturnFilterForOneMunicipalityWhenSingleAclIsSet()
    {
        var filterEntities = await GetEntitiesByAccessControlList(FilterCriteriaMockedData.MunicipalityId.ToString());

        filterEntities.MatchSnapshot(e => e.Id!);
    }

    [Fact]
    public async Task ShouldReturnFilterForMultipleMunicipalitiesWhenManyAclsAreSet()
    {
        var filterCriteriaEntities = await GetEntitiesByAccessControlList(
            FilterCriteriaMockedData.MunicipalityId.ToString(),
            FilterCriteriaMockedData.MunicipalityIdOther.ToString());

        filterCriteriaEntities.MatchSnapshot(e => e.Id!);
    }

    [Fact]
    public async Task ShouldReturnAllFilterWhenGlobalQueryFiltersAreIgnored()
    {
        using var scope = GetImpersonatedServiceScope(FilterCriteriaMockedData.MunicipalityId.ToString());
        var filterCriteriaRepository = scope.ServiceProvider.GetRequiredService<IFilterCriteriaRepository>();
        var filterCriteriaEntities = await filterCriteriaRepository.Query().IgnoreQueryFilters().OrderBy(d => d.Id).ToListAsync();

        filterCriteriaEntities.MatchSnapshot(e => e.Id!);
    }

    [Fact]
    public async Task ShouldReturnNullWhenUsingGetByIdForUnauthorizedAcl()
    {
        using var scope = GetImpersonatedServiceScope();
        var filterCriteriaRepository = scope.ServiceProvider.GetRequiredService<IFilterCriteriaRepository>();

        var filterCriteriaEntities = await filterCriteriaRepository.Query().IgnoreQueryFilters().ToListAsync();
        var filterCriteriaEntity = await filterCriteriaRepository.GetByKey(filterCriteriaEntities[0].Id);

        filterCriteriaEntity.Should().BeNull();
    }

    private async Task<IEnumerable<FilterCriteriaEntity>> GetEntitiesByAccessControlList(params string[] acls)
    {
        using var scope = GetImpersonatedServiceScope(acls);

        var filterCriteriaRepository = scope.ServiceProvider.GetRequiredService<IFilterCriteriaRepository>();
        var result = await filterCriteriaRepository.Query().OrderBy(d => d.Id).ToListAsync();
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
