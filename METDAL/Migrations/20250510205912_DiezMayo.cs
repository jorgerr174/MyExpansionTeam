using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace METDAL.Migrations
{
    /// <inheritdoc />
    public partial class DiezMayo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "City",
                table: "Franchises",
                newName: "Location");

            migrationBuilder.AddColumn<double>(
                name: "Y_A",
                table: "SeasonStats",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Y_T",
                table: "SeasonStats",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "Complete",
                table: "Franchises",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Incentives",
                table: "ContractYears",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Y_A",
                table: "SeasonStats");

            migrationBuilder.DropColumn(
                name: "Y_T",
                table: "SeasonStats");

            migrationBuilder.DropColumn(
                name: "Complete",
                table: "Franchises");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Franchises",
                newName: "City");

            migrationBuilder.AlterColumn<double>(
                name: "Incentives",
                table: "ContractYears",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
