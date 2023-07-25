using Microsoft.EntityFrameworkCore.Migrations;

namespace MuaVatTu.Data.Migrations
{
    public partial class UpdateDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DangKy_BoPhan_BoPhanId",
                table: "DangKy");

            migrationBuilder.DropForeignKey(
                name: "FK_MatHang_DangKy_DangKyId",
                table: "MatHang");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MatHang",
                table: "MatHang");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DangKy",
                table: "DangKy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BoPhan",
                table: "BoPhan");

            migrationBuilder.RenameTable(
                name: "MatHang",
                newName: "mvt_MatHang");

            migrationBuilder.RenameTable(
                name: "DangKy",
                newName: "mvt_DangKy");

            migrationBuilder.RenameTable(
                name: "BoPhan",
                newName: "mvt_BoPhan");

            migrationBuilder.RenameIndex(
                name: "IX_MatHang_DangKyId",
                table: "mvt_MatHang",
                newName: "IX_mvt_MatHang_DangKyId");

            migrationBuilder.RenameIndex(
                name: "IX_DangKy_BoPhanId",
                table: "mvt_DangKy",
                newName: "IX_mvt_DangKy_BoPhanId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_mvt_MatHang",
                table: "mvt_MatHang",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_mvt_DangKy",
                table: "mvt_DangKy",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_mvt_BoPhan",
                table: "mvt_BoPhan",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_mvt_DangKy_mvt_BoPhan_BoPhanId",
                table: "mvt_DangKy",
                column: "BoPhanId",
                principalTable: "mvt_BoPhan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mvt_MatHang_mvt_DangKy_DangKyId",
                table: "mvt_MatHang",
                column: "DangKyId",
                principalTable: "mvt_DangKy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mvt_DangKy_mvt_BoPhan_BoPhanId",
                table: "mvt_DangKy");

            migrationBuilder.DropForeignKey(
                name: "FK_mvt_MatHang_mvt_DangKy_DangKyId",
                table: "mvt_MatHang");

            migrationBuilder.DropPrimaryKey(
                name: "PK_mvt_MatHang",
                table: "mvt_MatHang");

            migrationBuilder.DropPrimaryKey(
                name: "PK_mvt_DangKy",
                table: "mvt_DangKy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_mvt_BoPhan",
                table: "mvt_BoPhan");

            migrationBuilder.RenameTable(
                name: "mvt_MatHang",
                newName: "MatHang");

            migrationBuilder.RenameTable(
                name: "mvt_DangKy",
                newName: "DangKy");

            migrationBuilder.RenameTable(
                name: "mvt_BoPhan",
                newName: "BoPhan");

            migrationBuilder.RenameIndex(
                name: "IX_mvt_MatHang_DangKyId",
                table: "MatHang",
                newName: "IX_MatHang_DangKyId");

            migrationBuilder.RenameIndex(
                name: "IX_mvt_DangKy_BoPhanId",
                table: "DangKy",
                newName: "IX_DangKy_BoPhanId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatHang",
                table: "MatHang",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DangKy",
                table: "DangKy",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BoPhan",
                table: "BoPhan",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DangKy_BoPhan_BoPhanId",
                table: "DangKy",
                column: "BoPhanId",
                principalTable: "BoPhan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MatHang_DangKy_DangKyId",
                table: "MatHang",
                column: "DangKyId",
                principalTable: "DangKy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
