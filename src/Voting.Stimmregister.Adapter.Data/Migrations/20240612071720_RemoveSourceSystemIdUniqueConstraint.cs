﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSourceSystemIdUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Persons_SourceSystemId_SourceSystemName_VersionCount_Munici~",
                table: "Persons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Persons_SourceSystemId_SourceSystemName_VersionCount_Munici~",
                table: "Persons",
                columns: new[] { "SourceSystemId", "SourceSystemName", "VersionCount", "MunicipalityId" },
                unique: true);
        }
    }
}
