using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crpg.Persistence.Migrations;

/// <inheritdoc />
public partial class DisableUniqueUserItem : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_user_items_user_id_item_id",
            table: "user_items");

        migrationBuilder.CreateIndex(
            name: "ix_user_items_user_id_item_id",
            table: "user_items",
            columns: new[] { "user_id", "item_id" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "ix_user_items_user_id_item_id",
            table: "user_items");

        migrationBuilder.CreateIndex(
            name: "ix_user_items_user_id_item_id",
            table: "user_items",
            columns: new[] { "user_id", "item_id" },
            unique: true);
    }
}
