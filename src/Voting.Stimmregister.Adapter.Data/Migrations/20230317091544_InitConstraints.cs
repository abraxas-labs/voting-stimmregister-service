// (c) Copyright 2023 by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class InitConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            PersonRegisterIsLatestIndexUp(migrationBuilder);
            ImportStatisticsConstraintsUp(migrationBuilder);
            FilterTypeAndOperationAsColumnsUp(migrationBuilder);
            AccessControlListDoiConstraintsUp(migrationBuilder);
            PersonConstraintsUp(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            PersonConstraintsDown(migrationBuilder);
            AccessControlListDoiConstraintsDown(migrationBuilder);
            FilterTypeAndOperationAsColumnsDown(migrationBuilder);
            ImportStatisticsConstraintsDown(migrationBuilder);
            PersonRegisterIsLatestIndexDown(migrationBuilder);
        }

        private void PersonRegisterIsLatestIndexUp(MigrationBuilder migrationBuilder)
        {
            const string sql = "CREATE UNIQUE INDEX persons_register_id_is_latest_true " +
                               @"ON ""Persons""(""RegisterId"",""IsLatest"") " +
                               @"WHERE ""IsLatest"" = TRUE;";
            migrationBuilder.Sql(sql);
        }

        private void ImportStatisticsConstraintsUp(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""ImportStatistics"" ALTER COLUMN ""ImportType"" SET NOT NULL");
            migrationBuilder.Sql(@"ALTER TABLE ""ImportStatistics"" ALTER COLUMN ""ImportStatus"" SET NOT NULL");
            migrationBuilder.Sql(@"ALTER TABLE ""ImportStatistics"" ADD CONSTRAINT CK_IMPSTATS_IMPORTTYPE CHECK (""ImportType"" in ('Person', 'DomainOfInfluence', 'Acl'))");
            migrationBuilder.Sql(@"ALTER TABLE ""ImportStatistics"" ADD CONSTRAINT CK_IMPSTATS_SOURCESYSTEM CHECK (""SourceSystem"" in ('Loganto', 'Cobra', 'VotingBasis'))");
            migrationBuilder.Sql(@"ALTER TABLE ""ImportStatistics"" ADD CONSTRAINT CK_IMPSTATS_IMPORTSTATUS CHECK (""ImportStatus"" in ('Queued', 'Running', 'Aborted', 'FinishedWithErrors', 'FinishedSuccessfully', 'Stale', 'Failed'))");
        }

        private void FilterTypeAndOperationAsColumnsUp(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""FilterCriteria"" ADD CONSTRAINT CK_FC_FILTEROPERATOR CHECK (""FilterOperator"" in ('Contains', 'Equals', 'StartsWith', 'EndsWith', 'Less', 'LessEqual', 'Greater', 'GreaterEqual'))");
            migrationBuilder.Sql(@"ALTER TABLE ""FilterCriteria"" ADD CONSTRAINT CK_FC_FILTERTYPE CHECK (""FilterType"" in ('String', 'Date', 'Boolean', 'Numeric', 'Select', 'Multiselect'))");
        }

        private void AccessControlListDoiConstraintsUp(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""AccessControlListDois"" ADD CONSTRAINT CK_ACLDOI_CANTON CHECK(""Canton"" in('Unknown', 'SG', 'TG', 'ZH', 'BE', 'LU', 'UR', 'SZ', 'OW', 'NW', 'GL', 'ZG', 'FR', 'SO', 'BS', 'BL', 'SH', 'AR', 'AI', 'GR', 'AG', 'TI', 'VD', 'VS', 'NE', 'GE', 'JU', 'FL'));");
            migrationBuilder.Sql(@"ALTER TABLE ""AccessControlListDois"" ADD CONSTRAINT CK_ADLDOI_TYPE CHECK(""Type"" in('Unspecified', 'Ch', 'Ct', 'Bz', 'Mu', 'Sk', 'Sc', 'Ki', 'Og', 'Ko', 'An', 'KiKat', 'KiEva', 'AnVek', 'AnWok', 'AnVok'))");
        }

        private void PersonConstraintsUp(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE ""Persons"" SET ""Sex"" = 'Undefined' WHERE ""Sex"" IS NULL");
            migrationBuilder.Sql(@"UPDATE ""Persons"" SET ""Sex"" = 'Male' WHERE ""Sex"" = '1'");
            migrationBuilder.Sql(@"UPDATE ""Persons"" SET ""Sex"" = 'Female' WHERE ""Sex"" = '2'");
            migrationBuilder.Sql(@"UPDATE ""Persons"" SET ""Sex"" = 'Undefined' WHERE ""Sex"" NOT IN('1', '2')");

            migrationBuilder.Sql(@"UPDATE ""Persons"" SET ""Religion"" = 'Evangelic' WHERE ""Religion"" = '111'");
            migrationBuilder.Sql(@"UPDATE ""Persons"" SET ""Religion"" = 'Catholic' WHERE ""Religion"" = '121'");
            migrationBuilder.Sql(@"UPDATE ""Persons"" SET ""Religion"" = 'ChristCatholic' WHERE ""Religion"" = '122'");
            migrationBuilder.Sql(@"UPDATE ""Persons"" SET ""Religion"" = 'Unknown' WHERE ""Religion"" NOT IN('Evangelic', 'Catholic', 'ChristCatholic')");

            migrationBuilder.Sql(@"ALTER TABLE ""Persons"" ADD CONSTRAINT CK_PERSON_RELIGION CHECK(""Religion"" in('Unknown', 'Evangelic', 'Catholic', 'ChristCatholic'));");
            migrationBuilder.Sql(@"ALTER TABLE ""Persons"" ADD CONSTRAINT CK_PERSON_SEX CHECK(""Sex"" in('Male', 'Female', 'Undefined'));");
        }

        private void PersonRegisterIsLatestIndexDown(MigrationBuilder migrationBuilder)
        {
            const string sql = "DROP INDEX persons_register_id_is_latest_true;";
            migrationBuilder.Sql(sql);
        }

        private void ImportStatisticsConstraintsDown(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ImportStatus",
                columns: new[] { "Id", "Description", "Name", "PhoneticId" },
                values: new object[,]
                {
                    { new Guid("092447e5-3602-4fe8-9cb5-cf449ce32abc"), "Status when an import finished successfully without errors.", "Finished sucessfully", "FinishedSuccess" },
                    { new Guid("0b1fbd0d-9700-47e7-96da-4424504d4dda"), "Status when an import has been dequeued and is processing.", "Running", "Running" },
                    { new Guid("0d5fdf1f-3ffa-4abf-b799-355087faa90a"), "Status when an import is not in any processing queue anymore.", "Stale", "Stale" },
                    { new Guid("430eec3d-dcc7-4b33-a395-c5f796dcf662"), "Status when an import failed unexpectedly.", "Failed", "Failed" },
                    { new Guid("433edd25-9dec-4791-bfb7-34967f710ec3"), "Initial status when import has been added to queue.", "Queued", "Queued" },
                    { new Guid("4b0c3340-225c-4d25-9d90-d64d2bbaf458"), "Status when an import has been finished with validation errors or other.", "Finished with errors", "FinishedWithErrors" },
                    { new Guid("d0eda14d-ead1-4a3d-8894-f99930a66182"), "Status when an import has been aborted, i.e. when the application cancellation token has bin signaled.", "Aborted", "Aborted" }
                });

            migrationBuilder.InsertData(
                table: "ImportTypes",
                columns: new[] { "Id", "Description", "Name", "PhoneticId" },
                values: new object[,]
                {
                    { new Guid("1bef28a6-0c0c-4174-a4fb-52b677bbf78b"), "Access control list (ACL) import from source system VOTING Basis", "ACL VOTING Basis", "AclVotingBasis" },
                    { new Guid("84bbf4ca-9ab7-4dc1-ab2d-5bd32c967cbc"), "Person import from source system Cobra", "Person Cobra", "PersonCobra" },
                    { new Guid("acc61d86-e29c-4024-86e3-b0b2c6ae8bad"), "Person import from source system Loganto", "Person Loganto", "PersonLoganto" },
                    { new Guid("dc9eb6fb-f798-4ee7-8822-b9e0dc578060"), "Domain of influence import from source system Loganto", "Domain of influence Loganto", "DomainOfInfluenceLoganto" }
                });
        }

        private void FilterTypeAndOperationAsColumnsDown(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FilterOperators",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Contains" },
                    { 2, "Equals" },
                    { 3, "StartsWith" },
                    { 4, "EndsWith" },
                    { 5, "Less" },
                    { 6, "LessEqual" },
                    { 7, "Greater" },
                    { 8, "GreaterEqual" }
                });

            migrationBuilder.InsertData(
                table: "FilterTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "String" },
                    { 2, "Date" },
                    { 3, "Boolean" },
                    { 4, "Numeric" },
                    { 5, "Select" },
                    { 6, "Multiselect" }
                });
        }

        private void AccessControlListDoiConstraintsDown(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""AccessControlListDois"" DROP CONSTRAINT CK_ACLDOI_CANTON");
            migrationBuilder.Sql(@"ALTER TABLE ""AccessControlListDois"" DROP CONSTRAINT CK_ADLDOI_TYPE");
        }

        private void PersonConstraintsDown(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""Persons"" DROP CONSTRAINT CK_PERSON_RELIGION");
            migrationBuilder.Sql(@"ALTER TABLE ""Persons"" DROP CONSTRAINT CK_PERSON_SEX");
        }
    }
}
