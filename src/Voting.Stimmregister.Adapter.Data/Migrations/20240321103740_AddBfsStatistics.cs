using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBfsStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BfsStatistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Bfs = table.Column<string>(type: "text", nullable: false),
                    BfsName = table.Column<string>(type: "text", nullable: false),
                    VoterTotalCount = table.Column<int>(type: "integer", nullable: false),
                    EVoterTotalCount = table.Column<int>(type: "integer", nullable: false),
                    EVoterRegistrationCount = table.Column<int>(type: "integer", nullable: false),
                    EVoterDeregistrationCount = table.Column<int>(type: "integer", nullable: false),
                    AuditInfo_CreatedById = table.Column<string>(type: "text", nullable: false),
                    AuditInfo_CreatedByName = table.Column<string>(type: "text", nullable: false),
                    AuditInfo_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AuditInfo_ModifiedById = table.Column<string>(type: "text", nullable: true),
                    AuditInfo_ModifiedByName = table.Column<string>(type: "text", nullable: true),
                    AuditInfo_ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BfsStatistics", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BfsStatistics");
        }
    }
}
