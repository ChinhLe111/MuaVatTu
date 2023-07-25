using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace MuaVatTu.Data.Migrations
{
    public partial class AddNewDBFinay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mvt_New",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    UnaccentTitle = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    CreatedOnDate = table.Column<DateTime>(nullable: false),
                    SearchVector = table.Column<NpgsqlTsVector>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mvt_New", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mvt_New_SearchVector",
                table: "mvt_New",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIST");
            migrationBuilder.Sql(
           @"CREATE TRIGGER New_search_vector_update BEFORE INSERT OR UPDATE
              ON ""mvt_New"" FOR EACH ROW EXECUTE PROCEDURE
              tsvector_update_trigger(""SearchVector"", 'pg_catalog.english', ""Title"", ""Content"",""UnaccentTitle"");");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mvt_New");
            migrationBuilder.Sql("DROP TRIGGER New_search_vector_update");
        }
    }
}
