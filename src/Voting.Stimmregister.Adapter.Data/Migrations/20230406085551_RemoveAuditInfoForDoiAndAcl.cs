using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class RemoveAuditInfoForDoiAndAcl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuditInfo_CreatedAt",
                table: "DomainOfInfluences");

            migrationBuilder.DropColumn(
                name: "AuditInfo_CreatedById",
                table: "DomainOfInfluences");

            migrationBuilder.DropColumn(
                name: "AuditInfo_CreatedByName",
                table: "DomainOfInfluences");

            migrationBuilder.DropColumn(
                name: "AuditInfo_ModifiedAt",
                table: "DomainOfInfluences");

            migrationBuilder.DropColumn(
                name: "AuditInfo_ModifiedById",
                table: "DomainOfInfluences");

            migrationBuilder.DropColumn(
                name: "AuditInfo_ModifiedByName",
                table: "DomainOfInfluences");

            migrationBuilder.DropColumn(
                name: "AuditInfo_CreatedAt",
                table: "AccessControlListDois");

            migrationBuilder.DropColumn(
                name: "AuditInfo_CreatedById",
                table: "AccessControlListDois");

            migrationBuilder.DropColumn(
                name: "AuditInfo_CreatedByName",
                table: "AccessControlListDois");

            migrationBuilder.DropColumn(
                name: "AuditInfo_ModifiedAt",
                table: "AccessControlListDois");

            migrationBuilder.DropColumn(
                name: "AuditInfo_ModifiedById",
                table: "AccessControlListDois");

            migrationBuilder.DropColumn(
                name: "AuditInfo_ModifiedByName",
                table: "AccessControlListDois");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AuditInfo_CreatedAt",
                table: "DomainOfInfluences",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_CreatedById",
                table: "DomainOfInfluences",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_CreatedByName",
                table: "DomainOfInfluences",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "AuditInfo_ModifiedAt",
                table: "DomainOfInfluences",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_ModifiedById",
                table: "DomainOfInfluences",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_ModifiedByName",
                table: "DomainOfInfluences",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AuditInfo_CreatedAt",
                table: "AccessControlListDois",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_CreatedById",
                table: "AccessControlListDois",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_CreatedByName",
                table: "AccessControlListDois",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "AuditInfo_ModifiedAt",
                table: "AccessControlListDois",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_ModifiedById",
                table: "AccessControlListDois",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_ModifiedByName",
                table: "AccessControlListDois",
                type: "text",
                nullable: true);
        }
    }
}
