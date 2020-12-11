using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Retro_Gamer.Migrations
{
    public partial class PhotoUrlPath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "DbGame",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "DbGame",
                keyColumn: "Id",
                keyValue: 3,
                column: "RelaseDate",
                value: new DateTime(2020, 7, 23, 17, 20, 43, 420, DateTimeKind.Local).AddTicks(6619));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "DbGame");

            migrationBuilder.UpdateData(
                table: "DbGame",
                keyColumn: "Id",
                keyValue: 3,
                column: "RelaseDate",
                value: new DateTime(2020, 7, 23, 16, 40, 38, 713, DateTimeKind.Local).AddTicks(2546));
        }
    }
}
