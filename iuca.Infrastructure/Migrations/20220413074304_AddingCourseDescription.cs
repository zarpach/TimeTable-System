using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingCourseDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescEng",
                table: "RegistrationCourses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescKir",
                table: "RegistrationCourses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescRus",
                table: "RegistrationCourses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Requirements",
                table: "RegistrationCourses",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescEng",
                table: "RegistrationCourses");

            migrationBuilder.DropColumn(
                name: "DescKir",
                table: "RegistrationCourses");

            migrationBuilder.DropColumn(
                name: "DescRus",
                table: "RegistrationCourses");

            migrationBuilder.DropColumn(
                name: "Requirements",
                table: "RegistrationCourses");
        }
    }
}
