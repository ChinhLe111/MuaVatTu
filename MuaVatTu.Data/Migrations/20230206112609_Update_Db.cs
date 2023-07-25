using Microsoft.EntityFrameworkCore.Migrations;

namespace MuaVatTu.Data.Migrations
{
    public partial class Update_Db : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mvt_TongSap_mvt_BoPhan_BoPhanId",
                table: "mvt_TongSap");

            migrationBuilder.DropIndex(
                name: "IX_mvt_TongSap_BoPhanId",
                table: "mvt_TongSap");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_mvt_TongSap_BoPhanId",
                table: "mvt_TongSap",
                column: "BoPhanId");

            migrationBuilder.AddForeignKey(
                name: "FK_mvt_TongSap_mvt_BoPhan_BoPhanId",
                table: "mvt_TongSap",
                column: "BoPhanId",
                principalTable: "mvt_BoPhan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
