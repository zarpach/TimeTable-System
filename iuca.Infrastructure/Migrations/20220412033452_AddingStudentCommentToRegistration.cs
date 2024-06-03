using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingStudentCommentToRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "StudentCourseRegistrations",
                newName: "StudentComment");

            migrationBuilder.AddColumn<string>(
                name: "AdviserComment",
                table: "StudentCourseRegistrations",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdviserComment",
                table: "StudentCourseRegistrations");

            migrationBuilder.RenameColumn(
                name: "StudentComment",
                table: "StudentCourseRegistrations",
                newName: "Comment");
        }
    }
}
