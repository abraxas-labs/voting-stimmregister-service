using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class AddSourceSystemNameConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""Persons"" ADD CONSTRAINT CK_PERSON_SOURCESYSTEMNAME CHECK (""SourceSystemName"" in ('Unspecified', 'Loganto', 'Cobra', 'VotingBasis'))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""Persons"" DROP CONSTRAINT CK_PERSON_SOURCESYSTEMNAME");
        }
    }
}
