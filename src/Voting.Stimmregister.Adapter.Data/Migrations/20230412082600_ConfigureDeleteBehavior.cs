using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class ConfigureDeleteBehavior : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilterCriteria_Filters_FilterId",
                table: "FilterCriteria");

            migrationBuilder.DropForeignKey(
                name: "FK_FilterCriteria_FilterVersions_FilterVersionId",
                table: "FilterCriteria");

            migrationBuilder.AddForeignKey(
                name: "FK_FilterCriteria_Filters_FilterId",
                table: "FilterCriteria",
                column: "FilterId",
                principalTable: "Filters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FilterCriteria_FilterVersions_FilterVersionId",
                table: "FilterCriteria",
                column: "FilterVersionId",
                principalTable: "FilterVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilterCriteria_Filters_FilterId",
                table: "FilterCriteria");

            migrationBuilder.DropForeignKey(
                name: "FK_FilterCriteria_FilterVersions_FilterVersionId",
                table: "FilterCriteria");

            migrationBuilder.AddForeignKey(
                name: "FK_FilterCriteria_Filters_FilterId",
                table: "FilterCriteria",
                column: "FilterId",
                principalTable: "Filters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FilterCriteria_FilterVersions_FilterVersionId",
                table: "FilterCriteria",
                column: "FilterVersionId",
                principalTable: "FilterVersions",
                principalColumn: "Id");
        }
    }
}
