using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class RenameSignatureHashField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SignatureHash",
                table: "Persons",
                newName: "Signature");

            migrationBuilder.RenameColumn(
                name: "SignatureHash",
                table: "FilterVersions",
                newName: "Signature");

            migrationBuilder.RenameColumn(
                name: "SignatureHash",
                table: "DomainOfInfluences",
                newName: "Signature");

            migrationBuilder.RenameColumn(
                name: "SignatureHash",
                table: "BfsIntegrities",
                newName: "Signature");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Signature",
                table: "Persons",
                newName: "SignatureHash");

            migrationBuilder.RenameColumn(
                name: "Signature",
                table: "FilterVersions",
                newName: "SignatureHash");

            migrationBuilder.RenameColumn(
                name: "Signature",
                table: "DomainOfInfluences",
                newName: "SignatureHash");

            migrationBuilder.RenameColumn(
                name: "Signature",
                table: "BfsIntegrities",
                newName: "SignatureHash");
        }
    }
}
