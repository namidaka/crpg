using System;
using Crpg.Domain.Entities.Servers;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Crpg.Persistence.Migrations;

/// <inheritdoc />
public partial class AddGamemodeStatistics : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:Enum:game_mode", "crpg_battle,crpg_conquest,crpgdtv,crpg_duel,crpg_siege,crpg_team_deathmatch,crpg_skirmish,crpg_unknown_game_mode");

        migrationBuilder.CreateTable(
            name: "character_statistics",
            columns: table => new
            {
                character_id = table.Column<int>(type: "integer", nullable: false),
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                kills = table.Column<int>(type: "integer", nullable: false),
                deaths = table.Column<int>(type: "integer", nullable: false),
                assists = table.Column<int>(type: "integer", nullable: false),
                play_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                game_mode = table.Column<GameMode>(type: "game_mode", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_character_statistics", x => new { x.character_id, x.id });
                table.ForeignKey(
                    name: "fk_character_statistics_characters_character_id",
                    column: x => x.character_id,
                    principalTable: "characters",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.Sql($@"
                INSERT INTO character_statistics (character_id, kills, deaths, assists, play_time, game_mode)
                SELECT id, kills, deaths, assists, play_time, 'crpg_battle'
                FROM characters");

        migrationBuilder.DropColumn(
            name: "assists",
            table: "characters");

        migrationBuilder.DropColumn(
            name: "deaths",
            table: "characters");

        migrationBuilder.DropColumn(
            name: "kills",
            table: "characters");

        migrationBuilder.DropColumn(
            name: "play_time",
            table: "characters");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "assists",
            table: "characters",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "deaths",
            table: "characters",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "kills",
            table: "characters",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<TimeSpan>(
            name: "play_time",
            table: "characters",
            type: "interval",
            nullable: false,
            defaultValue: new TimeSpan(0, 0, 0, 0, 0));

        migrationBuilder.Sql(
        @"UPDATE characters
                SET
                    kills = cs_aggregated.sum_kills,
                    deaths = cs_aggregated.sum_deaths,
                    assists = cs_aggregated.sum_assists,
                    play_time = cs_aggregated.sum_play_time
                FROM (
                    SELECT
                        character_id,
                        SUM(kills) AS sum_kills,
                        SUM(deaths) AS sum_deaths,
                        SUM(assists) AS sum_assists,
                        SUM(play_time) AS sum_play_time
                    FROM character_statistics
                    GROUP BY character_id
                ) AS cs_aggregated
            WHERE characters.id = cs_aggregated.character_id;");

        migrationBuilder.DropTable(
            name: "character_statistics");

        migrationBuilder.AlterDatabase()
            .OldAnnotation("Npgsql:Enum:game_mode", "crpg_battle,crpg_conquest,crpgdtv,crpg_duel,crpg_siege,crpg_team_deathmatch,crpg_skirmish,crpg_unknown_game_mode");
    }
}
