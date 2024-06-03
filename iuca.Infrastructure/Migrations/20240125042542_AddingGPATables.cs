using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingGPATables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentSemesterGPAs",
                columns: table => new
                {
                    StudentUserId = table.Column<string>(type: "text", nullable: false),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    GPA = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSemesterGPAs", x => new { x.StudentUserId, x.Season, x.Year });
                    table.ForeignKey(
                        name: "FK_StudentSemesterGPAs_AspNetUsers_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentTotalGPAs",
                columns: table => new
                {
                    StudentUserId = table.Column<string>(type: "text", nullable: false),
                    TotalGPA = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentTotalGPAs", x => x.StudentUserId);
                    table.ForeignKey(
                        name: "FK_StudentTotalGPAs_AspNetUsers_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentSemesterGPAs");

            migrationBuilder.DropTable(
                name: "StudentTotalGPAs");
        }
    }
}
