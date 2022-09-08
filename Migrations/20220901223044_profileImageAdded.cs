using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rpg.Migrations
{
    public partial class profileImageAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "profilePicturePath",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "profilePicturePath",
                table: "Users");
        }
    }
}
