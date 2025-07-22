using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace METDAL.Migrations
{
    /// <inheritdoc />
    public partial class teamLineUpsV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "lineup",
                table: "Teams",
                newName: "SPLineup");

            migrationBuilder.AddColumn<string>(
                name: "DefLineup",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OffLineup",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefLineup",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "OffLineup",
                table: "Teams");

            migrationBuilder.RenameColumn(
                name: "SPLineup",
                table: "Teams",
                newName: "lineup");
        }
    }
}
