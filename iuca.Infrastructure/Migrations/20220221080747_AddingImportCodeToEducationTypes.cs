using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingImportCodeToEducationTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImportCode",
                table: "InstructorBasicInfo",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ImportCode",
                table: "EducationTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "EducationTypes",
                columns: new[] { "Id", "ImportCode", "NameEng", "NameKir", "NameRus" },
                values: new object[] { 1, 0, "Not assigned", "Не указана", "Не указана" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EducationTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "ImportCode",
                table: "InstructorBasicInfo");

            migrationBuilder.DropColumn(
                name: "ImportCode",
                table: "EducationTypes");
        }
    }
}
