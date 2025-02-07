using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEVoterRegistrationCountColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EVoterDeregistrationCount",
                table: "BfsStatistics");

            migrationBuilder.DropColumn(
                name: "EVoterRegistrationCount",
                table: "BfsStatistics");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EVoterDeregistrationCount",
                table: "BfsStatistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EVoterRegistrationCount",
                table: "BfsStatistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
