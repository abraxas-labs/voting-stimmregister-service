// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.ModelBuilders;

/// <summary>
/// Model builder for the <see cref="PersonEntity"/>.
/// </summary>
public class PersonModelBuilder : IEntityTypeConfiguration<PersonEntity>
{
    public void Configure(EntityTypeBuilder<PersonEntity> builder)
    {
        builder
            .Property(d => d.CreatedDate)
            .HasUtcConversion();

        builder
            .Property(d => d.ModifiedDate)
            .HasUtcConversion();

        builder
            .Property(d => d.Sex)
            .HasConversion(new EnumToStringConverter<SexType>());

        builder
            .Property(d => d.Religion)
            .HasConversion(new EnumToStringConverter<ReligionType>());

        builder
            .HasOne(d => d.ImportStatistic)
            .WithMany(x => x.Persons)
            .HasForeignKey(d => d.ImportStatisticId)
            .OnDelete(DeleteBehavior.SetNull);

        // persons are often queried sorted by their official name then by their first name
        // ef core additionally sorts by id.
        builder.HasIndex(x => new { x.IsLatest, x.IsDeleted, x.OfficialName, x.FirstName, x.Id });
        builder.HasIndex(x => new { x.IsLatest, x.IsDeleted, x.IsValid, x.Id });

        // persons are queried by the importers by IsLatest and MunicipalityId
        builder.HasIndex(x => new { x.IsLatest, x.MunicipalityId, x.Id });

        // Note that we need to use the same DbContext instance here that has been called with OnModelCreating,
        // so that EF Core can recognize the Language field during requests, the correct DbContext instance will
        // be used (that is, the instance that is executing the query, not the one from the DbContextAccessor)
        builder.HasQueryFilter(
            f => DbContextAccessor.DbContext.PermissionService.BfsAccessControlList.Contains(f.MunicipalityId.ToString()));

        builder
            .Property(d => d.SourceSystemName)
            .HasConversion(new EnumToStringConverter<ImportSourceSystem>());

        builder
            .HasIndex(x => new { x.SourceSystemId, x.SourceSystemName, x.VersionCount, x.MunicipalityId })
            .IsUnique();

        // This index ensures no duplicate person with the same AVHN13 is kept for the same municipality
        // regardless of its source system. The uniqueness is only applied for the leading versions
        // (latest=true), while historical versions are ignored.
        builder
            .HasIndex(x => new { x.Vn, x.IsLatest, x.MunicipalityId })
            .HasFilter($"\"{nameof(PersonEntity.IsLatest)}\"")
            .IsUnique();
    }
}
