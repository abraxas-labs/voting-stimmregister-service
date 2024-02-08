using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class AddFilterVersionInvalidPersonCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Persons_IsLatest_IsDeleted_Id",
                table: "Persons");

            migrationBuilder.AddColumn<int>(
                name: "CountOfInvalidPersons",
                table: "FilterVersions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_IsLatest_IsDeleted_IsValid_Id",
                table: "Persons",
                columns: new[] { "IsLatest", "IsDeleted", "IsValid", "Id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Persons_IsLatest_IsDeleted_IsValid_Id",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "CountOfInvalidPersons",
                table: "FilterVersions");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_IsLatest_IsDeleted_Id",
                table: "Persons",
                columns: new[] { "IsLatest", "IsDeleted", "Id" });
        }
    }
}
