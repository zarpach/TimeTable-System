using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class InstructorExportChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstructorContactInfo_Countries_CitizenshipCountryId",
                table: "InstructorContactInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_InstructorContactInfo_Countries_CountryId",
                table: "InstructorContactInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_InstructorEducationInfo_EducationTypes_EducationTypeId",
                table: "InstructorEducationInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_InstructorEducationInfo_Universities_UniversityId",
                table: "InstructorEducationInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBasicInfo_Countries_CitizenshipId",
                table: "UserBasicInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBasicInfo_Nationalities_NationalityId",
                table: "UserBasicInfo");

            migrationBuilder.AlterColumn<int>(
                name: "NationalityId",
                table: "UserBasicInfo",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CitizenshipId",
                table: "UserBasicInfo",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "UniversityId",
                table: "InstructorEducationInfo",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "EducationTypeId",
                table: "InstructorEducationInfo",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "InstructorContactInfo",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CitizenshipCountryId",
                table: "InstructorContactInfo",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "IsChanged",
                table: "InstructorBasicInfo",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorContactInfo_Countries_CitizenshipCountryId",
                table: "InstructorContactInfo",
                column: "CitizenshipCountryId",
                principalTable: "Countries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorContactInfo_Countries_CountryId",
                table: "InstructorContactInfo",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorEducationInfo_EducationTypes_EducationTypeId",
                table: "InstructorEducationInfo",
                column: "EducationTypeId",
                principalTable: "EducationTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorEducationInfo_Universities_UniversityId",
                table: "InstructorEducationInfo",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBasicInfo_Countries_CitizenshipId",
                table: "UserBasicInfo",
                column: "CitizenshipId",
                principalTable: "Countries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBasicInfo_Nationalities_NationalityId",
                table: "UserBasicInfo",
                column: "NationalityId",
                principalTable: "Nationalities",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstructorContactInfo_Countries_CitizenshipCountryId",
                table: "InstructorContactInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_InstructorContactInfo_Countries_CountryId",
                table: "InstructorContactInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_InstructorEducationInfo_EducationTypes_EducationTypeId",
                table: "InstructorEducationInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_InstructorEducationInfo_Universities_UniversityId",
                table: "InstructorEducationInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBasicInfo_Countries_CitizenshipId",
                table: "UserBasicInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBasicInfo_Nationalities_NationalityId",
                table: "UserBasicInfo");

            migrationBuilder.DropColumn(
                name: "IsChanged",
                table: "InstructorBasicInfo");

            migrationBuilder.AlterColumn<int>(
                name: "NationalityId",
                table: "UserBasicInfo",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CitizenshipId",
                table: "UserBasicInfo",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UniversityId",
                table: "InstructorEducationInfo",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EducationTypeId",
                table: "InstructorEducationInfo",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "InstructorContactInfo",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CitizenshipCountryId",
                table: "InstructorContactInfo",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorContactInfo_Countries_CitizenshipCountryId",
                table: "InstructorContactInfo",
                column: "CitizenshipCountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorContactInfo_Countries_CountryId",
                table: "InstructorContactInfo",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorEducationInfo_EducationTypes_EducationTypeId",
                table: "InstructorEducationInfo",
                column: "EducationTypeId",
                principalTable: "EducationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorEducationInfo_Universities_UniversityId",
                table: "InstructorEducationInfo",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBasicInfo_Countries_CitizenshipId",
                table: "UserBasicInfo",
                column: "CitizenshipId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBasicInfo_Nationalities_NationalityId",
                table: "UserBasicInfo",
                column: "NationalityId",
                principalTable: "Nationalities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
