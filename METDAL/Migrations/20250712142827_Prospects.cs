using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace METDAL.Migrations
{
    /// <inheritdoc />
    public partial class Prospects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prospects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    HandSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArmLength = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Wingspan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FortyYardDash = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BenchPress = table.Column<int>(type: "int", nullable: true),
                    VertJump = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BroadJump = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThreeConeDrill = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TwentyYardShuttle = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AthScore = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prospects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prospects_Players_Id",
                        column: x => x.Id,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prospects");
        }
    }
}
