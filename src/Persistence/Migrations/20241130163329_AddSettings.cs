using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Crpg.Persistence.Migrations;

/// <inheritdoc />
public partial class AddSettings : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "settings",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                discord = table.Column<string>(type: "text", nullable: false),
                steam = table.Column<string>(type: "text", nullable: false),
                patreon = table.Column<string>(type: "text", nullable: false),
                github = table.Column<string>(type: "text", nullable: false),
                reddit = table.Column<string>(type: "text", nullable: false),
                mod_db = table.Column<string>(type: "text", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_settings", x => x.id);
            });

        migrationBuilder.InsertData(
            table: "settings",
            columns: new[] { "id", "discord", "steam", "patreon", "github", "reddit", "mod_db" },
            values: new object[]
            {
                1,
                "https://discord.gg/c-rpg",
                "https://steamcommunity.com/sharedfiles/filedetails/?id=2878356589",
                "https://www.patreon.com/crpg",
                "https://github.com/crpg2/crpg",
                "https://www.reddit.com/r/CRPG_Bannerlord",
                "https://www.moddb.com/mods/crpg",
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "settings");
    }
}
