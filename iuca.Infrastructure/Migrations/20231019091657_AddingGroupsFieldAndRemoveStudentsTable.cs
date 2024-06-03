using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingGroupsFieldAndRemoveStudentsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnouncementSectionStudent");

            migrationBuilder.AddColumn<IEnumerable<string>>(
                name: "GroupsJson",
                table: "AnnouncementSections",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupsJson",
                table: "AnnouncementSections");

            migrationBuilder.CreateTable(
                name: "AnnouncementSectionStudent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnnouncementSectionId = table.Column<int>(type: "integer", nullable: false),
                    StudentUserId = table.Column<string>(type: "text", nullable: true)
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
                name: "IX_AnnouncementSectionStudent_AnnouncementSectionId",
                table: "AnnouncementSectionStudent",
                column: "AnnouncementSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementSectionStudent_StudentUserId_AnnouncementSectio~",
                table: "AnnouncementSectionStudent",
                columns: new[] { "StudentUserId", "AnnouncementSectionId" },
                unique: true);
        }
    }
}
