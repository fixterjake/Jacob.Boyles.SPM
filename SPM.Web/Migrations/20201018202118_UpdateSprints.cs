using Microsoft.EntityFrameworkCore.Migrations;

namespace SPM.Web.Migrations
{
    public partial class UpdateSprints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Sprints");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Sprints",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Sprints",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Sprints",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Sprints");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Sprints");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Sprints",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Sprints",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
