using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace MuaVatTu.Data.Migrations
{
    public partial class Del_Col_New : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SearchVector",
                table: "mvt_New");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "mvt_New",
                type: "tsvector",
                nullable: true);
        }
    }
}
