using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Retro_Gamer.Migrations
{
    public partial class ExtendIdentityUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "DbGame",
                keyColumn: "Id",
                keyValue: 3,
                column: "RelaseDate",
                value: new DateTime(2020, 8, 18, 18, 6, 7, 206, DateTimeKind.Local).AddTicks(4810));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "DbGame",
                keyColumn: "Id",
                keyValue: 3,
                column: "RelaseDate",
                value: new DateTime(2020, 8, 5, 15, 0, 41, 618, DateTimeKind.Local).AddTicks(3544));
        }
    }
}
