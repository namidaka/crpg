using Microsoft.EntityFrameworkCore.Migrations;

namespace Crpg.Persistence.Migrations;

/// <inheritdoc />
public partial class AddClanLanguages : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:Enum:languages", "en,zh,ru,de,fr,it,es,pl,uk,ro,nl,tr,el,hu,sv,cs,pt,sr,bg,hr,da,fi,no,be,lv");

        migrationBuilder.AddColumn<string>(
            name: "languages",
            table: "clans",
            type: "text",
            nullable: false,
            defaultValue: string.Empty);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "languages",
            table: "clans");

        migrationBuilder.AlterDatabase()
            .OldAnnotation("Npgsql:Enum:languages", "en,zh,ru,de,fr,it,es,pl,uk,ro,nl,tr,el,hu,sv,cs,pt,sr,bg,hr,da,fi,no,be,lv");
    }
}
