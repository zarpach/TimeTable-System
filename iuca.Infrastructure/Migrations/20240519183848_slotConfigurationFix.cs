using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class slotConfigurationFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slots_AspNetUsers_InstructorUserId",
                table: "Slots");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Slots",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Slots_ApplicationUserId",
                table: "Slots",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_AspNetUsers_ApplicationUserId",
                table: "Slots",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_User_InstructorUserId",
                table: "Slots",
                column: "InstructorUserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slots_AspNetUsers_ApplicationUserId",
                table: "Slots");

            migrationBuilder.DropForeignKey(
                name: "FK_Slots_User_InstructorUserId",
                table: "Slots");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_Slots_ApplicationUserId",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Slots");

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_AspNetUsers_InstructorUserId",
                table: "Slots",
                column: "InstructorUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
