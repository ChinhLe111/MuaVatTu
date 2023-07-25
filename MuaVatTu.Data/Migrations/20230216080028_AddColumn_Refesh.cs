using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace MuaVatTu.Data.Migrations
{
    public partial class AddColumn_Refesh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "mvt_New",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_mvt_New_SearchVector",
                table: "mvt_New",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIST");
            migrationBuilder.Sql(
           @"CREATE TRIGGER new_search_vector_update BEFORE INSERT OR UPDATE
              ON ""mvt_New"" FOR EACH ROW EXECUTE PROCEDURE
              tsvector_update_trigger(""SearchVector"", 'pg_catalog.english', ""Title"", ""Content"");");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_mvt_New_SearchVector",
                table: "mvt_New");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                table: "mvt_New");
            migrationBuilder.Sql("DROP TRIGGER new_search_vector_update");
        }
    }
}
