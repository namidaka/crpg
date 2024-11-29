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
        migrationBuilder.Sql(@"
            ALTER TABLE idempotency_keys 
            ALTER COLUMN key TYPE uuid 
            USING key::uuid;
        ");

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

        migrationBuilder.Sql(@"
            ALTER TABLE idempotency_keys 
            ALTER COLUMN key TYPE text 
            USING key::text;
        ");
    }
}
