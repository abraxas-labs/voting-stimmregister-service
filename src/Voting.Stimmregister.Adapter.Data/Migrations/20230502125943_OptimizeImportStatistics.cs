using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voting.Stimmregister.Adapter.Data.Migrations
{
    public partial class OptimizeImportStatistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLatest",
                table: "ImportStatistics",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(@"
                UPDATE ""ImportStatistics"" SET ""IsLatest"" = true where ""Id"" IN(
                SELECT DISTINCT ON (""MunicipalityId"", ""ImportType"", ""SourceSystem"")
                ""Id""
                FROM ""ImportStatistics""
                ORDER BY ""MunicipalityId"", ""ImportType"", ""SourceSystem"", ""AuditInfo_CreatedAt"" DESC)");

            migrationBuilder.CreateIndex(
                name: "IX_ImportStatistics_IsLatest_MunicipalityId_SourceSystem_Impor~",
                table: "ImportStatistics",
                columns: new[] { "IsLatest", "MunicipalityId", "SourceSystem", "ImportType" },
                unique: true,
                filter: "\"IsLatest\"");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ImportStatistics_IsLatest_MunicipalityId_SourceSystem_Impor~",
                table: "ImportStatistics");

            migrationBuilder.DropColumn(
                name: "IsLatest",
                table: "ImportStatistics");
        }
    }
}
