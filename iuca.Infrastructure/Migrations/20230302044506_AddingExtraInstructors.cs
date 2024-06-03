using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingExtraInstructors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExtraInstructors",
                columns: table => new
                {
                    RegistrationCourseId = table.Column<int>(type: "integer", nullable: false),
                    InstructorUserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraInstructors", x => new { x.RegistrationCourseId, x.InstructorUserId });
                    table.ForeignKey(
                        name: "FK_ExtraInstructors_AspNetUsers_InstructorUserId",
                        column: x => x.InstructorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExtraInstructors_RegistrationCourses_RegistrationCourseId",
                        column: x => x.RegistrationCourseId,
                        principalTable: "RegistrationCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExtraInstructors_InstructorUserId",
                table: "ExtraInstructors",
                column: "InstructorUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExtraInstructors");
        }
    }
}
