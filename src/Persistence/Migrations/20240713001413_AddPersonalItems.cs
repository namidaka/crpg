using Microsoft.EntityFrameworkCore.Migrations;

namespace Crpg.Persistence.Migrations;

public partial class AddPersonalItems : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_personal_items_items_item_id",
            table: "personal_items");

        migrationBuilder.DropForeignKey(
            name: "fk_personal_items_users_user_id",
            table: "personal_items");

        migrationBuilder.DropPrimaryKey(
            name: "pk_personal_items",
            table: "personal_items");

        migrationBuilder.DropIndex(
            name: "ix_personal_items_item_id",
            table: "personal_items");

        migrationBuilder.DropIndex(
            name: "ix_personal_items_user_id_item_id",
            table: "personal_items");

        migrationBuilder.DropColumn(
            name: "user_id",
            table: "personal_items");

        migrationBuilder.DropColumn(
            name: "item_id",
            table: "personal_items");

        migrationBuilder.RenameColumn(
            name: "id",
            table: "personal_items",
            newName: "user_item_id");

        migrationBuilder.AddPrimaryKey(
            name: "pk_personal_items",
            table: "personal_items",
            column: "user_item_id");

        migrationBuilder.AddForeignKey(
            name: "fk_personal_items_user_items_user_item_id",
            table: "personal_items",
            column: "user_item_id",
            principalTable: "user_items",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_personal_items_user_items_user_item_id",
            table: "personal_items");

        migrationBuilder.DropPrimaryKey(
            name: "pk_personal_items",
            table: "personal_items");

        migrationBuilder.RenameColumn(
            name: "user_item_id",
            table: "personal_items",
            newName: "id");

        migrationBuilder.AddColumn<int>(
            name: "user_id",
            table: "personal_items",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "item_id",
            table: "personal_items",
            type: "text",
            nullable: false,
            defaultValue: string.Empty);

        migrationBuilder.AddPrimaryKey(
            name: "pk_personal_items",
            table: "personal_items",
            columns: new[] { "user_id", "item_id" });

        migrationBuilder.CreateIndex(
            name: "ix_personal_items_item_id",
            table: "personal_items",
            column: "item_id");

        migrationBuilder.CreateIndex(
            name: "ix_personal_items_user_id_item_id",
            table: "personal_items",
            columns: new[] { "user_id", "item_id" },
            unique: true);

        migrationBuilder.AddForeignKey(
            name: "fk_personal_items_items_item_id",
            table: "personal_items",
            column: "item_id",
            principalTable: "items",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "fk_personal_items_users_user_id",
            table: "personal_items",
            column: "user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
