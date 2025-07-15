using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace METDAL.Migrations
{
    /// <inheritdoc />
    public partial class preImportContracts2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractYears");

            // 1. Add a temporary column to hold the enum values
            migrationBuilder.AddColumn<int>(
                name: "AbbEnum",
                table: "Franchises",
                type: "int",
                nullable: true);

            // 2. Map existing string values to their enum equivalents
            // The mapping is straightforward since your enum names match the string values
            migrationBuilder.Sql(@"
        UPDATE Franchises SET AbbEnum = 
            CASE Abb 
                WHEN 'ARI' THEN 0
                WHEN 'ATL' THEN 1
                WHEN 'BAL' THEN 2
                WHEN 'BUF' THEN 3
                WHEN 'CAR' THEN 4
                WHEN 'CHI' THEN 5
                WHEN 'CIN' THEN 6
                WHEN 'CLE' THEN 7
                WHEN 'DAL' THEN 8
                WHEN 'DEN' THEN 9
                WHEN 'DET' THEN 10
                WHEN 'GB' THEN 11
                WHEN 'HOU' THEN 12
                WHEN 'IND' THEN 13
                WHEN 'JAX' THEN 14
                WHEN 'KC' THEN 15
                WHEN 'LAC' THEN 16
                WHEN 'LAR' THEN 17
                WHEN 'LV' THEN 18
                WHEN 'MIA' THEN 19
                WHEN 'MIN' THEN 20
                WHEN 'NE' THEN 21
                WHEN 'NO' THEN 22
                WHEN 'NYG' THEN 23
                WHEN 'NYJ' THEN 24
                WHEN 'PHI' THEN 25
                WHEN 'PIT' THEN 26
                WHEN 'SEA' THEN 27
                WHEN 'SF' THEN 28
                WHEN 'TB' THEN 29
                WHEN 'TEN' THEN 30
                WHEN 'WSH' THEN 31
                ELSE 0 -- Default to ARI if unknown
            END
    ");

            // 3. Drop the original column
            migrationBuilder.DropColumn(
                name: "Abb",
                table: "Franchises");

            // 4. Rename the new column to the original name
            migrationBuilder.RenameColumn(
                name: "AbbEnum",
                table: "Franchises",
                newName: "Abb");

            // 5. Make sure the column is not nullable
            migrationBuilder.AlterColumn<int>(
                name: "Abb",
                table: "Franchises",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "Garanteed",
                table: "Contracts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "Length",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "Total",
                table: "Contracts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "YearSigned",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Garanteed",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "YearSigned",
                table: "Contracts");

            // 1. Add a temporary string column
            migrationBuilder.AddColumn<string>(
                name: "AbbString",
                table: "Franchises",
                type: "nvarchar(5)",
                nullable: true);

            // 2. Convert the enum values back to strings
            migrationBuilder.Sql(@"
        UPDATE Franchises SET AbbString = 
            CASE Abb 
                WHEN 0 THEN 'ARI'
                WHEN 1 THEN 'ATL'
                WHEN 2 THEN 'BAL'
                WHEN 3 THEN 'BUF'
                WHEN 4 THEN 'CAR'
                WHEN 5 THEN 'CHI'
                WHEN 6 THEN 'CIN'
                WHEN 7 THEN 'CLE'
                WHEN 8 THEN 'DAL'
                WHEN 9 THEN 'DEN'
                WHEN 10 THEN 'DET'
                WHEN 11 THEN 'GB'
                WHEN 12 THEN 'HOU'
                WHEN 13 THEN 'IND'
                WHEN 14 THEN 'JAX'
                WHEN 15 THEN 'KC'
                WHEN 16 THEN 'LAC'
                WHEN 17 THEN 'LAR'
                WHEN 18 THEN 'LV'
                WHEN 19 THEN 'MIA'
                WHEN 20 THEN 'MIN'
                WHEN 21 THEN 'NE'
                WHEN 22 THEN 'NO'
                WHEN 23 THEN 'NYG'
                WHEN 24 THEN 'NYJ'
                WHEN 25 THEN 'PHI'
                WHEN 26 THEN 'PIT'
                WHEN 27 THEN 'SEA'
                WHEN 28 THEN 'SF'
                WHEN 29 THEN 'TB'
                WHEN 30 THEN 'TEN'
                WHEN 31 THEN 'WSH'
                ELSE 'UNK'
            END
    ");

            // 3. Drop the enum column
            migrationBuilder.DropColumn(
                name: "Abb",
                table: "Franchises");

            // 4. Rename the string column to the original name
            migrationBuilder.RenameColumn(
                name: "AbbString",
                table: "Franchises",
                newName: "Abb");

            // 5. Set appropriate properties
            migrationBuilder.AlterColumn<string>(
                name: "Abb",
                table: "Franchises",
                type: "nvarchar(5)",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "ContractYears",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    Earned = table.Column<double>(type: "float", nullable: false),
                    GaranteedMoney = table.Column<double>(type: "float", nullable: false),
                    Incentives = table.Column<double>(type: "float", nullable: false),
                    Season = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractYears", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractYears_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractYears_ContractId",
                table: "ContractYears",
                column: "ContractId");
        }
    }
}
