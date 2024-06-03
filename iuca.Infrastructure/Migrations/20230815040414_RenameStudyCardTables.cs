using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class RenameStudyCardTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourses_StudyCardCourses_StudyCardCourseId",
                table: "StudentCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyCardCourses_AspNetUsers_InstructorUserId",
                table: "StudyCardCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyCardCourses_CyclePartCourses_CyclePartCourseId",
                table: "StudyCardCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyCardCourses_InstructorBasicInfo_InstructorBasicInfoId",
                table: "StudyCardCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyCardCourses_StudyCards_OldStudyCardId",
                table: "StudyCardCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyCards_CyclePartCourses_CyclePartCourseId",
                table: "StudyCards");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyCards_DepartmentGroups_DepartmentGroupId",
                table: "StudyCards");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyCards_Organizations_OrganizationId",
                table: "StudyCards");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyCards_Semesters_SemesterId",
                table: "StudyCards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyCards",
                table: "StudyCards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyCardCourses",
                table: "StudyCardCourses");

            migrationBuilder.RenameTable(
                name: "StudyCards",
                newName: "OldStudyCards");

            migrationBuilder.RenameTable(
                name: "StudyCardCourses",
                newName: "OldStudyCardCourses");

            migrationBuilder.RenameIndex(
                name: "IX_StudyCards_SemesterId",
                table: "OldStudyCards",
                newName: "IX_OldStudyCards_SemesterId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyCards_OrganizationId",
                table: "OldStudyCards",
                newName: "IX_OldStudyCards_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyCards_DepartmentGroupId",
                table: "OldStudyCards",
                newName: "IX_OldStudyCards_DepartmentGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyCards_CyclePartCourseId",
                table: "OldStudyCards",
                newName: "IX_OldStudyCards_CyclePartCourseId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyCardCourses_OldStudyCardId",
                table: "OldStudyCardCourses",
                newName: "IX_OldStudyCardCourses_OldStudyCardId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyCardCourses_InstructorUserId",
                table: "OldStudyCardCourses",
                newName: "IX_OldStudyCardCourses_InstructorUserId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyCardCourses_InstructorBasicInfoId",
                table: "OldStudyCardCourses",
                newName: "IX_OldStudyCardCourses_InstructorBasicInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyCardCourses_CyclePartCourseId",
                table: "OldStudyCardCourses",
                newName: "IX_OldStudyCardCourses_CyclePartCourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OldStudyCards",
                table: "OldStudyCards",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OldStudyCardCourses",
                table: "OldStudyCardCourses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OldStudyCardCourses_AspNetUsers_InstructorUserId",
                table: "OldStudyCardCourses",
                column: "InstructorUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OldStudyCardCourses_CyclePartCourses_CyclePartCourseId",
                table: "OldStudyCardCourses",
                column: "CyclePartCourseId",
                principalTable: "CyclePartCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OldStudyCardCourses_InstructorBasicInfo_InstructorBasicInfo~",
                table: "OldStudyCardCourses",
                column: "InstructorBasicInfoId",
                principalTable: "InstructorBasicInfo",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OldStudyCardCourses_OldStudyCards_OldStudyCardId",
                table: "OldStudyCardCourses",
                column: "OldStudyCardId",
                principalTable: "OldStudyCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OldStudyCards_CyclePartCourses_CyclePartCourseId",
                table: "OldStudyCards",
                column: "CyclePartCourseId",
                principalTable: "CyclePartCourses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OldStudyCards_DepartmentGroups_DepartmentGroupId",
                table: "OldStudyCards",
                column: "DepartmentGroupId",
                principalTable: "DepartmentGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OldStudyCards_Organizations_OrganizationId",
                table: "OldStudyCards",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OldStudyCards_Semesters_SemesterId",
                table: "OldStudyCards",
                column: "SemesterId",
                principalTable: "Semesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourses_OldStudyCardCourses_StudyCardCourseId",
                table: "StudentCourses",
                column: "StudyCardCourseId",
                principalTable: "OldStudyCardCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OldStudyCardCourses_AspNetUsers_InstructorUserId",
                table: "OldStudyCardCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_OldStudyCardCourses_CyclePartCourses_CyclePartCourseId",
                table: "OldStudyCardCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_OldStudyCardCourses_InstructorBasicInfo_InstructorBasicInfo~",
                table: "OldStudyCardCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_OldStudyCardCourses_OldStudyCards_OldStudyCardId",
                table: "OldStudyCardCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_OldStudyCards_CyclePartCourses_CyclePartCourseId",
                table: "OldStudyCards");

            migrationBuilder.DropForeignKey(
                name: "FK_OldStudyCards_DepartmentGroups_DepartmentGroupId",
                table: "OldStudyCards");

            migrationBuilder.DropForeignKey(
                name: "FK_OldStudyCards_Organizations_OrganizationId",
                table: "OldStudyCards");

            migrationBuilder.DropForeignKey(
                name: "FK_OldStudyCards_Semesters_SemesterId",
                table: "OldStudyCards");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourses_OldStudyCardCourses_StudyCardCourseId",
                table: "StudentCourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OldStudyCards",
                table: "OldStudyCards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OldStudyCardCourses",
                table: "OldStudyCardCourses");

            migrationBuilder.RenameTable(
                name: "OldStudyCards",
                newName: "StudyCards");

            migrationBuilder.RenameTable(
                name: "OldStudyCardCourses",
                newName: "StudyCardCourses");

            migrationBuilder.RenameIndex(
                name: "IX_OldStudyCards_SemesterId",
                table: "StudyCards",
                newName: "IX_StudyCards_SemesterId");

            migrationBuilder.RenameIndex(
                name: "IX_OldStudyCards_OrganizationId",
                table: "StudyCards",
                newName: "IX_StudyCards_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_OldStudyCards_DepartmentGroupId",
                table: "StudyCards",
                newName: "IX_StudyCards_DepartmentGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_OldStudyCards_CyclePartCourseId",
                table: "StudyCards",
                newName: "IX_StudyCards_CyclePartCourseId");

            migrationBuilder.RenameIndex(
                name: "IX_OldStudyCardCourses_OldStudyCardId",
                table: "StudyCardCourses",
                newName: "IX_StudyCardCourses_OldStudyCardId");

            migrationBuilder.RenameIndex(
                name: "IX_OldStudyCardCourses_InstructorUserId",
                table: "StudyCardCourses",
                newName: "IX_StudyCardCourses_InstructorUserId");

            migrationBuilder.RenameIndex(
                name: "IX_OldStudyCardCourses_InstructorBasicInfoId",
                table: "StudyCardCourses",
                newName: "IX_StudyCardCourses_InstructorBasicInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_OldStudyCardCourses_CyclePartCourseId",
                table: "StudyCardCourses",
                newName: "IX_StudyCardCourses_CyclePartCourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyCards",
                table: "StudyCards",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyCardCourses",
                table: "StudyCardCourses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourses_StudyCardCourses_StudyCardCourseId",
                table: "StudentCourses",
                column: "StudyCardCourseId",
                principalTable: "StudyCardCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyCardCourses_AspNetUsers_InstructorUserId",
                table: "StudyCardCourses",
                column: "InstructorUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyCardCourses_CyclePartCourses_CyclePartCourseId",
                table: "StudyCardCourses",
                column: "CyclePartCourseId",
                principalTable: "CyclePartCourses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyCardCourses_InstructorBasicInfo_InstructorBasicInfoId",
                table: "StudyCardCourses",
                column: "InstructorBasicInfoId",
                principalTable: "InstructorBasicInfo",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyCardCourses_StudyCards_OldStudyCardId",
                table: "StudyCardCourses",
                column: "OldStudyCardId",
                principalTable: "StudyCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyCards_CyclePartCourses_CyclePartCourseId",
                table: "StudyCards",
                column: "CyclePartCourseId",
                principalTable: "CyclePartCourses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyCards_DepartmentGroups_DepartmentGroupId",
                table: "StudyCards",
                column: "DepartmentGroupId",
                principalTable: "DepartmentGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyCards_Organizations_OrganizationId",
                table: "StudyCards",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyCards_Semesters_SemesterId",
                table: "StudyCards",
                column: "SemesterId",
                principalTable: "Semesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
