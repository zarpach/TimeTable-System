using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class slotsadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LessonPeriod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    TimeBegin = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    TimeEnd = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonPeriod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LessonRooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomName = table.Column<string>(type: "text", nullable: true),
                    RoomCapacity = table.Column<int>(type: "integer", nullable: true),
                    Floor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonRooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Slots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    InstructorUserId = table.Column<string>(type: "text", nullable: true),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    LessonPeriodId = table.Column<int>(type: "integer", nullable: false),
                    LessonRoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnnouncementSectionId = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Slots_AnnouncementSections_AnnouncementSectionId",
                        column: x => x.AnnouncementSectionId,
                        principalTable: "AnnouncementSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Slots_AspNetUsers_InstructorUserId",
                        column: x => x.InstructorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Slots_DepartmentGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "DepartmentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Slots_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Slots_LessonPeriod_LessonPeriodId",
                        column: x => x.LessonPeriodId,
                        principalTable: "LessonPeriod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Slots_LessonRooms_LessonRoomId",
                        column: x => x.LessonRoomId,
                        principalTable: "LessonRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Slots_AnnouncementSectionId",
                table: "Slots",
                column: "AnnouncementSectionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Slots_DepartmentId",
                table: "Slots",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_GroupId",
                table: "Slots",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_InstructorUserId",
                table: "Slots",
                column: "InstructorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_LessonPeriodId",
                table: "Slots",
                column: "LessonPeriodId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Slots_LessonRoomId",
                table: "Slots",
                column: "LessonRoomId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Slots");

            migrationBuilder.DropTable(
                name: "LessonPeriod");

            migrationBuilder.DropTable(
                name: "LessonRooms");
        }
    }
}
