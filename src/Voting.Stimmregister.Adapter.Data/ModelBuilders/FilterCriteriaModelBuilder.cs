// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.ModelBuilders;

public class FilterCriteriaModelBuilder : IEntityTypeConfiguration<FilterCriteriaEntity>
{
    public void Configure(EntityTypeBuilder<FilterCriteriaEntity> builder)
    {
        AuditedEntityModelBuilder.Configure(builder);

        builder
            .HasOne(x => x.Filter)
            .WithMany(x => x.FilterCriterias)
            .HasForeignKey(x => x.FilterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.FilterVersion)
            .WithMany(x => x.FilterCriterias)
            .HasForeignKey(x => x.FilterVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => new { x.FilterId, x.ReferenceId })
            .IsUnique();

        builder
            .HasIndex(x => new { x.FilterVersionId, x.ReferenceId })
            .IsUnique();

        builder
            .Property(d => d.ReferenceId)
            .HasConversion(new EnumToStringConverter<FilterReference>());

        builder
            .Property(x => x.FilterType)
            .HasConversion(new EnumToStringConverter<FilterDataType>());

        builder
            .Property(x => x.FilterOperator)
            .HasConversion(new EnumToStringConverter<FilterOperatorType>());

        // Note that we need to use the same DbContext instance here that has been called with OnModelCreating,
        // so that EF Core can recognize the Language field during requests, the correct DbContext instance will
        // be used (that is, the instance that is executing the query, not the one from the DbContextAccessor)
        builder.HasQueryFilter(
            f => DbContextAccessor.DbContext.PermissionService.BfsAccessControlList.Contains(f.Filter != null ? f.Filter.MunicipalityId.ToString() : f.FilterVersion!.Filter!.MunicipalityId.ToString()));
    }
}
