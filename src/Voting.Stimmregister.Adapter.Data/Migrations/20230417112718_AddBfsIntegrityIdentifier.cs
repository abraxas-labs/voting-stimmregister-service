using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class AddBfsIntegrityIdentifier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Integrities");

            migrationBuilder.CreateTable(
                name: "BfsIntegrities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Bfs = table.Column<string>(type: "text", nullable: false),
                    ImportType = table.Column<string>(type: "text", nullable: false),
                    AuditInfo_CreatedById = table.Column<string>(type: "text", nullable: false),
                    AuditInfo_CreatedByName = table.Column<string>(type: "text", nullable: false),
                    AuditInfo_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AuditInfo_ModifiedById = table.Column<string>(type: "text", nullable: true),
                    AuditInfo_ModifiedByName = table.Column<string>(type: "text", nullable: true),
                    AuditInfo_ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SignatureVersion = table.Column<byte>(type: "smallint", nullable: false),
                    SignatureKeyId = table.Column<string>(type: "text", nullable: false),
                    SignatureHash = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BfsIntegrities", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BfsIntegrities");

            migrationBuilder.CreateTable(
                name: "Integrities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuditInfo_CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AuditInfo_CreatedById = table.Column<string>(type: "text", nullable: false),
                    AuditInfo_CreatedByName = table.Column<string>(type: "text", nullable: false),
                    AuditInfo_ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AuditInfo_ModifiedById = table.Column<string>(type: "text", nullable: true),
                    AuditInfo_ModifiedByName = table.Column<string>(type: "text", nullable: true),
                    Bfs = table.Column<string>(type: "text", nullable: false),
                    SignatureHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    SignatureKeyId = table.Column<string>(type: "text", nullable: false),
                    SignatureVersion = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Integrities", x => x.Id);
                });
        }
    }
}
