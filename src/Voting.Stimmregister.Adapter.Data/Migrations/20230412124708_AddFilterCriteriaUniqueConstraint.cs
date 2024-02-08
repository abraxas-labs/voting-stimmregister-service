using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class AddFilterCriteriaUniqueConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FilterCriteria_FilterId",
                table: "FilterCriteria");

            migrationBuilder.DropIndex(
                name: "IX_FilterCriteria_FilterVersionId",
                table: "FilterCriteria");

            migrationBuilder.CreateIndex(
                name: "IX_FilterCriteria_FilterId_ReferenceId",
                table: "FilterCriteria",
                columns: new[] { "FilterId", "ReferenceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FilterCriteria_FilterVersionId_ReferenceId",
                table: "FilterCriteria",
                columns: new[] { "FilterVersionId", "ReferenceId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FilterCriteria_FilterId_ReferenceId",
                table: "FilterCriteria");

            migrationBuilder.DropIndex(
                name: "IX_FilterCriteria_FilterVersionId_ReferenceId",
                table: "FilterCriteria");

            migrationBuilder.CreateIndex(
                name: "IX_FilterCriteria_FilterId",
                table: "FilterCriteria",
                column: "FilterId");

            migrationBuilder.CreateIndex(
                name: "IX_FilterCriteria_FilterVersionId",
                table: "FilterCriteria",
                column: "FilterVersionId");
        }
    }
}
