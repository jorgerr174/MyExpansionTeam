using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace METDAL.Migrations
{
    /// <inheritdoc />
    public partial class beforeImportStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonStats_Franchises_FranchiseId",
                table: "SeasonStats");

            migrationBuilder.DropIndex(
                name: "IX_SeasonStats_FranchiseId",
                table: "SeasonStats");

            migrationBuilder.DropColumn(
                name: "Att",
                table: "SeasonStats");

            migrationBuilder.DropColumn(
                name: "Car",
                table: "SeasonStats");

            migrationBuilder.DropColumn(
                name: "Cmp",
                table: "SeasonStats");

            migrationBuilder.DropColumn(
                name: "FantPt",
                table: "SeasonStats");

            migrationBuilder.DropColumn(
                name: "Fmb",
                table: "SeasonStats");

            migrationBuilder.DropColumn(
                name: "FmbRec",
                table: "SeasonStats");

            migrationBuilder.DropColumn(
                name: "FranchiseId",
                table: "SeasonStats");

            migrationBuilder.DropColumn(
                name: "Games",
                table: "SeasonStats");

            migrationBuilder.DropColumn(
                name: "Int",
                table: "SeasonStats");

            migrationBuilder.DropColumn(
                name: "VBD",
                table: "SeasonStats");

            migrationBuilder.DropColumn(
                name: "Y_A",
                table: "SeasonStats");

            migrationBuilder.DropColumn(
                name: "Y_T",
                table: "SeasonStats");

            migrationBuilder.RenameColumn(
                name: "Tgt",
                table: "SeasonStats",
                newName: "TackleStatsId");

            migrationBuilder.RenameColumn(
                name: "RushYards",
                table: "SeasonStats",
                newName: "RushStatsId");

            migrationBuilder.RenameColumn(
                name: "RushTD",
                table: "SeasonStats",
                newName: "RecStatsId");

            migrationBuilder.RenameColumn(
                name: "RecYards",
                table: "SeasonStats",
                newName: "PuntStatsId");

            migrationBuilder.RenameColumn(
                name: "RecTD",
                table: "SeasonStats",
                newName: "PassStatsId");

            migrationBuilder.RenameColumn(
                name: "Rec",
                table: "SeasonStats",
                newName: "PRStatsId");

            migrationBuilder.RenameColumn(
                name: "PosRank",
                table: "SeasonStats",
                newName: "KRStatsId");

            migrationBuilder.RenameColumn(
                name: "PassYards",
                table: "SeasonStats",
                newName: "KOStatsId");

            migrationBuilder.RenameColumn(
                name: "PassTD",
                table: "SeasonStats",
                newName: "IntStatsId");

            migrationBuilder.RenameColumn(
                name: "OvrRank",
                table: "SeasonStats",
                newName: "FGStatsId");

            migrationBuilder.CreateTable(
                name: "FGStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShMade = table.Column<int>(type: "int", nullable: false),
                    ShAtt = table.Column<int>(type: "int", nullable: false),
                    MidMade = table.Column<int>(type: "int", nullable: false),
                    MidAtt = table.Column<int>(type: "int", nullable: false),
                    LongMade = table.Column<int>(type: "int", nullable: false),
                    LongAtt = table.Column<int>(type: "int", nullable: false),
                    Blk = table.Column<int>(type: "int", nullable: false),
                    Lng = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FGStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    INT = table.Column<int>(type: "int", nullable: false),
                    TD = table.Column<int>(type: "int", nullable: false),
                    Yds = table.Column<int>(type: "int", nullable: false),
                    Lng = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KOStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OnSide = table.Column<int>(type: "int", nullable: false),
                    OnSideRec = table.Column<int>(type: "int", nullable: false),
                    Kick = table.Column<int>(type: "int", nullable: false),
                    Yds = table.Column<int>(type: "int", nullable: false),
                    TB = table.Column<int>(type: "int", nullable: false),
                    OOB = table.Column<int>(type: "int", nullable: false),
                    Ret = table.Column<int>(type: "int", nullable: false),
                    RetYds = table.Column<int>(type: "int", nullable: false),
                    TD = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KOStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KRStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ret = table.Column<int>(type: "int", nullable: false),
                    Yds = table.Column<int>(type: "int", nullable: false),
                    TD = table.Column<int>(type: "int", nullable: false),
                    Plus20 = table.Column<int>(type: "int", nullable: false),
                    Plus40 = table.Column<int>(type: "int", nullable: false),
                    Lng = table.Column<int>(type: "int", nullable: false),
                    FC = table.Column<int>(type: "int", nullable: false),
                    Fmb = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KRStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PassStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cmp = table.Column<int>(type: "int", nullable: false),
                    INT = table.Column<int>(type: "int", nullable: false),
                    PR = table.Column<double>(type: "float", nullable: false),
                    Sck = table.Column<int>(type: "int", nullable: false),
                    SckYds = table.Column<int>(type: "int", nullable: false),
                    Yds = table.Column<int>(type: "int", nullable: false),
                    Att = table.Column<int>(type: "int", nullable: false),
                    TD = table.Column<int>(type: "int", nullable: false),
                    Plus20 = table.Column<int>(type: "int", nullable: false),
                    Plus40 = table.Column<int>(type: "int", nullable: false),
                    Reach1st = table.Column<int>(type: "int", nullable: false),
                    Lng = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PRStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ret = table.Column<int>(type: "int", nullable: false),
                    Yds = table.Column<int>(type: "int", nullable: false),
                    TD = table.Column<int>(type: "int", nullable: false),
                    Plus20 = table.Column<int>(type: "int", nullable: false),
                    Plus40 = table.Column<int>(type: "int", nullable: false),
                    Lng = table.Column<int>(type: "int", nullable: false),
                    FC = table.Column<int>(type: "int", nullable: false),
                    Fmb = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PuntStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Inside20 = table.Column<int>(type: "int", nullable: false),
                    Down = table.Column<int>(type: "int", nullable: false),
                    FC = table.Column<int>(type: "int", nullable: false),
                    Blk = table.Column<int>(type: "int", nullable: false),
                    Lng = table.Column<int>(type: "int", nullable: false),
                    Kick = table.Column<int>(type: "int", nullable: false),
                    Yds = table.Column<int>(type: "int", nullable: false),
                    TB = table.Column<int>(type: "int", nullable: false),
                    OOB = table.Column<int>(type: "int", nullable: false),
                    Ret = table.Column<int>(type: "int", nullable: false),
                    RetYds = table.Column<int>(type: "int", nullable: false),
                    TD = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuntStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rec = table.Column<int>(type: "int", nullable: false),
                    Fmb = table.Column<int>(type: "int", nullable: false),
                    YAC = table.Column<int>(type: "int", nullable: false),
                    Yds = table.Column<int>(type: "int", nullable: false),
                    Att = table.Column<int>(type: "int", nullable: false),
                    TD = table.Column<int>(type: "int", nullable: false),
                    Plus20 = table.Column<int>(type: "int", nullable: false),
                    Plus40 = table.Column<int>(type: "int", nullable: false),
                    Reach1st = table.Column<int>(type: "int", nullable: false),
                    Lng = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RushStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fmb = table.Column<int>(type: "int", nullable: false),
                    Yds = table.Column<int>(type: "int", nullable: false),
                    Att = table.Column<int>(type: "int", nullable: false),
                    TD = table.Column<int>(type: "int", nullable: false),
                    Plus20 = table.Column<int>(type: "int", nullable: false),
                    Plus40 = table.Column<int>(type: "int", nullable: false),
                    Reach1st = table.Column<int>(type: "int", nullable: false),
                    Lng = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RushStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TackleStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comb = table.Column<int>(type: "int", nullable: false),
                    Solo = table.Column<int>(type: "int", nullable: false),
                    Asst = table.Column<int>(type: "int", nullable: false),
                    Sck = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TackleStats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeasonStats_FGStatsId",
                table: "SeasonStats",
                column: "FGStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonStats_IntStatsId",
                table: "SeasonStats",
                column: "IntStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonStats_KOStatsId",
                table: "SeasonStats",
                column: "KOStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonStats_KRStatsId",
                table: "SeasonStats",
                column: "KRStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonStats_PassStatsId",
                table: "SeasonStats",
                column: "PassStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonStats_PRStatsId",
                table: "SeasonStats",
                column: "PRStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonStats_PuntStatsId",
                table: "SeasonStats",
                column: "PuntStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonStats_RecStatsId",
                table: "SeasonStats",
                column: "RecStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonStats_RushStatsId",
                table: "SeasonStats",
                column: "RushStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonStats_TackleStatsId",
                table: "SeasonStats",
                column: "TackleStatsId");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonStats_FGStats_FGStatsId",
                table: "SeasonStats",
                column: "FGStatsId",
                principalTable: "FGStats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonStats_IntStats_IntStatsId",
                table: "SeasonStats",
                column: "IntStatsId",
                principalTable: "IntStats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonStats_KOStats_KOStatsId",
                table: "SeasonStats",
                column: "KOStatsId",
                principalTable: "KOStats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonStats_KRStats_KRStatsId",
                table: "SeasonStats",
                column: "KRStatsId",
                principalTable: "KRStats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonStats_PRStats_PRStatsId",
                table: "SeasonStats",
                column: "PRStatsId",
                principalTable: "PRStats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonStats_PassStats_PassStatsId",
                table: "SeasonStats",
                column: "PassStatsId",
                principalTable: "PassStats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonStats_PuntStats_PuntStatsId",
                table: "SeasonStats",
                column: "PuntStatsId",
                principalTable: "PuntStats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonStats_RecStats_RecStatsId",
                table: "SeasonStats",
                column: "RecStatsId",
                principalTable: "RecStats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonStats_RushStats_RushStatsId",
                table: "SeasonStats",
                column: "RushStatsId",
                principalTable: "RushStats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonStats_TackleStats_TackleStatsId",
                table: "SeasonStats",
                column: "TackleStatsId",
                principalTable: "TackleStats",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonStats_FGStats_FGStatsId",
                table: "SeasonStats");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonStats_IntStats_IntStatsId",
                table: "SeasonStats");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonStats_KOStats_KOStatsId",
                table: "SeasonStats");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonStats_KRStats_KRStatsId",
                table: "SeasonStats");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonStats_PRStats_PRStatsId",
                table: "SeasonStats");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonStats_PassStats_PassStatsId",
                table: "SeasonStats");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonStats_PuntStats_PuntStatsId",
                table: "SeasonStats");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonStats_RecStats_RecStatsId",
                table: "SeasonStats");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonStats_RushStats_RushStatsId",
                table: "SeasonStats");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonStats_TackleStats_TackleStatsId",
                table: "SeasonStats");

            migrationBuilder.DropTable(
                name: "FGStats");

            migrationBuilder.DropTable(
                name: "IntStats");

            migrationBuilder.DropTable(
                name: "KOStats");

            migrationBuilder.DropTable(
                name: "KRStats");

            migrationBuilder.DropTable(
                name: "PassStats");

            migrationBuilder.DropTable(
                name: "PRStats");

            migrationBuilder.DropTable(
                name: "PuntStats");

            migrationBuilder.DropTable(
                name: "RecStats");

            migrationBuilder.DropTable(
                name: "RushStats");

            migrationBuilder.DropTable(
                name: "TackleStats");

            migrationBuilder.DropIndex(
                name: "IX_SeasonStats_FGStatsId",
                table: "SeasonStats");

            migrationBuilder.DropIndex(
                name: "IX_SeasonStats_IntStatsId",
                table: "SeasonStats");

            migrationBuilder.DropIndex(
                name: "IX_SeasonStats_KOStatsId",
                table: "SeasonStats");

            migrationBuilder.DropIndex(
                name: "IX_SeasonStats_KRStatsId",
                table: "SeasonStats");

            migrationBuilder.DropIndex(
                name: "IX_SeasonStats_PassStatsId",
                table: "SeasonStats");

            migrationBuilder.DropIndex(
                name: "IX_SeasonStats_PRStatsId",
                table: "SeasonStats");

            migrationBuilder.DropIndex(
                name: "IX_SeasonStats_PuntStatsId",
                table: "SeasonStats");

            migrationBuilder.DropIndex(
                name: "IX_SeasonStats_RecStatsId",
                table: "SeasonStats");

            migrationBuilder.DropIndex(
                name: "IX_SeasonStats_RushStatsId",
                table: "SeasonStats");

            migrationBuilder.DropIndex(
                name: "IX_SeasonStats_TackleStatsId",
                table: "SeasonStats");

            migrationBuilder.RenameColumn(
                name: "TackleStatsId",
                table: "SeasonStats",
                newName: "Tgt");

            migrationBuilder.RenameColumn(
                name: "RushStatsId",
                table: "SeasonStats",
                newName: "RushYards");

            migrationBuilder.RenameColumn(
                name: "RecStatsId",
                table: "SeasonStats",
                newName: "RushTD");

            migrationBuilder.RenameColumn(
                name: "PuntStatsId",
                table: "SeasonStats",
                newName: "RecYards");

            migrationBuilder.RenameColumn(
                name: "PassStatsId",
                table: "SeasonStats",
                newName: "RecTD");

            migrationBuilder.RenameColumn(
                name: "PRStatsId",
                table: "SeasonStats",
                newName: "Rec");

            migrationBuilder.RenameColumn(
                name: "KRStatsId",
                table: "SeasonStats",
                newName: "PosRank");

            migrationBuilder.RenameColumn(
                name: "KOStatsId",
                table: "SeasonStats",
                newName: "PassYards");

            migrationBuilder.RenameColumn(
                name: "IntStatsId",
                table: "SeasonStats",
                newName: "PassTD");

            migrationBuilder.RenameColumn(
                name: "FGStatsId",
                table: "SeasonStats",
                newName: "OvrRank");

            migrationBuilder.AddColumn<int>(
                name: "Att",
                table: "SeasonStats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Car",
                table: "SeasonStats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cmp",
                table: "SeasonStats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FantPt",
                table: "SeasonStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Fmb",
                table: "SeasonStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FmbRec",
                table: "SeasonStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FranchiseId",
                table: "SeasonStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Games",
                table: "SeasonStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Int",
                table: "SeasonStats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VBD",
                table: "SeasonStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_SeasonStats_FranchiseId",
                table: "SeasonStats",
                column: "FranchiseId");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonStats_Franchises_FranchiseId",
                table: "SeasonStats",
                column: "FranchiseId",
                principalTable: "Franchises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
