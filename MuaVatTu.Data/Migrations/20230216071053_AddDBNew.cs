using Microsoft.EntityFrameworkCore.Migrations;

namespace MuaVatTu.Data.Migrations
{
    public partial class AddDBNew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_mvt_New_SearchVector",
                table: "mvt_New",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
            migrationBuilder.Sql(
           @"CREATE TRIGGER product_search_vector_update BEFORE INSERT OR UPDATE
              ON ""mvt_New"" FOR EACH ROW EXECUTE PROCEDURE
              tsvector_update_trigger(""SearchVector"", 'mvt_New', ""Title"", ""Slug"",""Content"");");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_mvt_New_SearchVector",
                table: "mvt_New");
            migrationBuilder.Sql("DROP TRIGGER product_search_vector_update");
        }
    }
}
