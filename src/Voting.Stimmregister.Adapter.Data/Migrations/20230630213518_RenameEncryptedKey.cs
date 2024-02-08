using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class RenameEncryptedKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EncryptedKey",
                table: "ImportStatistics",
                newName: "AesCipherMetadata");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AesCipherMetadata",
                table: "ImportStatistics",
                newName: "EncryptedKey");
        }
    }
}
