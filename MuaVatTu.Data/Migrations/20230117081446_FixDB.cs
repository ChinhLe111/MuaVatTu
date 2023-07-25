using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MuaVatTu.Data.Migrations
{
    public partial class FixDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "mvt_BoPhan");

            migrationBuilder.AlterColumn<string>(
                name: "NameOfLeader",
                table: "mvt_BoPhan",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "mvt_NhanVien",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    BoPhanId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mvt_NhanVien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mvt_NhanVien_mvt_BoPhan_BoPhanId",
                        column: x => x.BoPhanId,
                        principalTable: "mvt_BoPhan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mvt_NhanVien_BoPhanId",
                table: "mvt_NhanVien",
                column: "BoPhanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mvt_NhanVien");

            migrationBuilder.AlterColumn<string>(
                name: "NameOfLeader",
                table: "mvt_BoPhan",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "mvt_BoPhan",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
