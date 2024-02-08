﻿// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Voting.Stimmregister.Domain.Enums;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.ModelBuilders;

/// <summary>
/// Model builder for the <see cref="AccessControlListDoiEntity"/>.
/// </summary>
public class AccessControlListDoiModelBuilder : IEntityTypeConfiguration<AccessControlListDoiEntity>
{
    /// <summary>
    /// Configures the access control list entity which is structured as a self-referencing hierarchical tree
    /// with parent-child relations.
    /// </summary>
    /// <param name="builder">The entity model builder.</param>
    public void Configure(EntityTypeBuilder<AccessControlListDoiEntity> builder)
    {
        builder
            .HasOne(d => d.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(d => d.ImportStatistic)
            .WithMany(x => x.AccessControlListDois)
            .HasForeignKey(x => x.ImportStatisticId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .Property(d => d.Canton)
            .HasConversion(new EnumToStringConverter<Canton>());

        builder
            .Property(d => d.Type)
            .HasConversion(new EnumToStringConverter<DomainOfInfluenceType>());

        // Note: No QueryFilter here, because AccessControlListDoiEntity is the base to apply QueryFilter's to other entities.
    }
}
