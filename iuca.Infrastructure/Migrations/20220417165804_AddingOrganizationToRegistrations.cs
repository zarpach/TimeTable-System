using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingOrganizationToRegistrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "StudentCourseRegistrations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseRegistrations_OrganizationId",
                table: "StudentCourseRegistrations",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourseRegistrations_Organizations_OrganizationId",
                table: "StudentCourseRegistrations",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourseRegistrations_Organizations_OrganizationId",
                table: "StudentCourseRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_StudentCourseRegistrations_OrganizationId",
                table: "StudentCourseRegistrations");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "StudentCourseRegistrations");
        }
    }
}
