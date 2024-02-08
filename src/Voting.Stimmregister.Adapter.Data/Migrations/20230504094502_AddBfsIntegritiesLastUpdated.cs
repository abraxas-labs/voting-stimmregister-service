using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class AddBfsIntegritiesLastUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "BfsIntegrities",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.Sql(@"UPDATE ""BfsIntegrities"" SET ""LastUpdated"" = COALESCE(""AuditInfo_ModifiedAt"", ""AuditInfo_CreatedAt"")");
            migrationBuilder.Sql(@"ALTER TABLE ""BfsIntegrities"" ALTER COLUMN ""LastUpdated"" SET NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "BfsIntegrities");
        }
    }
}
