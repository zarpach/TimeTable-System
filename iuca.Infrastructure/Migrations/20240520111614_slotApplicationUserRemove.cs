using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class slotApplicationUserRemove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slots_User_InstructorUserId",
                table: "Slots");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_Slots_InstructorUserId",
                table: "Slots");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "IX_Slots_InstructorUserId",
                table: "Slots",
                column: "InstructorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_User_InstructorUserId",
                table: "Slots",
                column: "InstructorUserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
