using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace METDAL.Migrations
{
    /// <inheritdoc />
    public partial class NoNewFranchises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "NoNewFranchises",
                table: "Franchises",
                sql: "Id < 33");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "NoNewFranchises",
                table: "Franchises");
        }
    }
}
