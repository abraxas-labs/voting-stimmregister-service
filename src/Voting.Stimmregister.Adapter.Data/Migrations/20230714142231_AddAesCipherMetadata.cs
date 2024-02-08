using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class AddAesCipherMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AesCipherMetadata",
                table: "ImportStatistics");

            migrationBuilder.AddColumn<byte[]>(
                name: "AcmAesIv",
                table: "ImportStatistics",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "AcmEncryptedAesKey",
                table: "ImportStatistics",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "AcmEncryptedMacKey",
                table: "ImportStatistics",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "AcmHmac",
                table: "ImportStatistics",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcmAesIv",
                table: "ImportStatistics");

            migrationBuilder.DropColumn(
                name: "AcmEncryptedAesKey",
                table: "ImportStatistics");

            migrationBuilder.DropColumn(
                name: "AcmEncryptedMacKey",
                table: "ImportStatistics");

            migrationBuilder.DropColumn(
                name: "AcmHmac",
                table: "ImportStatistics");

            migrationBuilder.AddColumn<byte[]>(
                name: "AesCipherMetadata",
                table: "ImportStatistics",
                type: "bytea",
                maxLength: 32,
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
