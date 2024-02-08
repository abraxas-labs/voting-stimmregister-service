using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class AddPersonSendVotingCardsToDomainOfInfluenceReturnAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SendVotingCardsToDomainOfInfluenceReturnAddress",
                table: "Persons",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SendVotingCardsToDomainOfInfluenceReturnAddress",
                table: "Persons");
        }
    }
}
