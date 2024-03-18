using System;
using Crpg.Domain.Entities.Servers;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Crpg.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGamemodeStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:activity_log_type", "user_created,user_deleted,user_renamed,user_rewarded,item_bought,item_sold,item_broke,item_reforged,item_repaired,item_upgraded,character_created,character_deleted,character_rating_reset,character_respecialized,character_retired,character_rewarded,character_earned,server_joined,chat_message_sent,team_hit,clan_armory_add_item,clan_armory_remove_item,clan_armory_return_item,clan_armory_borrow_item")
                .Annotation("Npgsql:Enum:battle_fighter_application_status", "pending,declined,accepted")
                .Annotation("Npgsql:Enum:battle_mercenary_application_status", "pending,declined,accepted")
                .Annotation("Npgsql:Enum:battle_phase", "preparation,hiring,scheduled,live,end")
                .Annotation("Npgsql:Enum:battle_side", "attacker,defender")
                .Annotation("Npgsql:Enum:character_class", "peasant,infantry,shock_infantry,skirmisher,crossbowman,archer,cavalry,mounted_archer")
                .Annotation("Npgsql:Enum:clan_invitation_status", "pending,declined,accepted")
                .Annotation("Npgsql:Enum:clan_invitation_type", "request,offer")
                .Annotation("Npgsql:Enum:clan_member_role", "member,officer,leader")
                .Annotation("Npgsql:Enum:culture", "neutral,aserai,battania,empire,khuzait,looters,sturgia,vlandia")
                .Annotation("Npgsql:Enum:damage_type", "undefined,cut,pierce,blunt")
                .Annotation("Npgsql:Enum:game_mode", "crpg_battle,crpg_conquest,crpgdtv,crpg_duel,crpg_siege,crpg_team_deathmatch,crpg_skirmish,crpg_unknown_game_mode")
                .Annotation("Npgsql:Enum:item_slot", "head,shoulder,body,hand,leg,mount_harness,mount,weapon0,weapon1,weapon2,weapon3,weapon_extra")
                .Annotation("Npgsql:Enum:item_type", "undefined,head_armor,shoulder_armor,body_armor,hand_armor,leg_armor,mount_harness,mount,shield,bow,crossbow,one_handed_weapon,two_handed_weapon,polearm,thrown,arrows,bolts,pistol,musket,bullets,banner")
                .Annotation("Npgsql:Enum:languages", "en,zh,ru,de,fr,it,es,pl,uk,ro,nl,tr,el,hu,sv,cs,pt,sr,bg,hr,da,fi,no,be,lv")
                .Annotation("Npgsql:Enum:party_status", "idle,idle_in_settlement,recruiting_in_settlement,moving_to_point,following_party,moving_to_settlement,moving_to_attack_party,moving_to_attack_settlement,in_battle")
                .Annotation("Npgsql:Enum:platform", "steam,epic_games,microsoft")
                .Annotation("Npgsql:Enum:region", "eu,na,as,oc")
                .Annotation("Npgsql:Enum:restriction_type", "all,join,chat")
                .Annotation("Npgsql:Enum:role", "user,moderator,game_admin,admin")
                .Annotation("Npgsql:Enum:settlement_type", "village,castle,town")
                .Annotation("Npgsql:Enum:weapon_class", "undefined,dagger,one_handed_sword,two_handed_sword,one_handed_axe,two_handed_axe,mace,pick,two_handed_mace,one_handed_polearm,two_handed_polearm,low_grip_polearm,arrow,bolt,cartridge,bow,crossbow,stone,boulder,throwing_axe,throwing_knife,javelin,pistol,musket,small_shield,large_shield,banner")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:Enum:activity_log_type", "user_created,user_deleted,user_renamed,user_rewarded,item_bought,item_sold,item_broke,item_reforged,item_repaired,item_upgraded,character_created,character_deleted,character_rating_reset,character_respecialized,character_retired,character_rewarded,character_earned,server_joined,chat_message_sent,team_hit,clan_armory_add_item,clan_armory_remove_item,clan_armory_return_item,clan_armory_borrow_item")
                .OldAnnotation("Npgsql:Enum:battle_fighter_application_status", "pending,declined,accepted")
                .OldAnnotation("Npgsql:Enum:battle_mercenary_application_status", "pending,declined,accepted")
                .OldAnnotation("Npgsql:Enum:battle_phase", "preparation,hiring,scheduled,live,end")
                .OldAnnotation("Npgsql:Enum:battle_side", "attacker,defender")
                .OldAnnotation("Npgsql:Enum:character_class", "peasant,infantry,shock_infantry,skirmisher,crossbowman,archer,cavalry,mounted_archer")
                .OldAnnotation("Npgsql:Enum:clan_invitation_status", "pending,declined,accepted")
                .OldAnnotation("Npgsql:Enum:clan_invitation_type", "request,offer")
                .OldAnnotation("Npgsql:Enum:clan_member_role", "member,officer,leader")
                .OldAnnotation("Npgsql:Enum:culture", "neutral,aserai,battania,empire,khuzait,looters,sturgia,vlandia")
                .OldAnnotation("Npgsql:Enum:damage_type", "undefined,cut,pierce,blunt")
                .OldAnnotation("Npgsql:Enum:item_slot", "head,shoulder,body,hand,leg,mount_harness,mount,weapon0,weapon1,weapon2,weapon3,weapon_extra")
                .OldAnnotation("Npgsql:Enum:item_type", "undefined,head_armor,shoulder_armor,body_armor,hand_armor,leg_armor,mount_harness,mount,shield,bow,crossbow,one_handed_weapon,two_handed_weapon,polearm,thrown,arrows,bolts,pistol,musket,bullets,banner")
                .OldAnnotation("Npgsql:Enum:languages", "en,zh,ru,de,fr,it,es,pl,uk,ro,nl,tr,el,hu,sv,cs,pt,sr,bg,hr,da,fi,no,be,lv")
                .OldAnnotation("Npgsql:Enum:party_status", "idle,idle_in_settlement,recruiting_in_settlement,moving_to_point,following_party,moving_to_settlement,moving_to_attack_party,moving_to_attack_settlement,in_battle")
                .OldAnnotation("Npgsql:Enum:platform", "steam,epic_games,microsoft")
                .OldAnnotation("Npgsql:Enum:region", "eu,na,as,oc")
                .OldAnnotation("Npgsql:Enum:restriction_type", "all,join,chat")
                .OldAnnotation("Npgsql:Enum:role", "user,moderator,game_admin,admin")
                .OldAnnotation("Npgsql:Enum:settlement_type", "village,castle,town")
                .OldAnnotation("Npgsql:Enum:weapon_class", "undefined,dagger,one_handed_sword,two_handed_sword,one_handed_axe,two_handed_axe,mace,pick,two_handed_mace,one_handed_polearm,two_handed_polearm,low_grip_polearm,arrow,bolt,cartridge,bow,crossbow,stone,boulder,throwing_axe,throwing_knife,javelin,pistol,musket,small_shield,large_shield,banner")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");

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
                    game_mode = table.Column<GameMode>(type: "game_mode", nullable: false)
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
                .Annotation("Npgsql:Enum:activity_log_type", "user_created,user_deleted,user_renamed,user_rewarded,item_bought,item_sold,item_broke,item_reforged,item_repaired,item_upgraded,character_created,character_deleted,character_rating_reset,character_respecialized,character_retired,character_rewarded,character_earned,server_joined,chat_message_sent,team_hit,clan_armory_add_item,clan_armory_remove_item,clan_armory_return_item,clan_armory_borrow_item")
                .Annotation("Npgsql:Enum:battle_fighter_application_status", "pending,declined,accepted")
                .Annotation("Npgsql:Enum:battle_mercenary_application_status", "pending,declined,accepted")
                .Annotation("Npgsql:Enum:battle_phase", "preparation,hiring,scheduled,live,end")
                .Annotation("Npgsql:Enum:battle_side", "attacker,defender")
                .Annotation("Npgsql:Enum:character_class", "peasant,infantry,shock_infantry,skirmisher,crossbowman,archer,cavalry,mounted_archer")
                .Annotation("Npgsql:Enum:clan_invitation_status", "pending,declined,accepted")
                .Annotation("Npgsql:Enum:clan_invitation_type", "request,offer")
                .Annotation("Npgsql:Enum:clan_member_role", "member,officer,leader")
                .Annotation("Npgsql:Enum:culture", "neutral,aserai,battania,empire,khuzait,looters,sturgia,vlandia")
                .Annotation("Npgsql:Enum:damage_type", "undefined,cut,pierce,blunt")
                .Annotation("Npgsql:Enum:item_slot", "head,shoulder,body,hand,leg,mount_harness,mount,weapon0,weapon1,weapon2,weapon3,weapon_extra")
                .Annotation("Npgsql:Enum:item_type", "undefined,head_armor,shoulder_armor,body_armor,hand_armor,leg_armor,mount_harness,mount,shield,bow,crossbow,one_handed_weapon,two_handed_weapon,polearm,thrown,arrows,bolts,pistol,musket,bullets,banner")
                .Annotation("Npgsql:Enum:languages", "en,zh,ru,de,fr,it,es,pl,uk,ro,nl,tr,el,hu,sv,cs,pt,sr,bg,hr,da,fi,no,be,lv")
                .Annotation("Npgsql:Enum:party_status", "idle,idle_in_settlement,recruiting_in_settlement,moving_to_point,following_party,moving_to_settlement,moving_to_attack_party,moving_to_attack_settlement,in_battle")
                .Annotation("Npgsql:Enum:platform", "steam,epic_games,microsoft")
                .Annotation("Npgsql:Enum:region", "eu,na,as,oc")
                .Annotation("Npgsql:Enum:restriction_type", "all,join,chat")
                .Annotation("Npgsql:Enum:role", "user,moderator,game_admin,admin")
                .Annotation("Npgsql:Enum:settlement_type", "village,castle,town")
                .Annotation("Npgsql:Enum:weapon_class", "undefined,dagger,one_handed_sword,two_handed_sword,one_handed_axe,two_handed_axe,mace,pick,two_handed_mace,one_handed_polearm,two_handed_polearm,low_grip_polearm,arrow,bolt,cartridge,bow,crossbow,stone,boulder,throwing_axe,throwing_knife,javelin,pistol,musket,small_shield,large_shield,banner")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:Enum:activity_log_type", "user_created,user_deleted,user_renamed,user_rewarded,item_bought,item_sold,item_broke,item_reforged,item_repaired,item_upgraded,character_created,character_deleted,character_rating_reset,character_respecialized,character_retired,character_rewarded,character_earned,server_joined,chat_message_sent,team_hit,clan_armory_add_item,clan_armory_remove_item,clan_armory_return_item,clan_armory_borrow_item")
                .OldAnnotation("Npgsql:Enum:battle_fighter_application_status", "pending,declined,accepted")
                .OldAnnotation("Npgsql:Enum:battle_mercenary_application_status", "pending,declined,accepted")
                .OldAnnotation("Npgsql:Enum:battle_phase", "preparation,hiring,scheduled,live,end")
                .OldAnnotation("Npgsql:Enum:battle_side", "attacker,defender")
                .OldAnnotation("Npgsql:Enum:character_class", "peasant,infantry,shock_infantry,skirmisher,crossbowman,archer,cavalry,mounted_archer")
                .OldAnnotation("Npgsql:Enum:clan_invitation_status", "pending,declined,accepted")
                .OldAnnotation("Npgsql:Enum:clan_invitation_type", "request,offer")
                .OldAnnotation("Npgsql:Enum:clan_member_role", "member,officer,leader")
                .OldAnnotation("Npgsql:Enum:culture", "neutral,aserai,battania,empire,khuzait,looters,sturgia,vlandia")
                .OldAnnotation("Npgsql:Enum:damage_type", "undefined,cut,pierce,blunt")
                .OldAnnotation("Npgsql:Enum:game_mode", "crpg_battle,crpg_conquest,crpgdtv,crpg_duel,crpg_siege,crpg_team_deathmatch,crpg_skirmish,crpg_unknown_game_mode")
                .OldAnnotation("Npgsql:Enum:item_slot", "head,shoulder,body,hand,leg,mount_harness,mount,weapon0,weapon1,weapon2,weapon3,weapon_extra")
                .OldAnnotation("Npgsql:Enum:item_type", "undefined,head_armor,shoulder_armor,body_armor,hand_armor,leg_armor,mount_harness,mount,shield,bow,crossbow,one_handed_weapon,two_handed_weapon,polearm,thrown,arrows,bolts,pistol,musket,bullets,banner")
                .OldAnnotation("Npgsql:Enum:languages", "en,zh,ru,de,fr,it,es,pl,uk,ro,nl,tr,el,hu,sv,cs,pt,sr,bg,hr,da,fi,no,be,lv")
                .OldAnnotation("Npgsql:Enum:party_status", "idle,idle_in_settlement,recruiting_in_settlement,moving_to_point,following_party,moving_to_settlement,moving_to_attack_party,moving_to_attack_settlement,in_battle")
                .OldAnnotation("Npgsql:Enum:platform", "steam,epic_games,microsoft")
                .OldAnnotation("Npgsql:Enum:region", "eu,na,as,oc")
                .OldAnnotation("Npgsql:Enum:restriction_type", "all,join,chat")
                .OldAnnotation("Npgsql:Enum:role", "user,moderator,game_admin,admin")
                .OldAnnotation("Npgsql:Enum:settlement_type", "village,castle,town")
                .OldAnnotation("Npgsql:Enum:weapon_class", "undefined,dagger,one_handed_sword,two_handed_sword,one_handed_axe,two_handed_axe,mace,pick,two_handed_mace,one_handed_polearm,two_handed_polearm,low_grip_polearm,arrow,bolt,cartridge,bow,crossbow,stone,boulder,throwing_axe,throwing_knife,javelin,pistol,musket,small_shield,large_shield,banner")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");

        }
    }
}
