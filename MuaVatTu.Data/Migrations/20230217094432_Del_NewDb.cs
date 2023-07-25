using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace MuaVatTu.Data.Migrations
{
    public partial class Del_NewDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mvt_New");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mvt_New",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SearchVector = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: true),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    UnaccentTitle = table.Column<string>(type: "text", nullable: true)
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
        }
    }
}
