// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.ModelBuilders;

/// <summary>
/// Model builder for the <see cref="DomainOfInfluenceEntity"/>.
/// </summary>
public class DomainOfInfluenceModelBuilder : IEntityTypeConfiguration<DomainOfInfluenceEntity>
{
    public void Configure(EntityTypeBuilder<DomainOfInfluenceEntity> builder)
    {
        builder
            .HasOne(d => d.ImportStatistic)
            .WithMany(x => x.DomainOfInfluences)
            .HasForeignKey(x => x.ImportStatisticId)
            .OnDelete(DeleteBehavior.SetNull);

        // Note that we need to use the same DbContext instance here that has been called with OnModelCreating,
        // so that EF Core can recognize the Language field during requests, the correct DbContext instance will
        // be used (that is, the instance that is executing the query, not the one from the DbContextAccessor)
        builder.HasQueryFilter(
            f => DbContextAccessor.DbContext.PermissionService.BfsAccessControlList.Contains(f.MunicipalityId.ToString()));
    }
}
