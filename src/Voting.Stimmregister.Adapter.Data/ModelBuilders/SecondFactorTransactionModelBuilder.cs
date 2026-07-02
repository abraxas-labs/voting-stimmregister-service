// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.ModelBuilders;

/// <summary>
/// Model builder for the <see cref="SecondFactorTransactionEntity"/>.
/// </summary>
public class SecondFactorTransactionModelBuilder : IEntityTypeConfiguration<SecondFactorTransactionEntity>
{
    public void Configure(EntityTypeBuilder<SecondFactorTransactionEntity> builder)
    {
        builder.HasIndex(x => x.ExpireAt);
    }
}
