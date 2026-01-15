using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QueenFlight.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LinkUserPreferenceToAirport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "iata_code",
                table: "airports",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_airports_iata_code",
                table: "airports",
                column: "iata_code");

            migrationBuilder.CreateIndex(
                name: "IX_user_preferences_home_airport_iata",
                table: "user_preferences",
                column: "home_airport_iata");

            migrationBuilder.AddForeignKey(
                name: "FK_user_preferences_airports_home_airport_iata",
                table: "user_preferences",
                column: "home_airport_iata",
                principalTable: "airports",
                principalColumn: "iata_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_preferences_airports_home_airport_iata",
                table: "user_preferences");

            migrationBuilder.DropIndex(
                name: "IX_user_preferences_home_airport_iata",
                table: "user_preferences");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_airports_iata_code",
                table: "airports");

            migrationBuilder.AlterColumn<string>(
                name: "iata_code",
                table: "airports",
                type: "character varying(3)",
                maxLength: 3,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);
        }
    }
}
