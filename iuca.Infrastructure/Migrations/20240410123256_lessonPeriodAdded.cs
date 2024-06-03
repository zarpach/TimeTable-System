using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class lessonPeriodAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slots_LessonPeriod_LessonPeriodId",
                table: "Slots");

            migrationBuilder.DropTable(
                name: "LessonPeriod");

            migrationBuilder.CreateTable(
                name: "LessonPeriods",
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
                    table.PrimaryKey("PK_LessonPeriods", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_LessonPeriods_LessonPeriodId",
                table: "Slots",
                column: "LessonPeriodId",
                principalTable: "LessonPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slots_LessonPeriods_LessonPeriodId",
                table: "Slots");

            migrationBuilder.DropTable(
                name: "LessonPeriods");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_LessonPeriod_LessonPeriodId",
                table: "Slots",
                column: "LessonPeriodId",
                principalTable: "LessonPeriod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
