using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crpg.Persistence.Migrations;

/// <inheritdoc />
public partial class IdempotencyKeyStringToGuid : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<Guid>(
            name: "key",
            table: "idempotency_keys",
            type: "uuid",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.AddColumn<DateTime>(
            name: "updated_at",
            table: "idempotency_keys",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "updated_at",
            table: "idempotency_keys");

        migrationBuilder.AlterColumn<string>(
            name: "key",
            table: "idempotency_keys",
            type: "text",
            nullable: false,
            oldClrType: typeof(Guid),
            oldType: "uuid");
    }
}
