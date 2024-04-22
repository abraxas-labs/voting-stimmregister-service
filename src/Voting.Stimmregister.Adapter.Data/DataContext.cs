// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;
using Voting.Stimmregister.Abstractions.Adapter.VotingIam;
using Voting.Stimmregister.Adapter.Data.ModelBuilders;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data;

public class DataContext : DbContext, IDataContext
{
    public DataContext(
        DbContextOptions<DataContext> options,
        IPermissionService permissionService)
        : base(options)
    {
        PermissionService = permissionService;
    }

    /// <summary>
    /// Gets or sets the language used in this DbContext. Used to filter some translations/queries.
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Gets the permission service used in this DbContext. Used to filter DOI based access control list.
    /// </summary>
    public IPermissionService PermissionService { get; }

    /// <summary>
    /// Gets or sets the persons table.
    /// </summary>
    public DbSet<PersonEntity> Persons { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DomainOfInfluences table.
    /// </summary>
    public DbSet<DomainOfInfluenceEntity> DomainOfInfluences { get; set; } = null!;

    /// <summary>
    /// Gets or sets the domain of influence based access control list table.
    /// </summary>
    public DbSet<AccessControlListDoiEntity> AccessControlListDois { get; set; } = null!;

    /// <summary>
    /// Gets or sets the FilterCriteria table.
    /// </summary>
    public DbSet<FilterCriteriaEntity> FilterCriteria { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Filters table.
    /// </summary>
    public DbSet<FilterEntity> Filters { get; set; } = null!;

    /// <summary>
    /// Gets or sets the FilterVersions table.
    /// </summary>
    public DbSet<FilterVersionEntity> FilterVersions { get; set; } = null!;

    /// <summary>
    /// Gets or sets the FilterVersionPersons table.
    /// </summary>
    public DbSet<FilterVersionPersonEntity> FilterVersionPersons { get; set; } = null!;

    /// <summary>
    /// Gets or sets last search parameters of users.
    /// </summary>
    public DbSet<LastSearchParameterEntity> LastSearchParameters { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ImportStatistics table.
    /// </summary>
    public DbSet<ImportStatisticEntity> ImportStatistics { get; set; } = null!;

    /// <inheritdoc cref="IDataContext"/>
    public DbSet<BfsIntegrityEntity> BfsIntegrities { get; set; } = null!;

    /// <inheritdoc cref="IDataContext"/>
    public DbSet<EVoterEntity> EVoters { get; set; } = null!;

    /// <inheritdoc cref="IDataContext"/>
    public DbSet<EVoterAuditEntity> EVoterAudits { get; set; } = null!;

    /// <inheritdoc cref="IDataContext"/>
    public DbSet<PersonDoiEntity> PersonDois { get; set; } = null!;

    /// <inheritdoc cref="IDataContext"/>
    public DbSet<BfsStatisticEntity> BfsStatistics { get; set; } = null!;

    /// <summary>
    /// Saves changes async by calling <see cref="DbContext.SaveChangesAsync"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task SaveChangesAsync() => base.SaveChangesAsync();

    /// <inheritdoc cref="IDataContext.BeginTransaction"/>
    public Task<IDbContextTransaction> BeginTransaction()
        => Database.BeginTransactionAsync();

    /// <summary>
    /// Workaround to access the DbContext instance in the model builder classes.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        DbContextAccessor.DbContext = this;
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
    }
}
