// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voting.Stimmregister.Domain.Models;

namespace Voting.Stimmregister.Adapter.Data.ModelBuilders;

public class FilterModelBuilder : IEntityTypeConfiguration<FilterEntity>
{
    public void Configure(EntityTypeBuilder<FilterEntity> builder)
    {
        AuditedEntityModelBuilder.Configure(builder);

        builder.Ignore(x => x.LatestVersion);

        // Note that we need to use the same DbContext instance here that has been called with OnModelCreating,
        // so that EF Core can recognize the Language field during requests, the correct DbContext instance will
        // be used (that is, the instance that is executing the query, not the one from the DbContextAccessor)
        builder.HasQueryFilter(
            f => DbContextAccessor.DbContext.PermissionService.BfsAccessControlList.Contains(f.MunicipalityId.ToString()));
    }
}
