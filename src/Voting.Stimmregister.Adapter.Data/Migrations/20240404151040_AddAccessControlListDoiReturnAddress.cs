using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessControlListDoiReturnAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReturnAddress_AddressAddition",
                table: "AccessControlListDois",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnAddress_AddressLine1",
                table: "AccessControlListDois",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnAddress_AddressLine2",
                table: "AccessControlListDois",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnAddress_City",
                table: "AccessControlListDois",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnAddress_Country",
                table: "AccessControlListDois",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnAddress_Street",
                table: "AccessControlListDois",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnAddress_ZipCode",
                table: "AccessControlListDois",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnAddress_AddressAddition",
                table: "AccessControlListDois");

            migrationBuilder.DropColumn(
                name: "ReturnAddress_AddressLine1",
                table: "AccessControlListDois");

            migrationBuilder.DropColumn(
                name: "ReturnAddress_AddressLine2",
                table: "AccessControlListDois");

            migrationBuilder.DropColumn(
                name: "ReturnAddress_City",
                table: "AccessControlListDois");

            migrationBuilder.DropColumn(
                name: "ReturnAddress_Country",
                table: "AccessControlListDois");

            migrationBuilder.DropColumn(
                name: "ReturnAddress_Street",
                table: "AccessControlListDois");

            migrationBuilder.DropColumn(
                name: "ReturnAddress_ZipCode",
                table: "AccessControlListDois");
        }
    }
}
