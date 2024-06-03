using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingTwoResourceFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequiredTexts",
                table: "Syllabi",
                newName: "PrimaryResources");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalResources",
                table: "Syllabi",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalResources",
                table: "Syllabi");

            migrationBuilder.RenameColumn(
                name: "PrimaryResources",
                table: "Syllabi",
                newName: "RequiredTexts");
        }
    }
}
