using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class RemoveSyllabusLanguageIdField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Syllabi_Languages_LanguageId",
                table: "Syllabi");

            migrationBuilder.DropIndex(
                name: "IX_Syllabi_LanguageId",
                table: "Syllabi");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "Syllabi");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LanguageId",
                table: "Syllabi",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Syllabi_LanguageId",
                table: "Syllabi",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Syllabi_Languages_LanguageId",
                table: "Syllabi",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id");
        }
    }
}
