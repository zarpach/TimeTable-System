using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingSectionAuditable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "AnnouncementSections",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "AnnouncementSections",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AnnouncementSections",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "AnnouncementSections",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "AnnouncementSections",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "AnnouncementSections");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "AnnouncementSections");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AnnouncementSections");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "AnnouncementSections");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "AnnouncementSections");
        }
    }
}
