using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crpg.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRestrictionPublicReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "public_reason",
                table: "restrictions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "public_reason",
                table: "restrictions");
        }
    }
}
