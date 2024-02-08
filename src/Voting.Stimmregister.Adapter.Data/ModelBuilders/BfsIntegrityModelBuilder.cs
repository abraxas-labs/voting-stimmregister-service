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
/// Model builder for the <see cref="BfsIntegrityEntity"/>.
/// </summary>
public class BfsIntegrityModelBuilder
    : IEntityTypeConfiguration<BfsIntegrityEntity>
{
    public void Configure(EntityTypeBuilder<BfsIntegrityEntity> builder)
    {
        AuditedEntityModelBuilder.Configure(builder);

        builder
            .Property(p => p.ImportType)
            .HasConversion(new EnumToStringConverter<ImportType>());

        // Note that we need to use the same DbContext instance here that has been called with OnModelCreating,
        // so that EF Core can recognize the Language field during requests, the correct DbContext instance will
        // be used (that is, the instance that is executing the query, not the one from the DbContextAccessor)
        builder.HasQueryFilter(
            f => DbContextAccessor.DbContext.PermissionService.BfsAccessControlList.Contains(f.Bfs));
    }
}
