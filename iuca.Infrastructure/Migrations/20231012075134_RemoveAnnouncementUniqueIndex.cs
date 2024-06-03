using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class RemoveAnnouncementUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Announcements_SemesterId_CourseId",
                table: "Announcements");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_SemesterId",
                table: "Announcements",
                column: "SemesterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Announcements_SemesterId",
                table: "Announcements");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_SemesterId_CourseId",
                table: "Announcements",
                columns: new[] { "SemesterId", "CourseId" },
                unique: true);
        }
    }
}
