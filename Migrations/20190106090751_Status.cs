using Microsoft.EntityFrameworkCore.Migrations;

namespace Biblioteka1.Migrations
{
    public partial class Status : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CheckedOutDate",
                table: "LibraryItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckedOutUser",
                table: "LibraryItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckedOutDate",
                table: "LibraryItems");

            migrationBuilder.DropColumn(
                name: "CheckedOutUser",
                table: "LibraryItems");
        }
    }
}
