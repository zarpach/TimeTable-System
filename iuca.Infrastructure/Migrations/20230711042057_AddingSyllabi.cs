using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingSyllabi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Syllabi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RegistrationCourseId = table.Column<int>(type: "integer", nullable: false),
                    InstructorPhone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OfficeHours = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CourseDescription = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Objectives = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    TeachMethods = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    RequiredTexts = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    InstructorComment = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    ApproverComment = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    Language = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Syllabi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Syllabi_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Syllabi_RegistrationCourses_RegistrationCourseId",
                        column: x => x.RegistrationCourseId,
                        principalTable: "RegistrationCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcademicPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SyllabusId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicPolicies_Syllabi_SyllabusId",
                        column: x => x.SyllabusId,
                        principalTable: "Syllabi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseCalendar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SyllabusId = table.Column<int>(type: "integer", nullable: false),
                    Week = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Topics = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Assignments = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCalendar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseCalendar_Syllabi_SyllabusId",
                        column: x => x.SyllabusId,
                        principalTable: "Syllabi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SyllabusId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Points = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseRequirements_Syllabi_SyllabusId",
                        column: x => x.SyllabusId,
                        principalTable: "Syllabi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicPolicies_SyllabusId_Name",
                table: "AcademicPolicies",
                columns: new[] { "SyllabusId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseCalendar_SyllabusId",
                table: "CourseCalendar",
                column: "SyllabusId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseRequirements_SyllabusId",
                table: "CourseRequirements",
                column: "SyllabusId");

            migrationBuilder.CreateIndex(
                name: "IX_Syllabi_LanguageId",
                table: "Syllabi",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Syllabi_RegistrationCourseId",
                table: "Syllabi",
                column: "RegistrationCourseId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicPolicies");

            migrationBuilder.DropTable(
                name: "CourseCalendar");

            migrationBuilder.DropTable(
                name: "CourseRequirements");

            migrationBuilder.DropTable(
                name: "Syllabi");
        }
    }
}
