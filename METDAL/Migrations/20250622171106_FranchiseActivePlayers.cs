using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace METDAL.Migrations
{
    /// <inheritdoc />
    public partial class FranchiseActivePlayers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE VIEW FranchiseActivePlayers AS
                    SELECT c.FranchiseId FranchiseId, c.PlayerId playerId 
                    FROM Contracts c
                    WHERE c.Active = 1;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP VIEW FranchiseActivePlayers;
            ");
        }
    }
}
