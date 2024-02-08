// (c) Copyright 2023 by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class AddAuditInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "ImportStatistics",
                newName: "AuditInfo_ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "FilterVersions",
                newName: "AuditInfo_ModifiedById");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "FilterVersions",
                newName: "AuditInfo_ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "Filters",
                newName: "AuditInfo_ModifiedById");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Filters",
                newName: "AuditInfo_ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "FilterCriteria",
                newName: "AuditInfo_ModifiedById");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "FilterCriteria",
                newName: "AuditInfo_ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "EVoters",
                newName: "AuditInfo_CreatedById");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "EVoters",
                newName: "AuditInfo_ModifiedById");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "EVoterAudits",
                newName: "AuditInfo_CreatedById");

            migrationBuilder.RenameColumn(
                name: "EVoterCreatedBy",
                table: "EVoterAudits",
                newName: "EVoterAuditInfo_CreatedById");

            migrationBuilder.RenameColumn(
                name: "EVoterModifiedAt",
                table: "EVoterAudits",
                newName: "EVoterAuditInfo_ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "EVoterModifiedBy",
                table: "EVoterAudits",
                newName: "EVoterAuditInfo_ModifiedById");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "EVoterAudits",
                newName: "AuditInfo_ModifiedById");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "DomainOfInfluences",
                newName: "AuditInfo_ModifiedById");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "DomainOfInfluences",
                newName: "AuditInfo_ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "AccessControlListDois",
                newName: "AuditInfo_ModifiedById");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "AccessControlListDois",
                newName: "AuditInfo_ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Integrities",
                newName: "AuditInfo_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Integrities",
                newName: "AuditInfo_CreatedById");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "ImportStatistics",
                newName: "AuditInfo_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "ImportStatistics",
                newName: "AuditInfo_CreatedById");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "FilterVersions",
                newName: "AuditInfo_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "FilterVersions",
                newName: "AuditInfo_CreatedById");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "FilterVersionPersons",
                newName: "AuditInfo_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "FilterVersionPersons",
                newName: "AuditInfo_CreatedById");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Filters",
                newName: "AuditInfo_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Filters",
                newName: "AuditInfo_CreatedById");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "FilterCriteria",
                newName: "AuditInfo_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "FilterCriteria",
                newName: "AuditInfo_CreatedById");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "EVoters",
                newName: "AuditInfo_ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "EVoters",
                newName: "AuditInfo_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "EVoterAudits",
                newName: "AuditInfo_ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "EVoterAudits",
                newName: "AuditInfo_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "EVoterCreatedAt",
                table: "EVoterAudits",
                newName: "EVoterAuditInfo_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "DomainOfInfluences",
                newName: "AuditInfo_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "DomainOfInfluences",
                newName: "AuditInfo_CreatedById");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "AccessControlListDois",
                newName: "AuditInfo_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "AccessControlListDois",
                newName: "AuditInfo_CreatedById");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_CreatedByName",
                table: "Integrities",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_ModifiedById",
                table: "Integrities",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_ModifiedByName",
                table: "Integrities",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_CreatedByName",
                table: "ImportStatistics",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_ModifiedById",
                table: "ImportStatistics",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_ModifiedByName",
                table: "ImportStatistics",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_ModifiedByName",
                table: "FilterVersions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_CreatedByName",
                table: "FilterVersions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "AuditInfo_ModifiedAt",
                table: "FilterVersionPersons",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "AuditInfo_ModifiedAt",
                table: "Integrities",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_ModifiedByName",
                table: "FilterVersionPersons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_ModifiedById",
                table: "FilterVersionPersons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_CreatedByName",
                table: "FilterVersionPersons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_CreatedByName",
                table: "Filters",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_ModifiedByName",
                table: "Filters",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_CreatedByName",
                table: "FilterCriteria",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_ModifiedByName",
                table: "FilterCriteria",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_CreatedByName",
                table: "EVoters",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_ModifiedByName",
                table: "EVoters",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_CreatedByName",
                table: "EVoterAudits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_ModifiedByName",
                table: "EVoterAudits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EVoterAuditInfo_CreatedByName",
                table: "EVoterAudits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EVoterAuditInfo_ModifiedByName",
                table: "EVoterAudits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_CreatedByName",
                table: "DomainOfInfluences",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_ModifiedByName",
                table: "DomainOfInfluences",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_CreatedByName",
                table: "AccessControlListDois",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuditInfo_ModifiedByName",
                table: "AccessControlListDois",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE \"Integrities\" SET \"AuditInfo_CreatedByName\" = \"AuditInfo_CreatedById\", \"AuditInfo_ModifiedByName\" = \"AuditInfo_CreatedById\", \"AuditInfo_ModifiedById\" = \"AuditInfo_CreatedById\", \"AuditInfo_ModifiedAt\" = \"AuditInfo_CreatedAt\"");
            migrationBuilder.Sql("UPDATE \"ImportStatistics\" SET \"AuditInfo_CreatedByName\" = \"AuditInfo_CreatedById\", \"AuditInfo_ModifiedById\" = \"AuditInfo_CreatedById\", \"AuditInfo_ModifiedByName\" = \"AuditInfo_CreatedById\"");
            migrationBuilder.Sql("UPDATE \"FilterVersions\" SET \"AuditInfo_CreatedByName\" = \"AuditInfo_CreatedById\", \"AuditInfo_ModifiedById\" = \"AuditInfo_ModifiedById\"");
            migrationBuilder.Sql("UPDATE \"AccessControlListDois\" SET \"AuditInfo_CreatedByName\" = \"AuditInfo_CreatedById\", \"AuditInfo_ModifiedByName\" = \"AuditInfo_ModifiedById\"");
            migrationBuilder.Sql("UPDATE \"DomainOfInfluences\" SET \"AuditInfo_CreatedByName\" = \"AuditInfo_CreatedById\", \"AuditInfo_ModifiedByName\" = \"AuditInfo_ModifiedById\"");
            migrationBuilder.Sql("UPDATE \"EVoters\" SET \"AuditInfo_CreatedByName\" = \"AuditInfo_CreatedById\", \"AuditInfo_ModifiedByName\" = COALESCE(\"AuditInfo_ModifiedById\", \"AuditInfo_CreatedById\", 'unknown')");
            migrationBuilder.Sql("UPDATE \"FilterCriteria\" SET \"AuditInfo_CreatedByName\" = \"AuditInfo_CreatedById\", \"AuditInfo_ModifiedByName\" = \"AuditInfo_ModifiedById\"");
            migrationBuilder.Sql("UPDATE \"Filters\" SET \"AuditInfo_CreatedByName\" = \"AuditInfo_CreatedById\", \"AuditInfo_ModifiedByName\" = \"AuditInfo_ModifiedById\"");
            migrationBuilder.Sql("UPDATE \"FilterVersionPersons\" SET \"AuditInfo_CreatedByName\" = \"AuditInfo_CreatedById\", \"AuditInfo_ModifiedByName\" = \"AuditInfo_ModifiedById\", \"AuditInfo_ModifiedAt\" = \"AuditInfo_CreatedAt\", \"AuditInfo_ModifiedById\" = \"AuditInfo_CreatedById\"");
            migrationBuilder.Sql("UPDATE \"EVoterAudits\" SET \"AuditInfo_CreatedByName\" = \"AuditInfo_CreatedById\", \"AuditInfo_ModifiedByName\" = COALESCE(\"AuditInfo_ModifiedById\", \"AuditInfo_CreatedById\", 'unknown'), \"EVoterAuditInfo_CreatedByName\" = \"EVoterAuditInfo_CreatedById\", \"EVoterAuditInfo_ModifiedByName\" = COALESCE(\"AuditInfo_ModifiedById\", \"AuditInfo_CreatedById\", 'unknown')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotImplementedException();
        }
    }
}
