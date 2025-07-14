using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelFacilitator.Migrations
{
    /// <inheritdoc />
    public partial class SourceFileAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceFile",
                table: "ExcelRecords",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceFile",
                table: "ExcelRecords");
        }
    }
}
