using Microsoft.EntityFrameworkCore.Migrations;

namespace Biblioteka1.Migrations
{
    public partial class Status2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CheckedOutUser",
                table: "LibraryItems",
                newName: "CheckedOutTo");

            migrationBuilder.AddColumn<string>(
                name: "CheckedOutBy",
                table: "LibraryItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckedOutBy",
                table: "LibraryItems");

            migrationBuilder.RenameColumn(
                name: "CheckedOutTo",
                table: "LibraryItems",
                newName: "CheckedOutUser");
        }
    }
}
