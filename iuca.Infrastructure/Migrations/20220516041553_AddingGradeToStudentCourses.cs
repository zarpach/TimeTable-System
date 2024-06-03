using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingGradeToStudentCourses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GradeId",
                table: "StudentCoursesTemp",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentCoursesTemp_GradeId",
                table: "StudentCoursesTemp",
                column: "GradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCoursesTemp_Grades_GradeId",
                table: "StudentCoursesTemp",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentCoursesTemp_Grades_GradeId",
                table: "StudentCoursesTemp");

            migrationBuilder.DropIndex(
                name: "IX_StudentCoursesTemp_GradeId",
                table: "StudentCoursesTemp");

            migrationBuilder.DropColumn(
                name: "GradeId",
                table: "StudentCoursesTemp");
        }
    }
}
