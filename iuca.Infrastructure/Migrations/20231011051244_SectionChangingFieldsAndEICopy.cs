using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class SectionChangingFieldsAndEICopy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescEng",
                table: "AnnouncementSections");

            migrationBuilder.DropColumn(
                name: "DescKir",
                table: "AnnouncementSections");

            migrationBuilder.DropColumn(
                name: "DescRus",
                table: "AnnouncementSections");

            migrationBuilder.DropColumn(
                name: "GeneralEducation",
                table: "AnnouncementSections");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "AnnouncementSections");

            migrationBuilder.DropColumn(
                name: "NumberOfGroups",
                table: "AnnouncementSections");

            migrationBuilder.DropColumn(
                name: "Requirements",
                table: "AnnouncementSections");

            migrationBuilder.RenameColumn(
                name: "Points",
                table: "AnnouncementSections",
                newName: "Credits");

            migrationBuilder.AddColumn<IEnumerable<string>>(
                name: "ExtraInstructorsJson",
                table: "AnnouncementSections",
                type: "jsonb",
                nullable: true);

            migrationBuilder.Sql("UPDATE public.\"AnnouncementSections\" AS a SET \"ExtraInstructorsJson\" = (SELECT json_agg(json_build_object('Id', e.\"InstructorUserId\", 'FullName', CONCAT(u.\"LastNameEng\", ' ', u.\"FirstNameEng\", ' ', u.\"MiddleNameEng\"))::text) FROM public.\"ExtraInstructors\" AS e JOIN public.\"AspNetUsers\" AS u ON e.\"InstructorUserId\" = u.\"Id\" WHERE e.\"AnnouncementSectionId\" = a.\"Id\")");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtraInstructorsJson",
                table: "AnnouncementSections");

            migrationBuilder.RenameColumn(
                name: "Credits",
                table: "AnnouncementSections",
                newName: "Points");

            migrationBuilder.AddColumn<string>(
                name: "DescEng",
                table: "AnnouncementSections",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescKir",
                table: "AnnouncementSections",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescRus",
                table: "AnnouncementSections",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GeneralEducation",
                table: "AnnouncementSections",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "AnnouncementSections",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfGroups",
                table: "AnnouncementSections",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Requirements",
                table: "AnnouncementSections",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
