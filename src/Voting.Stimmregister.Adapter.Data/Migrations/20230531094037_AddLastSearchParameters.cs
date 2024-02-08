using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class AddLastSearchParameters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LastSearchParameterId",
                table: "FilterCriteria",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortIndex",
                table: "FilterCriteria",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LastSearchParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    SearchType = table.Column<int>(type: "integer", nullable: false),
                    PageInfo_Page = table.Column<int>(type: "integer", nullable: false),
                    PageInfo_PageSize = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastSearchParameters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilterCriteria_LastSearchParameterId",
                table: "FilterCriteria",
                column: "LastSearchParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_LastSearchParameters_TenantId_UserId_SearchType",
                table: "LastSearchParameters",
                columns: new[] { "TenantId", "UserId", "SearchType" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FilterCriteria_LastSearchParameters_LastSearchParameterId",
                table: "FilterCriteria",
                column: "LastSearchParameterId",
                principalTable: "LastSearchParameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilterCriteria_LastSearchParameters_LastSearchParameterId",
                table: "FilterCriteria");

            migrationBuilder.DropTable(
                name: "LastSearchParameters");

            migrationBuilder.DropIndex(
                name: "IX_FilterCriteria_LastSearchParameterId",
                table: "FilterCriteria");

            migrationBuilder.DropColumn(
                name: "LastSearchParameterId",
                table: "FilterCriteria");

            migrationBuilder.DropColumn(
                name: "SortIndex",
                table: "FilterCriteria");
        }
    }
}
