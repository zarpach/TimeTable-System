using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingCommentsToDebts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentArrears");

            migrationBuilder.CreateTable(
                name: "StudentDebts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentUserId = table.Column<string>(type: "text", nullable: true),
                    SemesterId = table.Column<int>(type: "integer", nullable: false),
                    AccountingOfficeDebt = table.Column<bool>(type: "boolean", nullable: false),
                    AccountingOfficeComment = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    LibraryDebt = table.Column<bool>(type: "boolean", nullable: false),
                    LibraryComment = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    DormitoryDebt = table.Column<bool>(type: "boolean", nullable: false),
                    DormitoryComment = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    RegisterOfficeDebt = table.Column<bool>(type: "boolean", nullable: false),
                    RegisterOfficeComment = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDebts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentDebts_AspNetUsers_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentDebts_Semesters_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semesters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentDebts_SemesterId",
                table: "StudentDebts",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDebts_StudentUserId",
                table: "StudentDebts",
                column: "StudentUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentDebts");

            migrationBuilder.CreateTable(
                name: "StudentArrears",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SemesterId = table.Column<int>(type: "integer", nullable: false),
                    AccountingOfficeArrear = table.Column<bool>(type: "boolean", nullable: false),
                    DormitoryArrear = table.Column<bool>(type: "boolean", nullable: false),
                    LibraryArrear = table.Column<bool>(type: "boolean", nullable: false),
                    RegisterOfficeArrear = table.Column<bool>(type: "boolean", nullable: false),
                    StudentUserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentArrears", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentArrears_AspNetUsers_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentArrears_Semesters_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semesters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentArrears_SemesterId",
                table: "StudentArrears",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentArrears_StudentUserId",
                table: "StudentArrears",
                column: "StudentUserId");
        }
    }
}
