// (c) Copyright 2023 by Abraxas Informatik AG
// For license information see LICENSE file

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class AddDoiSignatureFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "SignatureHash",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignatureHash",
                table: "DomainOfInfluences");

            migrationBuilder.DropColumn(
                name: "SignatureKeyId",
                table: "DomainOfInfluences");

            migrationBuilder.DropColumn(
                name: "SignatureVersion",
                table: "DomainOfInfluences");
        }
    }
}
