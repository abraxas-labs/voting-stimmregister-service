using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class AddInnosolvSourceSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""ImportStatistics"" DROP CONSTRAINT CK_IMPSTATS_SOURCESYSTEM");
            migrationBuilder.Sql(@"ALTER TABLE ""ImportStatistics"" ADD CONSTRAINT CK_IMPSTATS_SOURCESYSTEM CHECK (""SourceSystem"" in ('Loganto', 'Cobra', 'VotingBasis', 'Innosolv'))");

            migrationBuilder.Sql(@"ALTER TABLE ""Persons"" DROP CONSTRAINT CK_PERSON_SOURCESYSTEMNAME");
            migrationBuilder.Sql(@"ALTER TABLE ""Persons"" ADD CONSTRAINT CK_PERSON_SOURCESYSTEMNAME CHECK (""SourceSystemName"" in ('Unspecified', 'Loganto', 'Cobra', 'VotingBasis', 'Innosolv'))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""ImportStatistics"" DROP CONSTRAINT CK_IMPSTATS_SOURCESYSTEM");
            migrationBuilder.Sql(@"ALTER TABLE ""ImportStatistics"" ADD CONSTRAINT CK_IMPSTATS_SOURCESYSTEM CHECK (""SourceSystem"" in ('Loganto', 'Cobra', 'VotingBasis'))");

            migrationBuilder.Sql(@"ALTER TABLE ""Persons"" DROP CONSTRAINT CK_PERSON_SOURCESYSTEMNAME");
            migrationBuilder.Sql(@"ALTER TABLE ""Persons"" ADD CONSTRAINT CK_PERSON_SOURCESYSTEMNAME CHECK (""SourceSystemName"" in ('Unspecified', 'Loganto', 'Cobra', 'VotingBasis'))");
        }
    }
}
