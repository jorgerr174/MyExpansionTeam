using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace METDAL.Migrations
{
    /// <inheritdoc />
    public partial class postRosterViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Franchises_FranchiseId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Franchises_FranchiseId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonStats_Players_PlayerId",
                table: "SeasonStats");

            migrationBuilder.DropIndex(
                name: "IX_SeasonStats_PlayerId_Season",
                table: "SeasonStats");

            migrationBuilder.DropIndex(
                name: "IX_Players_FranchiseId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_FranchiseId",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_PlayerId_Active",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "FranchiseId",
                table: "Players");

            migrationBuilder.AddColumn<decimal>(
                name: "Cap",
                table: "Teams",
                type: "DECIMAL(3,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxPerTeam",
                table: "Teams",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProtectedPerTeam",
                table: "Teams",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProtectedPlayerIds",
                table: "Teams",
                type: "nvarchar(55)",
                maxLength: 55,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RosterSettings_ProtectedPlayersIds",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlayerId",
                table: "SeasonStats",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Protected1Id",
                table: "Franchises",
                type: "int",
                nullable: true,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Protected2Id",
                table: "Franchises",
                type: "int",
                nullable: true,
                defaultValue: 2);

            migrationBuilder.AddColumn<int>(
                name: "Protected3Id",
                table: "Franchises",
                type: "int",
                nullable: true,
                defaultValue: 3);

            migrationBuilder.CreateIndex(
                name: "IX_SeasonStats_PlayerId_Season",
                table: "SeasonStats",
                columns: new[] { "PlayerId", "Season" },
                unique: true,
                filter: "[PlayerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Franchises_Protected1Id",
                table: "Franchises",
                column: "Protected1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Franchises_Protected2Id",
                table: "Franchises",
                column: "Protected2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Franchises_Protected3Id",
                table: "Franchises",
                column: "Protected3Id");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_FranchiseId_Active",
                table: "Contracts",
                columns: new[] { "FranchiseId", "Active" });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_PlayerId",
                table: "Contracts",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Franchises_Players_Protected1Id",
                table: "Franchises",
                column: "Protected1Id",
                principalTable: "Players",
                principalColumn: "Id",
                onUpdate: ReferentialAction.NoAction,
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Franchises_Players_Protected2Id",
                table: "Franchises",
                column: "Protected2Id",
                principalTable: "Players",
                principalColumn: "Id",
                onUpdate: ReferentialAction.NoAction,
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Franchises_Players_Protected3Id",
                table: "Franchises",
                column: "Protected3Id",
                principalTable: "Players",
                principalColumn: "Id",
                onUpdate: ReferentialAction.NoAction,
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonStats_Players_PlayerId",
                table: "SeasonStats",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Franchises_Players_Protected1Id",
                table: "Franchises");

            migrationBuilder.DropForeignKey(
                name: "FK_Franchises_Players_Protected2Id",
                table: "Franchises");

            migrationBuilder.DropForeignKey(
                name: "FK_Franchises_Players_Protected3Id",
                table: "Franchises");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonStats_Players_PlayerId",
                table: "SeasonStats");

            migrationBuilder.DropIndex(
                name: "IX_SeasonStats_PlayerId_Season",
                table: "SeasonStats");

            migrationBuilder.DropIndex(
                name: "IX_Franchises_Protected1Id",
                table: "Franchises");

            migrationBuilder.DropIndex(
                name: "IX_Franchises_Protected2Id",
                table: "Franchises");

            migrationBuilder.DropIndex(
                name: "IX_Franchises_Protected3Id",
                table: "Franchises");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_FranchiseId_Active",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_PlayerId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Cap",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "MaxPerTeam",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "ProtectedPerTeam",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "ProtectedPlayerIds",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "RosterSettings_ProtectedPlayersIds",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Protected1Id",
                table: "Franchises");

            migrationBuilder.DropColumn(
                name: "Protected2Id",
                table: "Franchises");

            migrationBuilder.DropColumn(
                name: "Protected3Id",
                table: "Franchises");

            migrationBuilder.AlterColumn<int>(
                name: "PlayerId",
                table: "SeasonStats",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FranchiseId",
                table: "Players",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeasonStats_PlayerId_Season",
                table: "SeasonStats",
                columns: new[] { "PlayerId", "Season" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_FranchiseId",
                table: "Players",
                column: "FranchiseId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_FranchiseId",
                table: "Contracts",
                column: "FranchiseId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_PlayerId_Active",
                table: "Contracts",
                columns: new[] { "PlayerId", "Active" },
                unique: true,
                filter: "[Active] = 1");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Franchises_FranchiseId",
                table: "Contracts",
                column: "FranchiseId",
                principalTable: "Franchises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Franchises_FranchiseId",
                table: "Players",
                column: "FranchiseId",
                principalTable: "Franchises",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonStats_Players_PlayerId",
                table: "SeasonStats",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
