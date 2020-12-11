using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Retro_Gamer.Migrations
{
    public partial class SeedGames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DbGame",
                columns: new[] { "Id", "Description", "Genres", "Name", "Rating", "RelaseDate" },
                values: new object[] { 3, "Super Mario game inc", 4, "Super Mario", 10, new DateTime(2020, 7, 23, 16, 40, 38, 713, DateTimeKind.Local).AddTicks(2546) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DbGame",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
