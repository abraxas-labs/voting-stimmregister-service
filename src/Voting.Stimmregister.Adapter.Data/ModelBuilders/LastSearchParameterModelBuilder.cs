// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.ModelBuilders;

public class LastSearchParameterModelBuilder : IEntityTypeConfiguration<LastSearchParameterEntity>
{
    public void Configure(EntityTypeBuilder<LastSearchParameterEntity> builder)
    {
        builder
            .HasMany(x => x.FilterCriteria)
            .WithOne(x => x.LastSearchParameter)
            .HasForeignKey(x => x.LastSearchParameterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.UserId, x.SearchType })
            .IsUnique();

        builder.OwnsOne(x => x.PageInfo);
    }
}
