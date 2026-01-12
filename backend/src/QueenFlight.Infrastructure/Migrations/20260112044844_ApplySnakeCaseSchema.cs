using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QueenFlight.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApplySnakeCaseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AircraftModels_Manufacturers_ManufacturerId",
                table: "AircraftModels");

            migrationBuilder.DropForeignKey(
                name: "FK_Aircrafts_AircraftModels_ModelId",
                table: "Aircrafts");

            migrationBuilder.DropForeignKey(
                name: "FK_Aircrafts_Airlines_AirlineId",
                table: "Aircrafts");

            migrationBuilder.DropForeignKey(
                name: "FK_Airlines_Countries_CountryCode",
                table: "Airlines");

            migrationBuilder.DropForeignKey(
                name: "FK_Airports_Countries_CountryCode",
                table: "Airports");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferences_Airports_HomeAirportIata",
                table: "UserPreferences");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferences_Users_UserId",
                table: "UserPreferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Manufacturers",
                table: "Manufacturers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Countries",
                table: "Countries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Airports",
                table: "Airports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Airlines",
                table: "Airlines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Aircrafts",
                table: "Aircrafts");

            migrationBuilder.DropIndex(
                name: "IX_Aircrafts_Icao24",
                table: "Aircrafts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPreferences",
                table: "UserPreferences");

            migrationBuilder.DropIndex(
                name: "IX_UserPreferences_HomeAirportIata",
                table: "UserPreferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AircraftModels",
                table: "AircraftModels");

            migrationBuilder.DropColumn(
                name: "Timezone",
                table: "Airports");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Manufacturers",
                newName: "manufacturers");

            migrationBuilder.RenameTable(
                name: "Countries",
                newName: "countries");

            migrationBuilder.RenameTable(
                name: "Airports",
                newName: "airports");

            migrationBuilder.RenameTable(
                name: "Airlines",
                newName: "airlines");

            migrationBuilder.RenameTable(
                name: "Aircrafts",
                newName: "aircrafts");

            migrationBuilder.RenameTable(
                name: "UserPreferences",
                newName: "user_preferences");

            migrationBuilder.RenameTable(
                name: "AircraftModels",
                newName: "aircraft_models");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "users",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "users",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "users",
                newName: "password_hash");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "users",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "users",
                newName: "IX_users_email");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "manufacturers",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "manufacturers",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Region",
                table: "countries",
                newName: "region");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "countries",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "IsoCode",
                table: "countries",
                newName: "iso_code");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "airports",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "airports",
                newName: "longitude");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "airports",
                newName: "latitude");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "airports",
                newName: "city");

            migrationBuilder.RenameColumn(
                name: "IcaoCode",
                table: "airports",
                newName: "icao_code");

            migrationBuilder.RenameColumn(
                name: "CountryCode",
                table: "airports",
                newName: "country_code");

            migrationBuilder.RenameColumn(
                name: "IataCode",
                table: "airports",
                newName: "iata_code");

            migrationBuilder.RenameIndex(
                name: "IX_Airports_IcaoCode",
                table: "airports",
                newName: "IX_airports_icao_code");

            migrationBuilder.RenameIndex(
                name: "IX_Airports_CountryCode",
                table: "airports",
                newName: "IX_airports_country_code");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "airlines",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "airlines",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "LogoUrl",
                table: "airlines",
                newName: "logo_url");

            migrationBuilder.RenameColumn(
                name: "IcaoCode",
                table: "airlines",
                newName: "icao_code");

            migrationBuilder.RenameColumn(
                name: "IataCode",
                table: "airlines",
                newName: "iata_code");

            migrationBuilder.RenameColumn(
                name: "CountryCode",
                table: "airlines",
                newName: "country_code");

            migrationBuilder.RenameColumn(
                name: "CallsignPrefix",
                table: "airlines",
                newName: "callsign_prefix");

            migrationBuilder.RenameIndex(
                name: "IX_Airlines_IcaoCode",
                table: "airlines",
                newName: "IX_airlines_icao_code");

            migrationBuilder.RenameIndex(
                name: "IX_Airlines_CountryCode",
                table: "airlines",
                newName: "IX_airlines_country_code");

            migrationBuilder.RenameColumn(
                name: "Owner",
                table: "aircrafts",
                newName: "owner");

            migrationBuilder.RenameColumn(
                name: "Icao24",
                table: "aircrafts",
                newName: "icao24");

            migrationBuilder.RenameColumn(
                name: "RegistrationNumber",
                table: "aircrafts",
                newName: "registration_number");

            migrationBuilder.RenameColumn(
                name: "ModelId",
                table: "aircrafts",
                newName: "model_id");

            migrationBuilder.RenameColumn(
                name: "LastMetadataUpdate",
                table: "aircrafts",
                newName: "last_metadata_update");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "aircrafts",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "AirlineId",
                table: "aircrafts",
                newName: "airline_id");

            migrationBuilder.RenameIndex(
                name: "IX_Aircrafts_ModelId",
                table: "aircrafts",
                newName: "IX_aircrafts_model_id");

            migrationBuilder.RenameIndex(
                name: "IX_Aircrafts_AirlineId",
                table: "aircrafts",
                newName: "IX_aircrafts_airline_id");

            migrationBuilder.RenameColumn(
                name: "ShowWeather",
                table: "user_preferences",
                newName: "show_weather");

            migrationBuilder.RenameColumn(
                name: "HomeAirportIata",
                table: "user_preferences",
                newName: "home_airport_iata");

            migrationBuilder.RenameColumn(
                name: "DefaultMapStyle",
                table: "user_preferences",
                newName: "default_map_style");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_preferences",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "aircraft_models",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "aircraft_models",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WakeTurbulenceCategory",
                table: "aircraft_models",
                newName: "wake_turbulence_category");

            migrationBuilder.RenameColumn(
                name: "ManufacturerId",
                table: "aircraft_models",
                newName: "manufacturer_id");

            migrationBuilder.RenameColumn(
                name: "IcaoTypeCode",
                table: "aircraft_models",
                newName: "icao_type_code");

            migrationBuilder.RenameIndex(
                name: "IX_AircraftModels_ManufacturerId",
                table: "aircraft_models",
                newName: "IX_aircraft_models_manufacturer_id");

            migrationBuilder.RenameIndex(
                name: "IX_AircraftModels_IcaoTypeCode",
                table: "aircraft_models",
                newName: "IX_aircraft_models_icao_type_code");

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "member",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "users",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "iata_code",
                table: "airports",
                type: "character varying(3)",
                maxLength: 3,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AddColumn<string>(
                name: "ident",
                table: "airports",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "iata_code",
                table: "airlines",
                type: "character(2)",
                fixedLength: true,
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2)",
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "last_metadata_update",
                table: "aircrafts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "aircrafts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<bool>(
                name: "show_weather",
                table: "user_preferences",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "default_map_style",
                table: "user_preferences",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "dark",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_manufacturers",
                table: "manufacturers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_countries",
                table: "countries",
                column: "iso_code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_airports",
                table: "airports",
                column: "ident");

            migrationBuilder.AddPrimaryKey(
                name: "PK_airlines",
                table: "airlines",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_aircrafts",
                table: "aircrafts",
                column: "icao24");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_preferences",
                table: "user_preferences",
                column: "user_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_aircraft_models",
                table: "aircraft_models",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_airports_iata_code",
                table: "airports",
                column: "iata_code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_aircraft_models_manufacturers_manufacturer_id",
                table: "aircraft_models",
                column: "manufacturer_id",
                principalTable: "manufacturers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_aircrafts_aircraft_models_model_id",
                table: "aircrafts",
                column: "model_id",
                principalTable: "aircraft_models",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_aircrafts_airlines_airline_id",
                table: "aircrafts",
                column: "airline_id",
                principalTable: "airlines",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_airlines_countries_country_code",
                table: "airlines",
                column: "country_code",
                principalTable: "countries",
                principalColumn: "iso_code");

            migrationBuilder.AddForeignKey(
                name: "FK_airports_countries_country_code",
                table: "airports",
                column: "country_code",
                principalTable: "countries",
                principalColumn: "iso_code");

            migrationBuilder.AddForeignKey(
                name: "FK_user_preferences_users_user_id",
                table: "user_preferences",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_aircraft_models_manufacturers_manufacturer_id",
                table: "aircraft_models");

            migrationBuilder.DropForeignKey(
                name: "FK_aircrafts_aircraft_models_model_id",
                table: "aircrafts");

            migrationBuilder.DropForeignKey(
                name: "FK_aircrafts_airlines_airline_id",
                table: "aircrafts");

            migrationBuilder.DropForeignKey(
                name: "FK_airlines_countries_country_code",
                table: "airlines");

            migrationBuilder.DropForeignKey(
                name: "FK_airports_countries_country_code",
                table: "airports");

            migrationBuilder.DropForeignKey(
                name: "FK_user_preferences_users_user_id",
                table: "user_preferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_manufacturers",
                table: "manufacturers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_countries",
                table: "countries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_airports",
                table: "airports");

            migrationBuilder.DropIndex(
                name: "IX_airports_iata_code",
                table: "airports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_airlines",
                table: "airlines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_aircrafts",
                table: "aircrafts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_preferences",
                table: "user_preferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_aircraft_models",
                table: "aircraft_models");

            migrationBuilder.DropColumn(
                name: "ident",
                table: "airports");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "manufacturers",
                newName: "Manufacturers");

            migrationBuilder.RenameTable(
                name: "countries",
                newName: "Countries");

            migrationBuilder.RenameTable(
                name: "airports",
                newName: "Airports");

            migrationBuilder.RenameTable(
                name: "airlines",
                newName: "Airlines");

            migrationBuilder.RenameTable(
                name: "aircrafts",
                newName: "Aircrafts");

            migrationBuilder.RenameTable(
                name: "user_preferences",
                newName: "UserPreferences");

            migrationBuilder.RenameTable(
                name: "aircraft_models",
                newName: "AircraftModels");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "Users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Users",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_users_email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Manufacturers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Manufacturers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "region",
                table: "Countries",
                newName: "Region");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Countries",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "iso_code",
                table: "Countries",
                newName: "IsoCode");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Airports",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "longitude",
                table: "Airports",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "latitude",
                table: "Airports",
                newName: "Latitude");

            migrationBuilder.RenameColumn(
                name: "city",
                table: "Airports",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "icao_code",
                table: "Airports",
                newName: "IcaoCode");

            migrationBuilder.RenameColumn(
                name: "iata_code",
                table: "Airports",
                newName: "IataCode");

            migrationBuilder.RenameColumn(
                name: "country_code",
                table: "Airports",
                newName: "CountryCode");

            migrationBuilder.RenameIndex(
                name: "IX_airports_icao_code",
                table: "Airports",
                newName: "IX_Airports_IcaoCode");

            migrationBuilder.RenameIndex(
                name: "IX_airports_country_code",
                table: "Airports",
                newName: "IX_Airports_CountryCode");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Airlines",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Airlines",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "logo_url",
                table: "Airlines",
                newName: "LogoUrl");

            migrationBuilder.RenameColumn(
                name: "icao_code",
                table: "Airlines",
                newName: "IcaoCode");

            migrationBuilder.RenameColumn(
                name: "iata_code",
                table: "Airlines",
                newName: "IataCode");

            migrationBuilder.RenameColumn(
                name: "country_code",
                table: "Airlines",
                newName: "CountryCode");

            migrationBuilder.RenameColumn(
                name: "callsign_prefix",
                table: "Airlines",
                newName: "CallsignPrefix");

            migrationBuilder.RenameIndex(
                name: "IX_airlines_icao_code",
                table: "Airlines",
                newName: "IX_Airlines_IcaoCode");

            migrationBuilder.RenameIndex(
                name: "IX_airlines_country_code",
                table: "Airlines",
                newName: "IX_Airlines_CountryCode");

            migrationBuilder.RenameColumn(
                name: "owner",
                table: "Aircrafts",
                newName: "Owner");

            migrationBuilder.RenameColumn(
                name: "icao24",
                table: "Aircrafts",
                newName: "Icao24");

            migrationBuilder.RenameColumn(
                name: "registration_number",
                table: "Aircrafts",
                newName: "RegistrationNumber");

            migrationBuilder.RenameColumn(
                name: "model_id",
                table: "Aircrafts",
                newName: "ModelId");

            migrationBuilder.RenameColumn(
                name: "last_metadata_update",
                table: "Aircrafts",
                newName: "LastMetadataUpdate");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Aircrafts",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "airline_id",
                table: "Aircrafts",
                newName: "AirlineId");

            migrationBuilder.RenameIndex(
                name: "IX_aircrafts_model_id",
                table: "Aircrafts",
                newName: "IX_Aircrafts_ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_aircrafts_airline_id",
                table: "Aircrafts",
                newName: "IX_Aircrafts_AirlineId");

            migrationBuilder.RenameColumn(
                name: "show_weather",
                table: "UserPreferences",
                newName: "ShowWeather");

            migrationBuilder.RenameColumn(
                name: "home_airport_iata",
                table: "UserPreferences",
                newName: "HomeAirportIata");

            migrationBuilder.RenameColumn(
                name: "default_map_style",
                table: "UserPreferences",
                newName: "DefaultMapStyle");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserPreferences",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "AircraftModels",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AircraftModels",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "wake_turbulence_category",
                table: "AircraftModels",
                newName: "WakeTurbulenceCategory");

            migrationBuilder.RenameColumn(
                name: "manufacturer_id",
                table: "AircraftModels",
                newName: "ManufacturerId");

            migrationBuilder.RenameColumn(
                name: "icao_type_code",
                table: "AircraftModels",
                newName: "IcaoTypeCode");

            migrationBuilder.RenameIndex(
                name: "IX_aircraft_models_manufacturer_id",
                table: "AircraftModels",
                newName: "IX_AircraftModels_ManufacturerId");

            migrationBuilder.RenameIndex(
                name: "IX_aircraft_models_icao_type_code",
                table: "AircraftModels",
                newName: "IX_AircraftModels_IcaoTypeCode");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "member");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Users",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "IataCode",
                table: "Airports",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Timezone",
                table: "Airports",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IataCode",
                table: "Airlines",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(2)",
                oldFixedLength: true,
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastMetadataUpdate",
                table: "Aircrafts",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Aircrafts",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<bool>(
                name: "ShowWeather",
                table: "UserPreferences",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "DefaultMapStyle",
                table: "UserPreferences",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "dark");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Manufacturers",
                table: "Manufacturers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Countries",
                table: "Countries",
                column: "IsoCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Airports",
                table: "Airports",
                column: "IataCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Airlines",
                table: "Airlines",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Aircrafts",
                table: "Aircrafts",
                column: "Icao24");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPreferences",
                table: "UserPreferences",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AircraftModels",
                table: "AircraftModels",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Aircrafts_Icao24",
                table: "Aircrafts",
                column: "Icao24",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_HomeAirportIata",
                table: "UserPreferences",
                column: "HomeAirportIata");

            migrationBuilder.AddForeignKey(
                name: "FK_AircraftModels_Manufacturers_ManufacturerId",
                table: "AircraftModels",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Aircrafts_AircraftModels_ModelId",
                table: "Aircrafts",
                column: "ModelId",
                principalTable: "AircraftModels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Aircrafts_Airlines_AirlineId",
                table: "Aircrafts",
                column: "AirlineId",
                principalTable: "Airlines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Airlines_Countries_CountryCode",
                table: "Airlines",
                column: "CountryCode",
                principalTable: "Countries",
                principalColumn: "IsoCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Airports_Countries_CountryCode",
                table: "Airports",
                column: "CountryCode",
                principalTable: "Countries",
                principalColumn: "IsoCode");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferences_Airports_HomeAirportIata",
                table: "UserPreferences",
                column: "HomeAirportIata",
                principalTable: "Airports",
                principalColumn: "IataCode");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferences_Users_UserId",
                table: "UserPreferences",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
