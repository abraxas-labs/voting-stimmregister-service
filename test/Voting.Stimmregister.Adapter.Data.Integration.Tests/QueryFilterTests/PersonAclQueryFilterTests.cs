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

public class PersonAclQueryFilterTests : BaseWriteableDbTest
{
    public PersonAclQueryFilterTests(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await ResetDb();
        await PersonMockedData.Seed(RunScoped);
    }

    [Fact]
    public async Task ShouldReturnEmptyListOfPersonWhenAclIsNotSet()
    {
        var persons = await GetEntitiesByAccessControlList();

        persons.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldReturnEmptyListOfPersonWhenNonExistingAclIsSet()
    {
        var persons = await GetEntitiesByAccessControlList("123456789");

        persons.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldReturnPersonForOneMunicipalityWhenSingleAclIsSet()
    {
        var persons = await GetEntitiesByAccessControlList(PersonMockedData.MunicipalityIdStGallen.ToString());

        persons.MatchSnapshot(e => e.Id!);
    }

    [Fact]
    public async Task ShouldReturnPersonForMultipleMunicipalitiesWhenManyAclsAreSet()
    {
        var persons = await GetEntitiesByAccessControlList(
            PersonMockedData.MunicipalityIdStGallen.ToString(),
            PersonMockedData.MunicipalityIdGoldach.ToString());

        persons.MatchSnapshot(e => e.Id!);
    }

    [Fact]
    public async Task ShouldReturnAllPersonWhenGlobalQueryFiltersAreIgnored()
    {
        using var scope = GetImpersonatedServiceScope(PersonMockedData.MunicipalityIdStGallen.ToString());
        var personRepo = scope.ServiceProvider.GetRequiredService<IPersonRepository>();
        var persons = await personRepo.Query().IgnoreQueryFilters().OrderBy(d => d.DomainOfInfluenceId).ThenBy(x => x.RegisterId).ToListAsync();

        persons.MatchSnapshot(e => e.Id!);
    }

    [Fact]
    public async Task ShouldReturnNullWhenUsingGetByIdForUnauthorizedAcl()
    {
        using var scope = GetImpersonatedServiceScope();
        var doiRepo = scope.ServiceProvider.GetRequiredService<IPersonRepository>();

        var allPersons = await doiRepo.Query().IgnoreQueryFilters().ToListAsync();
        var person = await doiRepo.GetByKey(allPersons[0].Id);

        person.Should().BeNull();
    }

    private async Task<IEnumerable<PersonEntity>> GetEntitiesByAccessControlList(params string[] acls)
    {
        using var scope = GetImpersonatedServiceScope(acls);

        var personRepo = scope.ServiceProvider.GetRequiredService<IPersonRepository>();
        var result = await personRepo.Query().OrderBy(d => d.DomainOfInfluenceId).ThenBy(x => x.RegisterId).ToListAsync();
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
