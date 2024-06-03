using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class ChangingMidtermGrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentMidterms_Grades_GradeId",
                table: "StudentMidterms");

            migrationBuilder.DropIndex(
                name: "IX_StudentMidterms_GradeId",
                table: "StudentMidterms");

            migrationBuilder.DropColumn(
                name: "GradeId",
                table: "StudentMidterms");

            migrationBuilder.AddColumn<int>(
                name: "MidtermScore",
                table: "StudentMidterms",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MidtermScore",
                table: "StudentMidterms");

            migrationBuilder.AddColumn<int>(
                name: "GradeId",
                table: "StudentMidterms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StudentMidterms_GradeId",
                table: "StudentMidterms",
                column: "GradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentMidterms_Grades_GradeId",
                table: "StudentMidterms",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
