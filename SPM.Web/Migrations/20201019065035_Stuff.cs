using Microsoft.EntityFrameworkCore.Migrations;

namespace SPM.Web.Migrations
{
    public partial class Stuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Items");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Items",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Items",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Items");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Items",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Items",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
