using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace METDAL.Migrations
{
    /// <inheritdoc />
    public partial class Tradev2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trade_Teams_TeamId",
                table: "Trade");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trade",
                table: "Trade");

            migrationBuilder.DropColumn(
                name: "TotalCapSent",
                table: "Trade");

            migrationBuilder.DropColumn(
                name: "TotalCapTaken",
                table: "Trade");

            migrationBuilder.RenameTable(
                name: "Trade",
                newName: "Trades");

            migrationBuilder.RenameColumn(
                name: "Draft",
                table: "Teams",
                newName: "Selections");

            migrationBuilder.RenameColumn(
                name: "PlayersTaken",
                table: "Trades",
                newName: "TeamPlayers");

            migrationBuilder.RenameColumn(
                name: "PlayersSent",
                table: "Trades",
                newName: "TeamPicks");

            migrationBuilder.RenameColumn(
                name: "PicksTaken",
                table: "Trades",
                newName: "FranchisePlayers");

            migrationBuilder.RenameColumn(
                name: "PicksSent",
                table: "Trades",
                newName: "FranchisePicks");

            migrationBuilder.RenameIndex(
                name: "IX_Trade_TeamId",
                table: "Trades",
                newName: "IX_Trades_TeamId");

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "Trades",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trades",
                table: "Trades",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trades_Teams_TeamId",
                table: "Trades",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trades_Teams_TeamId",
                table: "Trades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trades",
                table: "Trades");

            migrationBuilder.RenameTable(
                name: "Trades",
                newName: "Trade");

            migrationBuilder.RenameColumn(
                name: "Selections",
                table: "Teams",
                newName: "Draft");

            migrationBuilder.RenameColumn(
                name: "TeamPlayers",
                table: "Trade",
                newName: "PlayersTaken");

            migrationBuilder.RenameColumn(
                name: "TeamPicks",
                table: "Trade",
                newName: "PlayersSent");

            migrationBuilder.RenameColumn(
                name: "FranchisePlayers",
                table: "Trade",
                newName: "PicksTaken");

            migrationBuilder.RenameColumn(
                name: "FranchisePicks",
                table: "Trade",
                newName: "PicksSent");

            migrationBuilder.RenameIndex(
                name: "IX_Trades_TeamId",
                table: "Trade",
                newName: "IX_Trade_TeamId");

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "Trade",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "TotalCapSent",
                table: "Trade",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalCapTaken",
                table: "Trade",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trade",
                table: "Trade",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trade_Teams_TeamId",
                table: "Trade",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }
    }
}
