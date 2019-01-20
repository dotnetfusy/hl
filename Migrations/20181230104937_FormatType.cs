using Microsoft.EntityFrameworkCore.Migrations;

namespace Biblioteka1.Migrations
{
    public partial class FormatType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Format",
                table: "LibraryItems",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Format",
                table: "LibraryItems",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
