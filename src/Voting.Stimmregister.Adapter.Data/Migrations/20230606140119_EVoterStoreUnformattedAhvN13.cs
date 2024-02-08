using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class EVoterStoreUnformattedAhvN13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SwissAbroadEvotingFlag",
                table: "Persons",
                newName: "EVoting");

            migrationBuilder.Sql(@"UPDATE ""Persons"" SET ""EVoting"" = FALSE WHERE ""EVoting"" IS NULL");
            migrationBuilder.Sql(@"ALTER TABLE ""Persons"" ALTER COLUMN ""EVoting"" SET NOT NULL");
            migrationBuilder.Sql(@"ALTER TABLE ""EVoters"" ALTER COLUMN ""Ahvn13"" TYPE bigint USING (REPLACE(""Ahvn13"", '.', '')::bigint)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new InvalidOperationException("Not implemented");
        }
    }
}
