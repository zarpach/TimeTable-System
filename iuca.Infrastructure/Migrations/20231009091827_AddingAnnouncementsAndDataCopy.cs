using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingAnnouncementsAndDataCopy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExtraInstructors_RegistrationCourses_RegistrationCourseId",
                table: "ExtraInstructors");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationCourses_AspNetUsers_InstructorUserId",
                table: "RegistrationCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationCourses_Courses_CourseId",
                table: "RegistrationCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationCourses_Organizations_OrganizationId",
                table: "RegistrationCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentCoursesTemp_RegistrationCourses_RegistrationCourseId",
                table: "StudentCoursesTemp");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyCardCourses_RegistrationCourses_RegistrationCourseId",
                table: "StudyCardCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_Syllabi_RegistrationCourses_RegistrationCourseId",
                table: "Syllabi");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RegistrationCourses",
                table: "RegistrationCourses");

            migrationBuilder.RenameTable(
                name: "RegistrationCourses",
                newName: "AnnouncementSections");

            migrationBuilder.RenameColumn(
                name: "RegistrationCourseId",
                table: "Syllabi",
                newName: "AnnouncementSectionId");

            migrationBuilder.RenameIndex(
                name: "IX_Syllabi_RegistrationCourseId",
                table: "Syllabi",
                newName: "IX_Syllabi_AnnouncementSectionId");

            migrationBuilder.RenameColumn(
                name: "RegistrationCourseId",
                table: "StudyCardCourses",
                newName: "AnnouncementSectionId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyCardCourses_RegistrationCourseId_StudyCardId",
                table: "StudyCardCourses",
                newName: "IX_StudyCardCourses_AnnouncementSectionId_StudyCardId");

            migrationBuilder.RenameColumn(
                name: "RegistrationCourseId",
                table: "StudentCoursesTemp",
                newName: "AnnouncementSectionId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentCoursesTemp_RegistrationCourseId",
                table: "StudentCoursesTemp",
                newName: "IX_StudentCoursesTemp_AnnouncementSectionId");

            migrationBuilder.RenameColumn(
                name: "RegistrationCourseId",
                table: "ExtraInstructors",
                newName: "AnnouncementSectionId");

            migrationBuilder.RenameIndex(
                name: "IX_RegistrationCourses_OrganizationId",
                table: "AnnouncementSections",
                newName: "IX_AnnouncementSections_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_RegistrationCourses_InstructorUserId",
                table: "AnnouncementSections",
                newName: "IX_AnnouncementSections_InstructorUserId");

            migrationBuilder.RenameIndex(
                name: "IX_RegistrationCourses_CourseId",
                table: "AnnouncementSections",
                newName: "IX_AnnouncementSections_CourseId");

            migrationBuilder.AddColumn<int>(
                name: "AnnouncementId",
                table: "AnnouncementSections",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnnouncementSections",
                table: "AnnouncementSections",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    SemesterId = table.Column<int>(type: "integer", nullable: false),
                    IsForAll = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Announcements_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Announcements_Semesters_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semesters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnnouncementSectionStudent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentUserId = table.Column<string>(type: "text", nullable: true),
                    AnnouncementSectionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementSectionStudent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnouncementSectionStudent_AnnouncementSections_Announcemen~",
                        column: x => x.AnnouncementSectionId,
                        principalTable: "AnnouncementSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_CourseId",
                table: "Announcements",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_SemesterId_CourseId",
                table: "Announcements",
                columns: new[] { "SemesterId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementSectionStudent_AnnouncementSectionId",
                table: "AnnouncementSectionStudent",
                column: "AnnouncementSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementSectionStudent_StudentUserId_AnnouncementSectio~",
                table: "AnnouncementSectionStudent",
                columns: new[] { "StudentUserId", "AnnouncementSectionId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementSections_AspNetUsers_InstructorUserId",
                table: "AnnouncementSections",
                column: "InstructorUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementSections_Courses_CourseId",
                table: "AnnouncementSections",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementSections_Organizations_OrganizationId",
                table: "AnnouncementSections",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExtraInstructors_AnnouncementSections_AnnouncementSectionId",
                table: "ExtraInstructors",
                column: "AnnouncementSectionId",
                principalTable: "AnnouncementSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCoursesTemp_AnnouncementSections_AnnouncementSection~",
                table: "StudentCoursesTemp",
                column: "AnnouncementSectionId",
                principalTable: "AnnouncementSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyCardCourses_AnnouncementSections_AnnouncementSectionId",
                table: "StudyCardCourses",
                column: "AnnouncementSectionId",
                principalTable: "AnnouncementSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Syllabi_AnnouncementSections_AnnouncementSectionId",
                table: "Syllabi",
                column: "AnnouncementSectionId",
                principalTable: "AnnouncementSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql("INSERT INTO public.\"Announcements\" (\"CourseId\", \"SemesterId\", \"IsForAll\", \"DateCreated\", \"IsDeleted\") SELECT DISTINCT \"CourseId\", (SELECT \"Id\" FROM \"Semesters\" WHERE \"Season\" = \"AnnouncementSections\".\"Season\" AND \"Year\" = \"AnnouncementSections\".\"Year\" AND \"OrganizationId\" = \"AnnouncementSections\".\"OrganizationId\"), FALSE, NOW(), FALSE FROM public.\"AnnouncementSections\"");
            migrationBuilder.Sql("UPDATE public.\"AnnouncementSections\" AS a SET \"AnnouncementId\" = (SELECT MAX(\"Id\") FROM public.\"Announcements\" WHERE \"CourseId\" = a.\"CourseId\" AND \"SemesterId\" = (SELECT \"Id\" FROM \"Semesters\" WHERE \"Season\" = a.\"Season\" AND \"Year\" = a.\"Year\" AND \"OrganizationId\" = a.\"OrganizationId\"))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementSections_AspNetUsers_InstructorUserId",
                table: "AnnouncementSections");

            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementSections_Courses_CourseId",
                table: "AnnouncementSections");

            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementSections_Organizations_OrganizationId",
                table: "AnnouncementSections");

            migrationBuilder.DropForeignKey(
                name: "FK_ExtraInstructors_AnnouncementSections_AnnouncementSectionId",
                table: "ExtraInstructors");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentCoursesTemp_AnnouncementSections_AnnouncementSection~",
                table: "StudentCoursesTemp");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyCardCourses_AnnouncementSections_AnnouncementSectionId",
                table: "StudyCardCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_Syllabi_AnnouncementSections_AnnouncementSectionId",
                table: "Syllabi");

            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "AnnouncementSectionStudent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnnouncementSections",
                table: "AnnouncementSections");

            migrationBuilder.DropColumn(
                name: "AnnouncementId",
                table: "AnnouncementSections");

            migrationBuilder.RenameTable(
                name: "AnnouncementSections",
                newName: "RegistrationCourses");

            migrationBuilder.RenameColumn(
                name: "AnnouncementSectionId",
                table: "Syllabi",
                newName: "RegistrationCourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Syllabi_AnnouncementSectionId",
                table: "Syllabi",
                newName: "IX_Syllabi_RegistrationCourseId");

            migrationBuilder.RenameColumn(
                name: "AnnouncementSectionId",
                table: "StudyCardCourses",
                newName: "RegistrationCourseId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyCardCourses_AnnouncementSectionId_StudyCardId",
                table: "StudyCardCourses",
                newName: "IX_StudyCardCourses_RegistrationCourseId_StudyCardId");

            migrationBuilder.RenameColumn(
                name: "AnnouncementSectionId",
                table: "StudentCoursesTemp",
                newName: "RegistrationCourseId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentCoursesTemp_AnnouncementSectionId",
                table: "StudentCoursesTemp",
                newName: "IX_StudentCoursesTemp_RegistrationCourseId");

            migrationBuilder.RenameColumn(
                name: "AnnouncementSectionId",
                table: "ExtraInstructors",
                newName: "RegistrationCourseId");

            migrationBuilder.RenameIndex(
                name: "IX_AnnouncementSections_OrganizationId",
                table: "RegistrationCourses",
                newName: "IX_RegistrationCourses_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_AnnouncementSections_InstructorUserId",
                table: "RegistrationCourses",
                newName: "IX_RegistrationCourses_InstructorUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AnnouncementSections_CourseId",
                table: "RegistrationCourses",
                newName: "IX_RegistrationCourses_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RegistrationCourses",
                table: "RegistrationCourses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExtraInstructors_RegistrationCourses_RegistrationCourseId",
                table: "ExtraInstructors",
                column: "RegistrationCourseId",
                principalTable: "RegistrationCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationCourses_AspNetUsers_InstructorUserId",
                table: "RegistrationCourses",
                column: "InstructorUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationCourses_Courses_CourseId",
                table: "RegistrationCourses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationCourses_Organizations_OrganizationId",
                table: "RegistrationCourses",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCoursesTemp_RegistrationCourses_RegistrationCourseId",
                table: "StudentCoursesTemp",
                column: "RegistrationCourseId",
                principalTable: "RegistrationCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyCardCourses_RegistrationCourses_RegistrationCourseId",
                table: "StudyCardCourses",
                column: "RegistrationCourseId",
                principalTable: "RegistrationCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Syllabi_RegistrationCourses_RegistrationCourseId",
                table: "Syllabi",
                column: "RegistrationCourseId",
                principalTable: "RegistrationCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
