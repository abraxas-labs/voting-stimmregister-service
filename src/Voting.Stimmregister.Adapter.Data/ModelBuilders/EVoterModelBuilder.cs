// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.ModelBuilders;

/// <summary>
/// Model builder for the <see cref="EVoterEntity"/>.
/// </summary>
public class EVoterModelBuilder : IEntityTypeConfiguration<EVoterEntity>
{
    public void Configure(EntityTypeBuilder<EVoterEntity> builder)
    {
        AuditedEntityModelBuilder.Configure(builder);
    }
}
