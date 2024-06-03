using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingOrganizationToGPA : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "StudentTotalGPAs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "StudentSemesterGPAs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StudentTotalGPAs_OrganizationId",
                table: "StudentTotalGPAs",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSemesterGPAs_OrganizationId",
                table: "StudentSemesterGPAs",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSemesterGPAs_Organizations_OrganizationId",
                table: "StudentSemesterGPAs",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentTotalGPAs_Organizations_OrganizationId",
                table: "StudentTotalGPAs",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentSemesterGPAs_Organizations_OrganizationId",
                table: "StudentSemesterGPAs");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentTotalGPAs_Organizations_OrganizationId",
                table: "StudentTotalGPAs");

            migrationBuilder.DropIndex(
                name: "IX_StudentTotalGPAs_OrganizationId",
                table: "StudentTotalGPAs");

            migrationBuilder.DropIndex(
                name: "IX_StudentSemesterGPAs_OrganizationId",
                table: "StudentSemesterGPAs");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "StudentTotalGPAs");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "StudentSemesterGPAs");
        }
    }
}
