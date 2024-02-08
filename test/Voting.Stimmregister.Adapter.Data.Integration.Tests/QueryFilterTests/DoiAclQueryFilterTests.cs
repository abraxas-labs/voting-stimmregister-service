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

public class DoiAclQueryFilterTests : BaseWriteableDbTest
{
    public DoiAclQueryFilterTests(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await ResetDb();
        await DomainOfInfluenceMockedData.Seed(RunScoped);
    }

    [Fact]
    public async Task ShouldReturnEmptyListOfDoiWhenAclIsNotSet()
    {
        var dois = await GetEntitiesByAccessControlList();

        dois.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldReturnEmptyListOfDoiWhenNonExistingAclIsSet()
    {
        var dois = await GetEntitiesByAccessControlList("123456789");

        dois.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldReturnDoiForOneMunicipalityWhenSingleAclIsSet()
    {
        var dois = await GetEntitiesByAccessControlList(DomainOfInfluenceMockedData.MunicipalityIdStGallen.ToString());

        dois.MatchSnapshot(e => e.Id!);
    }

    [Fact]
    public async Task ShouldReturnDoiForMultipleMunicipalitiesWhenManyAclsAreSet()
    {
        var dois = await GetEntitiesByAccessControlList(
            DomainOfInfluenceMockedData.MunicipalityIdStGallen.ToString(),
            DomainOfInfluenceMockedData.MunicipalityIdGoldach.ToString());

        dois.MatchSnapshot(e => e.Id!);
    }

    [Fact]
    public async Task ShouldReturnAllDoiWhenGlobalQueryFiltersAreIgnored()
    {
        using var scope = GetImpersonatedServiceScope(DomainOfInfluenceMockedData.MunicipalityIdStGallen.ToString());
        var doiRepo = scope.ServiceProvider.GetRequiredService<IDomainOfInfluenceRepository>();
        var dois = await doiRepo.Query().IgnoreQueryFilters().OrderBy(d => d.DomainOfInfluenceId).ToListAsync();

        dois.MatchSnapshot(e => e.Id!);
    }

    [Fact]
    public async Task ShouldReturnNullWhenUsingGetByIdForUnauthorizedAcl()
    {
        using var scope = GetImpersonatedServiceScope();
        var doiRepo = scope.ServiceProvider.GetRequiredService<IDomainOfInfluenceRepository>();

        var allDois = await doiRepo.Query().IgnoreQueryFilters().ToListAsync();
        var doi = await doiRepo.GetByKey(allDois[0].Id);

        doi.Should().BeNull();
    }

    private async Task<IEnumerable<DomainOfInfluenceEntity>> GetEntitiesByAccessControlList(params string[] acls)
    {
        using var scope = GetImpersonatedServiceScope(acls);

        var doiRepo = scope.ServiceProvider.GetRequiredService<IDomainOfInfluenceRepository>();
        var result = await doiRepo.Query().OrderBy(d => d.DomainOfInfluenceId).ToListAsync();
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
