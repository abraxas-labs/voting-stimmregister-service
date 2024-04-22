// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Abstractions.Adapter.Data.DataContexts;

/// <summary>
/// Database data context abstraction.
/// </summary>
public interface IDataContext
{
    /// <summary>
    /// Gets or sets the language used in this DbContext. Used to filter some translations/queries.
    /// </summary>
    string? Language { get; set; }

    /// <summary>
    /// Gets or sets the Person table.
    /// </summary>
    DbSet<PersonEntity> Persons { get; set; }

    /// <summary>
    /// Gets or sets the DomainOfInfluences table.
    /// </summary>
    DbSet<DomainOfInfluenceEntity> DomainOfInfluences { get; set; }

    /// <summary>
    /// Gets or sets the domain of influence based access control list table.
    /// </summary>
    DbSet<AccessControlListDoiEntity> AccessControlListDois { get; set; }

    /// <summary>
    /// Gets or sets the FilterCriterias table.
    /// </summary>
    DbSet<FilterCriteriaEntity> FilterCriteria { get; set; }

    /// <summary>
    /// Gets or sets the Filters table.
    /// </summary>
    DbSet<FilterEntity> Filters { get; set; }

    /// <summary>
    /// Gets or sets the FilterVersions table.
    /// </summary>
    DbSet<FilterVersionEntity> FilterVersions { get; set; }

    /// <summary>
    /// Gets or sets the FilterVersionPersons table.
    /// </summary>
    DbSet<FilterVersionPersonEntity> FilterVersionPersons { get; set; }

    /// <summary>
    /// Gets or sets the ImportStatistics table.
    /// </summary>
    DbSet<ImportStatisticEntity> ImportStatistics { get; set; }

    /// <summary>
    /// Gets or sets the BfsIntegrities table.
    /// </summary>
    DbSet<BfsIntegrityEntity> BfsIntegrities { get; set; }

    /// <summary>
    /// Gets or sets the E-Voter table.
    /// </summary>
    DbSet<EVoterEntity> EVoters { get; set; }

    /// <summary>
    /// Gets or sets the E-Voter audit table.
    /// </summary>
    DbSet<EVoterAuditEntity> EVoterAudits { get; set; }

    /// <summary>
    /// Gets or sets the Person DOI table.
    /// </summary>
    DbSet<PersonDoiEntity> PersonDois { get; set; }

    /// <summary>
    /// Gets or sets the BfsStatistics table.
    /// </summary>
    DbSet<BfsStatisticEntity> BfsStatistics { get; set; }

    /// <summary>
    /// Saves changes async by calling <see cref="DbContext.SaveChangesAsync"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SaveChangesAsync();

    /// <summary>
    /// Starts a new database transaction.
    /// </summary>
    /// <returns>The created database transaction.</returns>
    Task<IDbContextTransaction> BeginTransaction();
}
