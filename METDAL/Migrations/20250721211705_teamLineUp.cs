using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace METDAL.Migrations
{
    /// <inheritdoc />
    public partial class teamLineUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "lineup",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lineup",
                table: "Teams");
        }
    }
}
