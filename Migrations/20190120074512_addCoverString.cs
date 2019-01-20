using Microsoft.EntityFrameworkCore.Migrations;

namespace Biblioteka1.Migrations
{
    public partial class addCoverString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverString",
                table: "LibraryItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverString",
                table: "LibraryItems");
        }
    }
}
