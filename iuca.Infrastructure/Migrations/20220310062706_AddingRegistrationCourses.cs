using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingRegistrationCourses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistrationCourses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    InstructorUserId = table.Column<string>(type: "text", nullable: true),
                    CourseDetId = table.Column<int>(type: "integer", nullable: false),
                    Section = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Points = table.Column<float>(type: "real", nullable: false),
                    Language = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    GeneralEducation = table.Column<bool>(type: "boolean", nullable: false),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    NumberOfGroups = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrationCourses_AspNetUsers_InstructorUserId",
                        column: x => x.InstructorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RegistrationCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrationCourses_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationCourses_CourseId",
                table: "RegistrationCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationCourses_InstructorUserId",
                table: "RegistrationCourses",
                column: "InstructorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationCourses_OrganizationId",
                table: "RegistrationCourses",
                column: "OrganizationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrationCourses");
        }
    }
}
