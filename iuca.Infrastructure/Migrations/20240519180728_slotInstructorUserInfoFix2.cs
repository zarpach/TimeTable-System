using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class slotInstructorUserInfoFix2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstructorUserId1",
                table: "Slots",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Slots_InstructorUserId1",
                table: "Slots",
                column: "InstructorUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_UserBasicInfo_InstructorUserId1",
                table: "Slots",
                column: "InstructorUserId1",
                principalTable: "UserBasicInfo",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slots_UserBasicInfo_InstructorUserId1",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Slots_InstructorUserId1",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "InstructorUserId1",
                table: "Slots");
        }
    }
}
