using Microsoft.EntityFrameworkCore.Migrations;

namespace Crpg.Persistence.Migrations;

/// <inheritdoc />
public partial class AddMapConfiguration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_map_game_server_configs_game_server_config_id",
            table: "map");

        migrationBuilder.DropPrimaryKey(
            name: "pk_map",
            table: "map");

        migrationBuilder.RenameTable(
            name: "map",
            newName: "maps");

        migrationBuilder.RenameIndex(
            name: "ix_map_game_server_config_id",
            table: "maps",
            newName: "ix_maps_game_server_config_id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_maps",
            table: "maps",
            column: "id");

        migrationBuilder.AddForeignKey(
            name: "fk_maps_game_server_configs_game_server_config_id",
            table: "maps",
            column: "game_server_config_id",
            principalTable: "game_server_configs",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_maps_game_server_configs_game_server_config_id",
            table: "maps");

        migrationBuilder.DropPrimaryKey(
            name: "pk_maps",
            table: "maps");

        migrationBuilder.RenameTable(
            name: "maps",
            newName: "map");

        migrationBuilder.RenameIndex(
            name: "ix_maps_game_server_config_id",
            table: "map",
            newName: "ix_map_game_server_config_id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_map",
            table: "map",
            column: "id");

        migrationBuilder.AddForeignKey(
            name: "fk_map_game_server_configs_game_server_config_id",
            table: "map",
            column: "game_server_config_id",
            principalTable: "game_server_configs",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
