using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crpg.Persistence.Migrations;

/// <inheritdoc />
public partial class CharacterGoldExpEarnStats : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:Enum:activity_log_type", "user_created,user_deleted,user_renamed,user_rewarded,item_bought,item_sold,item_broke,item_reforged,item_repaired,item_upgraded,character_created,character_deleted,character_rating_reset,character_respecialized,character_retired,character_rewarded,character_earned,server_joined,chat_message_sent,team_hit,clan_armory_add_item,clan_armory_remove_item,clan_armory_return_item,clan_armory_borrow_item")
            .OldAnnotation("Npgsql:Enum:activity_log_type", "user_created,user_deleted,user_renamed,user_rewarded,item_bought,item_sold,item_broke,item_reforged,item_repaired,item_upgraded,character_created,character_deleted,character_rating_reset,character_respecialized,character_retired,character_rewarded,server_joined,chat_message_sent,team_hit,clan_armory_add_item,clan_armory_remove_item,clan_armory_return_item,clan_armory_borrow_item");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:Enum:activity_log_type", "user_created,user_deleted,user_renamed,user_rewarded,item_bought,item_sold,item_broke,item_reforged,item_repaired,item_upgraded,character_created,character_deleted,character_rating_reset,character_respecialized,character_retired,character_rewarded,server_joined,chat_message_sent,team_hit,clan_armory_add_item,clan_armory_remove_item,clan_armory_return_item,clan_armory_borrow_item")
            .OldAnnotation("Npgsql:Enum:activity_log_type", "user_created,user_deleted,user_renamed,user_rewarded,item_bought,item_sold,item_broke,item_reforged,item_repaired,item_upgraded,character_created,character_deleted,character_rating_reset,character_respecialized,character_retired,character_rewarded,character_earned,server_joined,chat_message_sent,team_hit,clan_armory_add_item,clan_armory_remove_item,clan_armory_return_item,clan_armory_borrow_item");
    }
}
