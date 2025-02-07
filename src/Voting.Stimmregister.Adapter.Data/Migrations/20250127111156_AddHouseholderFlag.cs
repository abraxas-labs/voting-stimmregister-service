using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHouseholderFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHouseholder",
                table: "Persons",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ResidenceApartmentId",
                table: "Persons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResidenceBuildingId",
                table: "Persons",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHouseholder",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "ResidenceApartmentId",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "ResidenceBuildingId",
                table: "Persons");
        }
    }
}
