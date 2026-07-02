using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSecondFactorTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SecondFactorTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    PollCount = table.Column<int>(type: "integer", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpireAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActionIdHash = table.Column<string>(type: "text", nullable: false),
                    NevisExternalTokenJwtIds = table.Column<List<string>>(type: "text[]", nullable: true),
                    Verified = table.Column<bool>(type: "boolean", nullable: false),
                    Used = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecondFactorTransactions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SecondFactorTransactions_ExpireAt",
                table: "SecondFactorTransactions",
                column: "ExpireAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SecondFactorTransactions");
        }
    }
}
