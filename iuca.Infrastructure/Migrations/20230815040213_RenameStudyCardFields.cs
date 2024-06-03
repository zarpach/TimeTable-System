using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class RenameStudyCardFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyCardCourses_StudyCards_StudyCardId",
                table: "StudyCardCourses");

            migrationBuilder.RenameColumn(
                name: "StudyCardId",
                table: "StudyCardCourses",
                newName: "OldStudyCardId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyCardCourses_StudyCardId",
                table: "StudyCardCourses",
                newName: "IX_StudyCardCourses_OldStudyCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyCardCourses_StudyCards_OldStudyCardId",
                table: "StudyCardCourses",
                column: "OldStudyCardId",
                principalTable: "StudyCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyCardCourses_StudyCards_OldStudyCardId",
                table: "StudyCardCourses");

            migrationBuilder.RenameColumn(
                name: "OldStudyCardId",
                table: "StudyCardCourses",
                newName: "StudyCardId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyCardCourses_OldStudyCardId",
                table: "StudyCardCourses",
                newName: "IX_StudyCardCourses_StudyCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyCardCourses_StudyCards_StudyCardId",
                table: "StudyCardCourses",
                column: "StudyCardId",
                principalTable: "StudyCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
