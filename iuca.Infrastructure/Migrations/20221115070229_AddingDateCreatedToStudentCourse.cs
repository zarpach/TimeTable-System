using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingDateCreatedToStudentCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Queue",
                table: "StudentCoursesTemp");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "StudentCoursesTemp",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "StudentCoursesTemp");

            migrationBuilder.AddColumn<int>(
                name: "Queue",
                table: "StudentCoursesTemp",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
