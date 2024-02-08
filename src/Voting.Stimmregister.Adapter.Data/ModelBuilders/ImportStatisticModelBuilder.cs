// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.ModelBuilders;

/// <summary>
/// Model builder for the <see cref="ImportStatisticEntity"/>.
/// </summary>
public class ImportStatisticModelBuilder : IEntityTypeConfiguration<ImportStatisticEntity>
{
    public void Configure(EntityTypeBuilder<ImportStatisticEntity> builder)
    {
        AuditedEntityModelBuilder.Configure(builder);

        builder
            .Property(p => p.ImportStatus)
            .HasConversion(new EnumToStringConverter<ImportStatus>());

        builder
            .Property(p => p.ImportType)
            .HasConversion(new EnumToStringConverter<ImportType>());

        builder
            .Property(p => p.SourceSystem)
            .HasConversion(new EnumToStringConverter<ImportSourceSystem>());

        builder
            .Property(d => d.StartedDate)
            .HasUtcConversion();

        builder
            .Property(d => d.FinishedDate)
            .HasUtcConversion();

        // only is latest = true should be part of this index
        builder.HasIndex(x => new { x.IsLatest, x.MunicipalityId, x.SourceSystem, x.ImportType })
            .HasFilter($"\"{nameof(ImportStatisticEntity.IsLatest)}\"")
            .IsUnique();

        // Note: No QueryFilter here, because not all imports are for only one specific Municipality Id (ACL, DOI).
    }
}
