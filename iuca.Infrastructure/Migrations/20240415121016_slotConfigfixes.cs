using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class slotConfigfixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Slots_AnnouncementSectionId",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Slots_LessonPeriodId",
                table: "Slots");

            migrationBuilder.AddColumn<int>(
                name: "SemesterId",
                table: "Slots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Slots_AnnouncementSectionId",
                table: "Slots",
                column: "AnnouncementSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_LessonPeriodId",
                table: "Slots",
                column: "LessonPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_SemesterId",
                table: "Slots",
                column: "SemesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_Semesters_SemesterId",
                table: "Slots",
                column: "SemesterId",
                principalTable: "Semesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slots_Semesters_SemesterId",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Slots_AnnouncementSectionId",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Slots_LessonPeriodId",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Slots_SemesterId",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "SemesterId",
                table: "Slots");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_AnnouncementSectionId",
                table: "Slots",
                column: "AnnouncementSectionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Slots_LessonPeriodId",
                table: "Slots",
                column: "LessonPeriodId",
                unique: true);
        }
    }
}
