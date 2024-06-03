using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class ChangingStudentDebts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountingOfficeComment",
                table: "StudentDebts");

            migrationBuilder.DropColumn(
                name: "AccountingOfficeDebt",
                table: "StudentDebts");

            migrationBuilder.DropColumn(
                name: "DormitoryComment",
                table: "StudentDebts");

            migrationBuilder.DropColumn(
                name: "DormitoryDebt",
                table: "StudentDebts");

            migrationBuilder.DropColumn(
                name: "LibraryComment",
                table: "StudentDebts");

            migrationBuilder.DropColumn(
                name: "LibraryDebt",
                table: "StudentDebts");

            migrationBuilder.RenameColumn(
                name: "RegisterOfficeDebt",
                table: "StudentDebts",
                newName: "IsDebt");

            migrationBuilder.RenameColumn(
                name: "RegisterOfficeComment",
                table: "StudentDebts",
                newName: "Comment");

            migrationBuilder.AddColumn<int>(
                name: "DebtType",
                table: "StudentDebts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DebtType",
                table: "StudentDebts");

            migrationBuilder.RenameColumn(
                name: "IsDebt",
                table: "StudentDebts",
                newName: "RegisterOfficeDebt");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "StudentDebts",
                newName: "RegisterOfficeComment");

            migrationBuilder.AddColumn<string>(
                name: "AccountingOfficeComment",
                table: "StudentDebts",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AccountingOfficeDebt",
                table: "StudentDebts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DormitoryComment",
                table: "StudentDebts",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DormitoryDebt",
                table: "StudentDebts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LibraryComment",
                table: "StudentDebts",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LibraryDebt",
                table: "StudentDebts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
