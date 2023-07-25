using Microsoft.EntityFrameworkCore.Migrations;

namespace MuaVatTu.Data.Migrations
{
    public partial class AddCol_TitleUnacennt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UnaccentTitle",
                table: "mvt_New",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnaccentTitle",
                table: "mvt_New");
        }
    }
}
