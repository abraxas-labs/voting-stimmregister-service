using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAhvn13UniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Persons_Vn_IsLatest_MunicipalityId",
                table: "Persons",
                columns: new[] { "Vn", "IsLatest", "MunicipalityId" },
                unique: true,
                filter: "\"IsLatest\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Persons_Vn_IsLatest_MunicipalityId",
                table: "Persons");
        }
    }
}
