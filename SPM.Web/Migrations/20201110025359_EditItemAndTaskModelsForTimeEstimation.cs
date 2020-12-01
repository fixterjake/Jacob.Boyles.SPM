using Microsoft.EntityFrameworkCore.Migrations;

namespace SPM.Web.Migrations
{
    public partial class EditItemAndTaskModelsForTimeEstimation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "Items");

            migrationBuilder.AddColumn<int>(
                name: "Time",
                table: "Tasks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Estimated",
                table: "Items",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Estimated",
                table: "Items");

            migrationBuilder.AddColumn<int>(
                name: "Time",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
