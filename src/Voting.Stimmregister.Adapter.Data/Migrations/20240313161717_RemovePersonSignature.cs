using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovePersonSignature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Signature",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "SignatureKeyId",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "SignatureVersion",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Signature",
                table: "DomainOfInfluences");

            migrationBuilder.DropColumn(
                name: "SignatureKeyId",
                table: "DomainOfInfluences");

            migrationBuilder.DropColumn(
                name: "SignatureVersion",
                table: "DomainOfInfluences");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Signature",
                table: "Persons",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "SignatureKeyId",
                table: "Persons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte>(
                name: "SignatureVersion",
                table: "Persons",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte[]>(
                name: "Signature",
                table: "DomainOfInfluences",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "SignatureKeyId",
                table: "DomainOfInfluences",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte>(
                name: "SignatureVersion",
                table: "DomainOfInfluences",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
