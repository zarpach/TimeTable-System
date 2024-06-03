using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingNewStudyCards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudyCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SemesterId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentGroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyCards_DepartmentGroups_DepartmentGroupId",
                        column: x => x.DepartmentGroupId,
                        principalTable: "DepartmentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyCards_Semesters_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semesters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyCardCourses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudyCardId = table.Column<int>(type: "integer", nullable: false),
                    RegistrationCourseId = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyCardCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyCardCourses_RegistrationCourses_RegistrationCourseId",
                        column: x => x.RegistrationCourseId,
                        principalTable: "RegistrationCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyCardCourses_StudyCards_StudyCardId",
                        column: x => x.StudyCardId,
                        principalTable: "StudyCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudyCardCourses_RegistrationCourseId_StudyCardId",
                table: "StudyCardCourses",
                columns: new[] { "RegistrationCourseId", "StudyCardId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyCardCourses_StudyCardId",
                table: "StudyCardCourses",
                column: "StudyCardId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCards_DepartmentGroupId",
                table: "StudyCards",
                column: "DepartmentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCards_SemesterId_DepartmentGroupId",
                table: "StudyCards",
                columns: new[] { "SemesterId", "DepartmentGroupId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudyCardCourses");

            migrationBuilder.DropTable(
                name: "StudyCards");
        }
    }
}
