using Microsoft.EntityFrameworkCore.Migrations;

namespace Retro_Gamer.Migrations
{
    public partial class Memory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Memorie",
                table: "DbMemories");

            migrationBuilder.AddColumn<string>(
                name: "Memory",
                table: "DbMemories",
                maxLength: 600,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Memory",
                table: "DbMemories");

            migrationBuilder.AddColumn<string>(
                name: "Memorie",
                table: "DbMemories",
                type: "nvarchar(600)",
                maxLength: 600,
                nullable: false,
                defaultValue: "");
        }
    }
}
