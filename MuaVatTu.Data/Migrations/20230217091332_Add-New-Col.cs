using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace MuaVatTu.Data.Migrations
{
    public partial class AddNewCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_mvt_New_SearchVector",
                table: "mvt_New");

            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "FullVector",
                table: "mvt_New",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_mvt_New_FullVector",
                table: "mvt_New",
                column: "FullVector")
                .Annotation("Npgsql:IndexMethod", "GIST");
            migrationBuilder.Sql(
     @"CREATE TRIGGER product_search_vector_update BEFORE INSERT OR UPDATE
              ON ""mvt_New"" FOR EACH ROW EXECUTE PROCEDURE
              tsvector_update_trigger(""FullVector"", 'pg_catalog.english', ""Title"", ""Content"", ""UnaccentTitle"");");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_mvt_New_FullVector",
                table: "mvt_New");

            migrationBuilder.DropColumn(
                name: "FullVector",
                table: "mvt_New");

            migrationBuilder.CreateIndex(
                name: "IX_mvt_New_SearchVector",
                table: "mvt_New",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIST");
            migrationBuilder.Sql("DROP TRIGGER product_search_vector_update");
        }
    }
}
