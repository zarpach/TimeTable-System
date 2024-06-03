using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingStudentCoursesTemp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Places",
                table: "RegistrationCourses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StudentCoursesTemp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentCourseRegistrationId = table.Column<int>(type: "integer", nullable: false),
                    RegistrationCourseId = table.Column<int>(type: "integer", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    Comment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsPassed = table.Column<bool>(type: "boolean", nullable: false),
                    Queue = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCoursesTemp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentCoursesTemp_RegistrationCourses_RegistrationCourseId",
                        column: x => x.RegistrationCourseId,
                        principalTable: "RegistrationCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentCoursesTemp_StudentCourseRegistrations_StudentCourse~",
                        column: x => x.StudentCourseRegistrationId,
                        principalTable: "StudentCourseRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentCoursesTemp_RegistrationCourseId",
                table: "StudentCoursesTemp",
                column: "RegistrationCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCoursesTemp_StudentCourseRegistrationId",
                table: "StudentCoursesTemp",
                column: "StudentCourseRegistrationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentCoursesTemp");

            migrationBuilder.DropColumn(
                name: "Places",
                table: "RegistrationCourses");
        }
    }
}
