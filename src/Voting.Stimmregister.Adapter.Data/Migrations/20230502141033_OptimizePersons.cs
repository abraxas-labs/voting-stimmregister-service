using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class OptimizePersons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Persons",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(@"UPDATE ""Persons"" SET ""IsDeleted"" = true WHERE ""DeletedDate"" is not null");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_IsLatest_IsDeleted_Id",
                table: "Persons",
                columns: new[] { "IsLatest", "IsDeleted", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Persons_IsLatest_IsDeleted_OfficialName_FirstName_Id",
                table: "Persons",
                columns: new[] { "IsLatest", "IsDeleted", "OfficialName", "FirstName", "Id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Persons_IsLatest_IsDeleted_Id",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Persons_IsLatest_IsDeleted_OfficialName_FirstName_Id",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Persons");
        }
    }
}
