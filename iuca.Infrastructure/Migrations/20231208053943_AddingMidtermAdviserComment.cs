using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingMidtermAdviserComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdviserComment",
                table: "StudentMidterms",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdviserComment",
                table: "StudentMidterms");
        }
    }
}
