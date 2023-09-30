using Crpg.Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Crpg.Persistence.Migrations;

/// <inheritdoc />
public partial class AddGameServerConfiguration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()

            .Annotation("Npgsql:Enum:restriction_type", "all,join,chat")
            .Annotation("Npgsql:Enum:role", "user,moderator,game_admin,admin")
            .OldAnnotation("Npgsql:Enum:role", "user,moderator,game_admin,admin");

        migrationBuilder.CreateTable(
            name: "game_server_configs",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                servername = table.Column<string>(name: "server_name", type: "text", nullable: false),
                region = table.Column<Region>(type: "region", nullable: false),
                serverinstance = table.Column<int>(name: "server_instance", type: "integer", nullable: false),
                gamemode = table.Column<int>(name: "game_mode", type: "integer", nullable: false),
                gamepassword = table.Column<string>(name: "game_password", type: "text", nullable: true),
                welcomemessage = table.Column<string>(name: "welcome_message", type: "text", nullable: true),
                maps = table.Column<List<string>>(type: "text[]", nullable: false),
                allowpollstokickplayers = table.Column<bool>(name: "allow_polls_to_kick_players", type: "boolean", nullable: true),
                allowpollstochangemaps = table.Column<bool>(name: "allow_polls_to_change_maps", type: "boolean", nullable: true),
                disableculturevoting = table.Column<bool>(name: "disable_culture_voting", type: "boolean", nullable: true),
                cultureteam1 = table.Column<Culture>(name: "culture_team1", type: "culture", nullable: false),
                cultureteam2 = table.Column<Culture>(name: "culture_team2", type: "culture", nullable: false),
                maxnumberofplayers = table.Column<int>(name: "max_number_of_players", type: "integer", nullable: false),
                autoteambalancethreshold = table.Column<int>(name: "auto_team_balance_threshold", type: "integer", nullable: true),
                friendlyfiredamagemeleefriendpercent = table.Column<int>(name: "friendly_fire_damage_melee_friend_percent", type: "integer", nullable: true),
                friendlyfiredamagemeleeselfpercent = table.Column<int>(name: "friendly_fire_damage_melee_self_percent", type: "integer", nullable: true),
                friendlyfiredamagerangedfriendpercent = table.Column<int>(name: "friendly_fire_damage_ranged_friend_percent", type: "integer", nullable: true),
                friendlyfiredamagerangedselfpercent = table.Column<int>(name: "friendly_fire_damage_ranged_self_percent", type: "integer", nullable: true),
                numberofbotsteam1 = table.Column<int>(name: "number_of_bots_team1", type: "integer", nullable: false),
                numberofbotsteam2 = table.Column<int>(name: "number_of_bots_team2", type: "integer", nullable: false),
                minnumberofplayersformatchstart = table.Column<int>(name: "min_number_of_players_for_match_start", type: "integer", nullable: true),
                roundtotal = table.Column<int>(name: "round_total", type: "integer", nullable: true),
                maptimelimit = table.Column<int>(name: "map_time_limit", type: "integer", nullable: true),
                roundtimelimit = table.Column<int>(name: "round_time_limit", type: "integer", nullable: true),
                warmuptimelimit = table.Column<int>(name: "warmup_time_limit", type: "integer", nullable: true),
                roundpreparationtimelimit = table.Column<int>(name: "round_preparation_time_limit", type: "integer", nullable: true),
                enableautomatedbattleswitching = table.Column<bool>(name: "enable_automated_battle_switching", type: "boolean", nullable: true),
                respawnperiodteam1 = table.Column<int>(name: "respawn_period_team1", type: "integer", nullable: true),
                respawnperiodteam2 = table.Column<int>(name: "respawn_period_team2", type: "integer", nullable: true),
                setautomatedbattlecount = table.Column<bool>(name: "set_automated_battle_count", type: "boolean", nullable: true),
                minscoretowinduel = table.Column<int>(name: "min_score_to_win_duel", type: "integer", nullable: true),
                endgameaftermissionisover = table.Column<bool>(name: "end_game_after_mission_is_over", type: "boolean", nullable: true),
                crpgteambalancerclangroupsizepenalty = table.Column<float>(name: "crpg_team_balancer_clan_group_size_penalty", type: "real", nullable: true),
                crpgexperiencemultiplier = table.Column<float>(name: "crpg_experience_multiplier", type: "real", nullable: true),
                crpgrewardtick = table.Column<int>(name: "crpg_reward_tick", type: "integer", nullable: true),
                crpgteambalanceonce = table.Column<bool>(name: "crpg_team_balance_once", type: "boolean", nullable: true),
                crpghappyhours = table.Column<TimeSpan>(name: "crpg_happy_hours", type: "interval", nullable: true),
                crpgapplyharmonypatches = table.Column<bool>(name: "crpg_apply_harmony_patches", type: "boolean", nullable: true),
                updatedat = table.Column<DateTime>(name: "updated_at", type: "timestamp with time zone", nullable: false),
                createdat = table.Column<DateTime>(name: "created_at", type: "timestamp with time zone", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_game_server_configs", x => x.id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "game_server_configs");

        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:Enum:role", "user,moderator,game_admin,admin")
            .OldAnnotation("Npgsql:Enum:role", "user,moderator,game_admin,admin");
    }
}
