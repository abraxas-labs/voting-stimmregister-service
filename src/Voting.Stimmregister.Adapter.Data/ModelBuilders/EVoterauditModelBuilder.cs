// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.ModelBuilders;

/// <summary>
/// Model builder for the <see cref="EVoterAuditEntity"/>.
/// </summary>
public class EVoterauditModelBuilder : IEntityTypeConfiguration<EVoterAuditEntity>
{
    public void Configure(EntityTypeBuilder<EVoterAuditEntity> builder)
    {
        AuditedEntityModelBuilder.Configure(builder);
        AuditedEntityModelBuilder.Configure(builder.OwnsOne(x => x.EVoterAuditInfo));
    }
}
