using Microsoft.EntityFrameworkCore.Migrations;

namespace MuaVatTu.Data.Migrations
{
    public partial class AddColUnacenAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UnaccentTitle",
                table: "mvt_New",
                nullable: true);
            migrationBuilder.Sql(
          @"CREATE TRIGGER ADD_search_vector_update BEFORE INSERT OR UPDATE
              ON ""mvt_New"" FOR EACH ROW EXECUTE PROCEDURE
              tsvector_update_trigger(""SearchVector"", 'pg_catalog.english', ""Title"", ""Content"",""UnaccentTitle"");");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnaccentTitle",
                table: "mvt_New");
            migrationBuilder.Sql("DROP TRIGGER ADD_search_vector_update");
        }
    }
}
