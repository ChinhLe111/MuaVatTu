using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace MuaVatTu.Data.Migrations
{
    public partial class RefeshAllDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_mvt_New_SearchVector",
                table: "mvt_New");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                table: "mvt_New");

            migrationBuilder.DropColumn(
                name: "UnaccentTitle",
                table: "mvt_New");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "mvt_New",
                type: "tsvector",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnaccentTitle",
                table: "mvt_New",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_mvt_New_SearchVector",
                table: "mvt_New",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIST");
        }
    }
}
