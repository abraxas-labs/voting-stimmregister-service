// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Voting.Lib.Database.Models;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Core.Extensions;
using Voting.Stimmregister.Domain.Models;
using Voting.Stimmregister.Test.Utils.Helpers;
using Voting.Stimmregister.Test.Utils.MockData;
using Xunit;

namespace Voting.Stimmregister.Adapter.Data.Integration.Tests.QueryFilterTests;

public class QueryFilterTests : BaseWriteableDbTest
{
    private const int AclIdWithData = 3001;

    private const string AclIdWithNoData = "Some-With-No-Data";

    private readonly Type[] _aclUnspecificEntityTypes = new[]
    {
        typeof(AccessControlListDoiEntity), // ACL itself should not be secured by query filter
        typeof(EVoterAuditEntity), // Currently out of scope
        typeof(EVoterEntity), // Currently out of scope
        typeof(ImportStatisticEntity), // ACL debatable, but implementation lets municipality is often null.
    };

    public QueryFilterTests(TestApplicationFactory factory)
        : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await ResetDb();

        await AllTablesWithMockedData.Seed(RunScoped, AclIdWithData);
    }

    [Fact]
    public void ShouldReturnEntitiesOfEachEntityTypeWhenAclIsSet()
    {
        using var scope = GetImpersonatedServiceScope(AclIdWithData.ToString());

        var configuredEntityTypes = GetConfiguredEntityTypes<DataContext>();
        var aclSpecificEntityTypes = configuredEntityTypes.Where(x => !_aclUnspecificEntityTypes.Contains(x)).ToList();

        var dataContext = scope.ServiceProvider.GetRequiredService<IDataContext>();

        foreach (var configuredEntityType in aclSpecificEntityTypes)
        {
            var dbSet = GetDbSetForClrType(dataContext, configuredEntityType);
            var accessibleEntitiesCount = dbSet.Count();
            accessibleEntitiesCount.Should().BeGreaterThan(0, $"the tenant data of entity type '{configuredEntityType.Name}' should be accessible.");
        }
    }

    [Fact]
    public void ShouldReturnEmptyListOfEachEntityTypeWhenNonExistingAclIsNotSet()
    {
        using var scope = GetImpersonatedServiceScope(AclIdWithNoData);

        var configuredEntityTypes = GetConfiguredEntityTypes<DataContext>();
        var aclSpecificEntityTypes = configuredEntityTypes.Where(x => !_aclUnspecificEntityTypes.Contains(x)).ToList();

        var dataContext = scope.ServiceProvider.GetRequiredService<IDataContext>();

        foreach (var configuredEntityType in aclSpecificEntityTypes)
        {
            var dbSet = GetDbSetForClrType(dataContext, configuredEntityType);
            var accessibleEntitiesCount = dbSet.Count();
            accessibleEntitiesCount.Should().Be(0, $"There should be a QueryFilter for entity type '{configuredEntityType.Name}' to ensure security");
        }
    }

    [Fact]
    public void ShouldReturnEmptyListOfEachEntityTypeWhenAclIsNotSet()
    {
        using var scope = GetImpersonatedServiceScope();

        var configuredEntityTypes = GetConfiguredEntityTypes<DataContext>();
        var aclSpecificEntityTypes = configuredEntityTypes.Where(x => !_aclUnspecificEntityTypes.Contains(x)).ToList();

        var dataContext = scope.ServiceProvider.GetRequiredService<IDataContext>();

        foreach (var configuredEntityType in aclSpecificEntityTypes)
        {
            var dbSet = GetDbSetForClrType(dataContext, configuredEntityType);
            var accessibleEntitiesCount = dbSet.Count();
            accessibleEntitiesCount.Should().Be(0, $"There should be a QueryFilter for entity type '{configuredEntityType.Name}' to ensure security");
        }
    }

    private static IQueryable<BaseEntity> GetDbSetForClrType(IDataContext dbContext, Type clrType)
    {
        var dbSetType = typeof(DbSet<>);

        var propertyInfo = dbContext.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => new { PropertyInfo = p, p.PropertyType })
            .First(t =>
                t.PropertyType.IsGenericType &&
                t.PropertyType.GetGenericTypeDefinition() == dbSetType &&
                t.PropertyType.GetGenericArguments()[0] == clrType)
            .PropertyInfo;
        return (propertyInfo.GetValue(dbContext) as IQueryable<BaseEntity>)!;
    }

    private static IEnumerable<Type> GetConfiguredEntityTypes<TDbContext>()
        where TDbContext : DbContext
    {
        var assembly = typeof(TDbContext).Assembly;

        foreach (var type in assembly.GetTypes().OrderBy(t => t.FullName))
        {
            // Only accept types that contain a parameterless constructor, are not abstract and satisfy a predicate if it was used.
            if (type.GetConstructor(Type.EmptyTypes) == null)
            {
                continue;
            }

            foreach (var @interface in type.GetInterfaces())
            {
                if (!@interface.IsGenericType)
                {
                    continue;
                }

                if (@interface.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                    @interface.GenericTypeArguments.Length > 0)
                {
                    // Each model binder should bind exactly on entity type
                    yield return @interface.GenericTypeArguments.Single(x => x.BaseType == typeof(BaseEntityWithSignature) || x.BaseType == typeof(BaseEntity) || x.BaseType!.GetGenericTypeDefinition() == typeof(BaseEntity<>));
                }
            }
        }
    }

    private IServiceScope GetImpersonatedServiceScope(params string[] acls)
    {
        return GetService<IServiceScopeFactory>().CreateImpersonationScope(
            new Lib.Iam.Models.User { Loginid = "1234" },
            new Lib.Iam.Models.Tenant { Id = "1234" },
            doiAcl: acls);
    }
}
