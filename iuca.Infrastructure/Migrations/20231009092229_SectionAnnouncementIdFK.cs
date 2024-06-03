using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class SectionAnnouncementIdFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementSections_AnnouncementId",
                table: "AnnouncementSections",
                column: "AnnouncementId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementSections_Announcements_AnnouncementId",
                table: "AnnouncementSections",
                column: "AnnouncementId",
                principalTable: "Announcements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementSections_Announcements_AnnouncementId",
                table: "AnnouncementSections");

            migrationBuilder.DropIndex(
                name: "IX_AnnouncementSections_AnnouncementId",
                table: "AnnouncementSections");
        }
    }
}
