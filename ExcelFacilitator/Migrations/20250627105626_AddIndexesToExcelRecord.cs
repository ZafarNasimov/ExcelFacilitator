using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFacilitator.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesToExcelRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ExcelRecords_BuyerTin",
                table: "ExcelRecords",
                column: "BuyerTin");

            migrationBuilder.CreateIndex(
                name: "IX_ExcelRecords_ContractDate",
                table: "ExcelRecords",
                column: "ContractDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExcelRecords_DocumentDate",
                table: "ExcelRecords",
                column: "DocumentDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExcelRecords_ItemCatalogCode",
                table: "ExcelRecords",
                column: "ItemCatalogCode");

            migrationBuilder.CreateIndex(
                name: "IX_ExcelRecords_Price",
                table: "ExcelRecords",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_ExcelRecords_PriceWithVat",
                table: "ExcelRecords",
                column: "PriceWithVat");

            migrationBuilder.CreateIndex(
                name: "IX_ExcelRecords_Quantity",
                table: "ExcelRecords",
                column: "Quantity");

            migrationBuilder.CreateIndex(
                name: "IX_ExcelRecords_SellerTin",
                table: "ExcelRecords",
                column: "SellerTin");

            migrationBuilder.CreateIndex(
                name: "IX_ExcelRecords_SourceFile",
                table: "ExcelRecords",
                column: "SourceFile");

            migrationBuilder.CreateIndex(
                name: "IX_ExcelRecords_TotalWithVat",
                table: "ExcelRecords",
                column: "TotalWithVat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExcelRecords_BuyerTin",
                table: "ExcelRecords");

            migrationBuilder.DropIndex(
                name: "IX_ExcelRecords_ContractDate",
                table: "ExcelRecords");

            migrationBuilder.DropIndex(
                name: "IX_ExcelRecords_DocumentDate",
                table: "ExcelRecords");

            migrationBuilder.DropIndex(
                name: "IX_ExcelRecords_ItemCatalogCode",
                table: "ExcelRecords");

            migrationBuilder.DropIndex(
                name: "IX_ExcelRecords_Price",
                table: "ExcelRecords");

            migrationBuilder.DropIndex(
                name: "IX_ExcelRecords_PriceWithVat",
                table: "ExcelRecords");

            migrationBuilder.DropIndex(
                name: "IX_ExcelRecords_Quantity",
                table: "ExcelRecords");

            migrationBuilder.DropIndex(
                name: "IX_ExcelRecords_SellerTin",
                table: "ExcelRecords");

            migrationBuilder.DropIndex(
                name: "IX_ExcelRecords_SourceFile",
                table: "ExcelRecords");

            migrationBuilder.DropIndex(
                name: "IX_ExcelRecords_TotalWithVat",
                table: "ExcelRecords");
        }
    }
}
