using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddDropFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAddDropApproved",
                table: "StudentCoursesTemp",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAddDropProcessed",
                table: "StudentCoursesTemp",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "StudentCoursesTemp",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AddDropState",
                table: "StudentCourseRegistrations",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<bool>(
                name: "IsAddDropApproved",
                table: "StudentCourseRegistrations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAddDropApproved",
                table: "StudentCoursesTemp");

            migrationBuilder.DropColumn(
                name: "IsAddDropProcessed",
                table: "StudentCoursesTemp");

            migrationBuilder.DropColumn(
                name: "State",
                table: "StudentCoursesTemp");

            migrationBuilder.DropColumn(
                name: "AddDropState",
                table: "StudentCourseRegistrations");

            migrationBuilder.DropColumn(
                name: "IsAddDropApproved",
                table: "StudentCourseRegistrations");
        }
    }
}
