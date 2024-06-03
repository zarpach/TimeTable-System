using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingPrepDepartmentGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrepDepartmentGroupId",
                table: "StudentOrgInfo",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentOrgInfo_PrepDepartmentGroupId",
                table: "StudentOrgInfo",
                column: "PrepDepartmentGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentOrgInfo_DepartmentGroups_PrepDepartmentGroupId",
                table: "StudentOrgInfo",
                column: "PrepDepartmentGroupId",
                principalTable: "DepartmentGroups",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentOrgInfo_DepartmentGroups_PrepDepartmentGroupId",
                table: "StudentOrgInfo");

            migrationBuilder.DropIndex(
                name: "IX_StudentOrgInfo_PrepDepartmentGroupId",
                table: "StudentOrgInfo");

            migrationBuilder.DropColumn(
                name: "PrepDepartmentGroupId",
                table: "StudentOrgInfo");
        }
    }
}
