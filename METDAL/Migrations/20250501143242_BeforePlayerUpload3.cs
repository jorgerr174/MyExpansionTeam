using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace METDAL.Migrations
{
    /// <inheritdoc />
    public partial class BeforePlayerUpload3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SeasonStats_PlayerId",
                table: "SeasonStats");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_PlayerId",
                table: "Contracts");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonStats_PlayerId_Season",
                table: "SeasonStats",
                columns: new[] { "PlayerId", "Season" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_PlayerId_Active",
                table: "Contracts",
                columns: new[] { "PlayerId", "Active" },
                unique: true,
                filter: "[Active] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SeasonStats_PlayerId_Season",
                table: "SeasonStats");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_PlayerId_Active",
                table: "Contracts");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonStats_PlayerId",
                table: "SeasonStats",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_PlayerId",
                table: "Contracts",
                column: "PlayerId");
        }
    }
}
