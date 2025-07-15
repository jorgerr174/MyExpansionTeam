using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace METDAL.Migrations
{
    /// <inheritdoc />
    public partial class BeforePlayerUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Franchises_TeamId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "TotalMoney",
                table: "ContractYears");

            migrationBuilder.DropColumn(
                name: "APY",
                table: "Contracts");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "Players",
                newName: "FranchiseId");

            migrationBuilder.RenameIndex(
                name: "IX_Players_TeamId",
                table: "Players",
                newName: "IX_Players_FranchiseId");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "ContractYears",
                newName: "Season");

            migrationBuilder.AlterColumn<int>(
                name: "Position3",
                table: "Players",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Position2",
                table: "Players",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Players",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateOnly>(
                name: "BirthDate",
                table: "Players",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "College",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Weight",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<double>(
                name: "Earned",
                table: "ContractYears",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FranchiseId",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SeasonStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Season = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    FranchiseId = table.Column<int>(type: "int", nullable: false),
                    Games = table.Column<int>(type: "int", nullable: false),
                    Fmb = table.Column<int>(type: "int", nullable: false),
                    FmbRec = table.Column<int>(type: "int", nullable: false),
                    FantPt = table.Column<int>(type: "int", nullable: false),
                    VBD = table.Column<int>(type: "int", nullable: false),
                    OvrRank = table.Column<int>(type: "int", nullable: true),
                    PosRank = table.Column<int>(type: "int", nullable: true),
                    Cmp = table.Column<int>(type: "int", nullable: true),
                    Att = table.Column<int>(type: "int", nullable: true),
                    PassYards = table.Column<int>(type: "int", nullable: true),
                    PassTD = table.Column<int>(type: "int", nullable: true),
                    Int = table.Column<int>(type: "int", nullable: true),
                    Rec = table.Column<int>(type: "int", nullable: true),
                    Tgt = table.Column<int>(type: "int", nullable: true),
                    RecYards = table.Column<int>(type: "int", nullable: true),
                    RecTD = table.Column<int>(type: "int", nullable: true),
                    Car = table.Column<int>(type: "int", nullable: true),
                    RushYards = table.Column<int>(type: "int", nullable: true),
                    RushTD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeasonStats_Franchises_FranchiseId",
                        column: x => x.FranchiseId,
                        principalTable: "Franchises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeasonStats_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_FranchiseId",
                table: "Contracts",
                column: "FranchiseId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonStats_FranchiseId",
                table: "SeasonStats",
                column: "FranchiseId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonStats_PlayerId",
                table: "SeasonStats",
                column: "PlayerId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Franchises_FranchiseId",
                table: "Contracts");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Franchises_FranchiseId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "SeasonStats");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_FranchiseId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "College",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "FranchiseId",
                table: "Contracts");

            migrationBuilder.RenameColumn(
                name: "FranchiseId",
                table: "Players",
                newName: "TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_Players_FranchiseId",
                table: "Players",
                newName: "IX_Players_TeamId");

            migrationBuilder.RenameColumn(
                name: "Season",
                table: "ContractYears",
                newName: "Year");

            migrationBuilder.AlterColumn<int>(
                name: "Position3",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Position2",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Players",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(70)",
                oldMaxLength: 70);

            migrationBuilder.AlterColumn<double>(
                name: "Earned",
                table: "ContractYears",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<double>(
                name: "TotalMoney",
                table: "ContractYears",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "APY",
                table: "Contracts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Franchises_TeamId",
                table: "Players",
                column: "TeamId",
                principalTable: "Franchises",
                principalColumn: "Id");
        }
    }
}
