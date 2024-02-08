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
/// Model builder for the <see cref="PersonDoiEntity"/>.
/// </summary>
public class PersonDoiModelBuilder : IEntityTypeConfiguration<PersonDoiEntity>
{
    public void Configure(EntityTypeBuilder<PersonDoiEntity> builder)
    {
        builder
            .HasOne(x => x.Person)
            .WithMany(x => x.PersonDois)
            .HasForeignKey(x => x.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(x => x.DomainOfInfluenceType)
            .HasConversion(new EnumToStringConverter<DomainOfInfluenceType>());

        builder.HasQueryFilter(
            f => DbContextAccessor.DbContext.PermissionService.BfsAccessControlList.Contains(f.Person!.MunicipalityId.ToString()));
    }
}
