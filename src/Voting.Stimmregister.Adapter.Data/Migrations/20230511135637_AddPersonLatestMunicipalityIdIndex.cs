using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class AddPersonLatestMunicipalityIdIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Persons_IsLatest_MunicipalityId_Id",
                table: "Persons",
                columns: new[] { "IsLatest", "MunicipalityId", "Id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Persons_IsLatest_MunicipalityId_Id",
                table: "Persons");
        }
    }
}
