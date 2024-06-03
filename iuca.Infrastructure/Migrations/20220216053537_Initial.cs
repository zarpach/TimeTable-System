using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsMainOrganization = table.Column<bool>(type: "boolean", nullable: false),
                    LastNameEng = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FirstNameEng = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MiddleNameEng = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NameEng = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NameRus = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NameKir = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ImportCode = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cycles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameEng = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NameRus = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameKir = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cycles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameEng = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NameRus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NameKir = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GradeMark = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Gpa = table.Column<float>(type: "real", nullable: false),
                    NameEng = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NameRus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NameKir = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ImportCode = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NameEng = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NameRus = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NameKir = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ImportCode = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nationalities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameEng = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NameRus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NameKir = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ImportCode = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nationalities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsMain = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstructorBasicInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsMainOrganization = table.Column<bool>(type: "boolean", nullable: false),
                    IsMarried = table.Column<bool>(type: "boolean", nullable: false),
                    ChildrenQty = table.Column<int>(type: "integer", nullable: false),
                    InstructorUserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorBasicInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstructorBasicInfo_AspNetUsers_InstructorUserId",
                        column: x => x.InstructorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StaffBasicInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StaffInfo = table.Column<string>(type: "text", nullable: true),
                    IsMainOrganization = table.Column<bool>(type: "boolean", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffBasicInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffBasicInfo_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StudentBasicInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsMainOrganization = table.Column<bool>(type: "boolean", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: true),
                    ArmyService = table.Column<bool>(type: "boolean", nullable: false),
                    Toefl = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentBasicInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentBasicInfo_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Universities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NameEng = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameRus = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameKir = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CountryId = table.Column<int>(type: "integer", nullable: true),
                    ImportCode = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Universities_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserBasicInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LastNameRus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FirstNameRus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MiddleNameRus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Sex = table.Column<int>(type: "integer", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsMainOrganization = table.Column<bool>(type: "boolean", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: true),
                    NationalityId = table.Column<int>(type: "integer", nullable: false),
                    CitizenshipId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBasicInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBasicInfo_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserBasicInfo_Countries_CitizenshipId",
                        column: x => x.CitizenshipId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBasicInfo_Nationalities_NationalityId",
                        column: x => x.NationalityId,
                        principalTable: "Nationalities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdviserStudents",
                columns: table => new
                {
                    InstructorUserId = table.Column<string>(type: "text", nullable: false),
                    StudentUserId = table.Column<string>(type: "text", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdviserStudents", x => new { x.InstructorUserId, x.StudentUserId, x.OrganizationId });
                    table.ForeignKey(
                        name: "FK_AdviserStudents_AspNetUsers_InstructorUserId",
                        column: x => x.InstructorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdviserStudents_AspNetUsers_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdviserStudents_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeanAdvisers",
                columns: table => new
                {
                    DeanUserId = table.Column<string>(type: "text", nullable: false),
                    AdviserUserId = table.Column<string>(type: "text", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeanAdvisers", x => new { x.DeanUserId, x.AdviserUserId, x.OrganizationId });
                    table.ForeignKey(
                        name: "FK_DeanAdvisers_AspNetUsers_AdviserUserId",
                        column: x => x.AdviserUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeanAdvisers_AspNetUsers_DeanUserId",
                        column: x => x.DeanUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeanAdvisers_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NameEng = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NameRus = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NameKir = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    ImportCode = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Semesters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semesters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Semesters_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTypeOrganizations",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "text", nullable: false),
                    UserType = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypeOrganizations", x => new { x.ApplicationUserId, x.UserType, x.OrganizationId });
                    table.ForeignKey(
                        name: "FK_UserTypeOrganizations_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTypeOrganizations_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstructorContactInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InstructorBasicInfoId = table.Column<int>(type: "integer", nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: false),
                    CityEng = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StreetEng = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AddressEng = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CityRus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StreetRus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AddressRus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ZipCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CitizenshipCountryId = table.Column<int>(type: "integer", nullable: false),
                    CitizenshipCityEng = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CitizenshipStreetEng = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CitizenshipAddressEng = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CitizenshipCityRus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CitizenshipStreetRus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CitizenshipAddressRus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CitizenshipZipCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CitizenshipPhone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ContactNameEng = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ContactNameRus = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ContactPhone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RelationEng = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RelationRus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RelationKir = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorContactInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstructorContactInfo_Countries_CitizenshipCountryId",
                        column: x => x.CitizenshipCountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstructorContactInfo_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstructorContactInfo_InstructorBasicInfo_InstructorBasicIn~",
                        column: x => x.InstructorBasicInfoId,
                        principalTable: "InstructorBasicInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstructorOtherJobInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlaceNameEng = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PlaceNameRus = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PlaceNameKir = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PositionEng = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PositionRus = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PositionKir = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    InstructorBasicInfoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorOtherJobInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstructorOtherJobInfo_InstructorBasicInfo_InstructorBasicI~",
                        column: x => x.InstructorBasicInfoId,
                        principalTable: "InstructorBasicInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentContactInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentBasicInfoId = table.Column<int>(type: "integer", nullable: false),
                    StreetEng = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CityEng = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    StreetRus = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CityRus = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CountryId = table.Column<int>(type: "integer", nullable: false),
                    Zip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CitizenshipStreetEng = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CitizenshipCityEng = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CitizenshipStreetRus = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CitizenshipCityRus = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CitizenshipCountryId = table.Column<int>(type: "integer", nullable: false),
                    CitizenshipZip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CitizenshipPhone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ContactNameEng = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ContactNameRus = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ContactPhone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RelationEng = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RelationRus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RelationKir = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentContactInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentContactInfo_Countries_CitizenshipCountryId",
                        column: x => x.CitizenshipCountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentContactInfo_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentContactInfo_StudentBasicInfo_StudentBasicInfoId",
                        column: x => x.StudentBasicInfoId,
                        principalTable: "StudentBasicInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentLanguages",
                columns: table => new
                {
                    StudentBasicInfoId = table.Column<int>(type: "integer", nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentLanguages", x => new { x.StudentBasicInfoId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_StudentLanguages_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentLanguages_StudentBasicInfo_StudentBasicInfoId",
                        column: x => x.StudentBasicInfoId,
                        principalTable: "StudentBasicInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentParentsInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentBasicInfoId = table.Column<int>(type: "integer", nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    WorkPlace = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Relation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DeadYear = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentParentsInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentParentsInfo_StudentBasicInfo_StudentBasicInfoId",
                        column: x => x.StudentBasicInfoId,
                        principalTable: "StudentBasicInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstructorEducationInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MajorEng = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MajorRus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MajorKir = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GraduateYear = table.Column<int>(type: "integer", nullable: false),
                    InstructorBasicInfoId = table.Column<int>(type: "integer", nullable: false),
                    UniversityId = table.Column<int>(type: "integer", nullable: false),
                    EducationTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorEducationInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstructorEducationInfo_EducationTypes_EducationTypeId",
                        column: x => x.EducationTypeId,
                        principalTable: "EducationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstructorEducationInfo_InstructorBasicInfo_InstructorBasic~",
                        column: x => x.InstructorBasicInfoId,
                        principalTable: "InstructorBasicInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstructorEducationInfo_Universities_UniversityId",
                        column: x => x.UniversityId,
                        principalTable: "Universities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcademicPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicPlans_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcademicPlans_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameEng = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameRus = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameKir = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Abbreviation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    ImportCode = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Courses_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Courses_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeanDepartments",
                columns: table => new
                {
                    DeanUserId = table.Column<string>(type: "text", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeanDepartments", x => new { x.DeanUserId, x.DepartmentId, x.OrganizationId });
                    table.ForeignKey(
                        name: "FK_DeanDepartments_AspNetUsers_DeanUserId",
                        column: x => x.DeanUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeanDepartments_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeanDepartments_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentGroups_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartmentGroups_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstructorOrgInfo",
                columns: table => new
                {
                    InstructorBasicInfoId = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    PartTime = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorOrgInfo", x => new { x.InstructorBasicInfoId, x.OrganizationId });
                    table.ForeignKey(
                        name: "FK_InstructorOrgInfo_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstructorOrgInfo_InstructorBasicInfo_InstructorBasicInfoId",
                        column: x => x.InstructorBasicInfoId,
                        principalTable: "InstructorBasicInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstructorOrgInfo_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SemesterPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SemesterId = table.Column<int>(type: "integer", nullable: false),
                    Period = table.Column<int>(type: "integer", nullable: false),
                    DateBegin = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemesterPeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SemesterPeriods_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SemesterPeriods_Semesters_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semesters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentArrears",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentUserId = table.Column<string>(type: "text", nullable: true),
                    SemesterId = table.Column<int>(type: "integer", nullable: false),
                    AccountingOfficeArrear = table.Column<bool>(type: "boolean", nullable: false),
                    LibraryArrear = table.Column<bool>(type: "boolean", nullable: false),
                    DormitoryArrear = table.Column<bool>(type: "boolean", nullable: false),
                    RegisterOfficeArrear = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentArrears", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentArrears_AspNetUsers_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentArrears_Semesters_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semesters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentCourseRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentUserId = table.Column<string>(type: "text", nullable: true),
                    SemesterId = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    DateCreate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DateChange = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCourseRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentCourseRegistrations_AspNetUsers_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentCourseRegistrations_Semesters_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semesters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CycleParts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AcademicPlanId = table.Column<int>(type: "integer", nullable: false),
                    CycleId = table.Column<int>(type: "integer", nullable: false),
                    AcademicPlanPart = table.Column<int>(type: "integer", nullable: false),
                    ReqPts = table.Column<int>(type: "integer", nullable: false),
                    ReqPtsCrs1Sem1 = table.Column<int>(type: "integer", nullable: false),
                    ReqPtsCrs1Sem2 = table.Column<int>(type: "integer", nullable: false),
                    ReqPtsCrs2Sem1 = table.Column<int>(type: "integer", nullable: false),
                    ReqPtsCrs2Sem2 = table.Column<int>(type: "integer", nullable: false),
                    ReqPtsCrs3Sem1 = table.Column<int>(type: "integer", nullable: false),
                    ReqPtsCrs3Sem2 = table.Column<int>(type: "integer", nullable: false),
                    ReqPtsCrs4Sem1 = table.Column<int>(type: "integer", nullable: false),
                    ReqPtsCrs4Sem2 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CycleParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CycleParts_AcademicPlans_AcademicPlanId",
                        column: x => x.AcademicPlanId,
                        principalTable: "AcademicPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CycleParts_Cycles_CycleId",
                        column: x => x.CycleId,
                        principalTable: "Cycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoursePrerequisites",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    PrerequisiteId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursePrerequisites", x => new { x.CourseId, x.PrerequisiteId });
                    table.ForeignKey(
                        name: "FK_CoursePrerequisites_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoursePrerequisites_Courses_PrerequisiteId",
                        column: x => x.PrerequisiteId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentCourseGrades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentUserId = table.Column<string>(type: "text", nullable: true),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Points = table.Column<float>(type: "real", nullable: false),
                    GradeId = table.Column<int>(type: "integer", nullable: true),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    ImportCode = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCourseGrades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentCourseGrades_AspNetUsers_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentCourseGrades_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentCourseGrades_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentCourseGrades_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentOrgInfo",
                columns: table => new
                {
                    StudentBasicInfoId = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentGroupId = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    IsPrep = table.Column<bool>(type: "boolean", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentOrgInfo", x => new { x.StudentBasicInfoId, x.OrganizationId });
                    table.ForeignKey(
                        name: "FK_StudentOrgInfo_DepartmentGroups_DepartmentGroupId",
                        column: x => x.DepartmentGroupId,
                        principalTable: "DepartmentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentOrgInfo_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentOrgInfo_StudentBasicInfo_StudentBasicInfoId",
                        column: x => x.StudentBasicInfoId,
                        principalTable: "StudentBasicInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CyclePartCourses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CyclePartId = table.Column<int>(type: "integer", nullable: false),
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupName = table.Column<string>(type: "text", nullable: true),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    PtsCrs1Sem1 = table.Column<int>(type: "integer", nullable: false),
                    PtsCrs1Sem2 = table.Column<int>(type: "integer", nullable: false),
                    PtsCrs2Sem1 = table.Column<int>(type: "integer", nullable: false),
                    PtsCrs2Sem2 = table.Column<int>(type: "integer", nullable: false),
                    PtsCrs3Sem1 = table.Column<int>(type: "integer", nullable: false),
                    PtsCrs3Sem2 = table.Column<int>(type: "integer", nullable: false),
                    PtsCrs4Sem1 = table.Column<int>(type: "integer", nullable: false),
                    PtsCrs4Sem2 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CyclePartCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CyclePartCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CyclePartCourses_CycleParts_CyclePartId",
                        column: x => x.CyclePartId,
                        principalTable: "CycleParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SemesterId = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentGroupId = table.Column<int>(type: "integer", nullable: false),
                    CyclePartCourseId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyCards_CyclePartCourses_CyclePartCourseId",
                        column: x => x.CyclePartCourseId,
                        principalTable: "CyclePartCourses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudyCards_DepartmentGroups_DepartmentGroupId",
                        column: x => x.DepartmentGroupId,
                        principalTable: "DepartmentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyCards_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyCards_Semesters_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semesters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransferCourses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentUserId = table.Column<string>(type: "text", nullable: true),
                    UniversityId = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    GradeId = table.Column<int>(type: "integer", nullable: true),
                    IsAcademicPlanCourse = table.Column<bool>(type: "boolean", nullable: false),
                    CyclePartCourseId = table.Column<int>(type: "integer", nullable: true),
                    NameEng = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameRus = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameKir = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Points = table.Column<float>(type: "real", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferCourses_AspNetUsers_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransferCourses_CyclePartCourses_CyclePartCourseId",
                        column: x => x.CyclePartCourseId,
                        principalTable: "CyclePartCourses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransferCourses_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransferCourses_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransferCourses_Universities_UniversityId",
                        column: x => x.UniversityId,
                        principalTable: "Universities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyCardCourses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudyCardId = table.Column<int>(type: "integer", nullable: false),
                    CyclePartCourseId = table.Column<int>(type: "integer", nullable: false),
                    InstructorUserId = table.Column<string>(type: "text", nullable: true),
                    IsVacancy = table.Column<bool>(type: "boolean", nullable: false),
                    Places = table.Column<int>(type: "integer", nullable: false),
                    InstructorBasicInfoId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyCardCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyCardCourses_AspNetUsers_InstructorUserId",
                        column: x => x.InstructorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudyCardCourses_CyclePartCourses_CyclePartCourseId",
                        column: x => x.CyclePartCourseId,
                        principalTable: "CyclePartCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyCardCourses_InstructorBasicInfo_InstructorBasicInfoId",
                        column: x => x.InstructorBasicInfoId,
                        principalTable: "InstructorBasicInfo",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudyCardCourses_StudyCards_StudyCardId",
                        column: x => x.StudyCardId,
                        principalTable: "StudyCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentCourses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentCourseRegistrationId = table.Column<int>(type: "integer", nullable: false),
                    StudyCardCourseId = table.Column<int>(type: "integer", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    Comment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsPassed = table.Column<bool>(type: "boolean", nullable: false),
                    Queue = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentCourses_StudentCourseRegistrations_StudentCourseRegi~",
                        column: x => x.StudentCourseRegistrationId,
                        principalTable: "StudentCourseRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentCourses_StudyCardCourses_StudyCardCourseId",
                        column: x => x.StudyCardCourseId,
                        principalTable: "StudyCardCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Code", "ImportCode", "NameEng", "NameKir", "NameRus" },
                values: new object[] { 1, "NA", 0, "Not assigned", "Не указана", "Не указана" });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "ImportCode", "NameEng", "NameKir", "NameRus" },
                values: new object[] { 1, "Na", 0, "Not assigned", "Не указан", "Не указан" });

            migrationBuilder.InsertData(
                table: "Nationalities",
                columns: new[] { "Id", "ImportCode", "NameEng", "NameKir", "NameRus" },
                values: new object[] { 1, 0, "Not assigned", "Не указана", "Не указана" });

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Id", "IsMain", "Name" },
                values: new object[,]
                {
                    { 1, true, "Университет МУЦА" },
                    { 2, false, "Колледж МУЦА" }
                });

            migrationBuilder.InsertData(
                table: "Universities",
                columns: new[] { "Id", "Code", "CountryId", "ImportCode", "NameEng", "NameKir", "NameRus" },
                values: new object[] { 1, "NA", null, 0, "Not assigned", "Не указана", "Не указана" });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Code", "ImportCode", "NameEng", "NameKir", "NameRus", "OrganizationId" },
                values: new object[] { 1, "NA", 0, "Not assigned", null, "Не указано", 1 });

            migrationBuilder.InsertData(
                table: "DepartmentGroups",
                columns: new[] { "Id", "Code", "DepartmentId", "OrganizationId", "Year" },
                values: new object[] { 1, "NA", 1, 1, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicPlans_DepartmentId",
                table: "AcademicPlans",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicPlans_OrganizationId",
                table: "AcademicPlans",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AdviserStudents_OrganizationId",
                table: "AdviserStudents",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AdviserStudents_StudentUserId",
                table: "AdviserStudents",
                column: "StudentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoursePrerequisites_PrerequisiteId",
                table: "CoursePrerequisites",
                column: "PrerequisiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_DepartmentId",
                table: "Courses",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_LanguageId",
                table: "Courses",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_OrganizationId",
                table: "Courses",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_CyclePartCourses_CourseId",
                table: "CyclePartCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CyclePartCourses_CyclePartId",
                table: "CyclePartCourses",
                column: "CyclePartId");

            migrationBuilder.CreateIndex(
                name: "IX_CycleParts_AcademicPlanId",
                table: "CycleParts",
                column: "AcademicPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_CycleParts_CycleId",
                table: "CycleParts",
                column: "CycleId");

            migrationBuilder.CreateIndex(
                name: "IX_DeanAdvisers_AdviserUserId",
                table: "DeanAdvisers",
                column: "AdviserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DeanAdvisers_OrganizationId",
                table: "DeanAdvisers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_DeanDepartments_DepartmentId",
                table: "DeanDepartments",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DeanDepartments_OrganizationId",
                table: "DeanDepartments",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentGroups_DepartmentId",
                table: "DepartmentGroups",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentGroups_OrganizationId",
                table: "DepartmentGroups",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_OrganizationId",
                table: "Departments",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorBasicInfo_InstructorUserId",
                table: "InstructorBasicInfo",
                column: "InstructorUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstructorContactInfo_CitizenshipCountryId",
                table: "InstructorContactInfo",
                column: "CitizenshipCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorContactInfo_CountryId",
                table: "InstructorContactInfo",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorContactInfo_InstructorBasicInfoId",
                table: "InstructorContactInfo",
                column: "InstructorBasicInfoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstructorEducationInfo_EducationTypeId",
                table: "InstructorEducationInfo",
                column: "EducationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorEducationInfo_InstructorBasicInfoId",
                table: "InstructorEducationInfo",
                column: "InstructorBasicInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorEducationInfo_UniversityId",
                table: "InstructorEducationInfo",
                column: "UniversityId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorOrgInfo_DepartmentId",
                table: "InstructorOrgInfo",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorOrgInfo_OrganizationId",
                table: "InstructorOrgInfo",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorOtherJobInfo_InstructorBasicInfoId",
                table: "InstructorOtherJobInfo",
                column: "InstructorBasicInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_SemesterPeriods_OrganizationId",
                table: "SemesterPeriods",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SemesterPeriods_SemesterId",
                table: "SemesterPeriods",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Semesters_OrganizationId",
                table: "Semesters",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffBasicInfo_ApplicationUserId",
                table: "StaffBasicInfo",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentArrears_SemesterId",
                table: "StudentArrears",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentArrears_StudentUserId",
                table: "StudentArrears",
                column: "StudentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentBasicInfo_ApplicationUserId",
                table: "StudentBasicInfo",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentContactInfo_CitizenshipCountryId",
                table: "StudentContactInfo",
                column: "CitizenshipCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentContactInfo_CountryId",
                table: "StudentContactInfo",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentContactInfo_StudentBasicInfoId",
                table: "StudentContactInfo",
                column: "StudentBasicInfoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseGrades_CourseId",
                table: "StudentCourseGrades",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseGrades_GradeId",
                table: "StudentCourseGrades",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseGrades_OrganizationId",
                table: "StudentCourseGrades",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseGrades_StudentUserId",
                table: "StudentCourseGrades",
                column: "StudentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseRegistrations_SemesterId",
                table: "StudentCourseRegistrations",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseRegistrations_StudentUserId",
                table: "StudentCourseRegistrations",
                column: "StudentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourses_StudentCourseRegistrationId",
                table: "StudentCourses",
                column: "StudentCourseRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourses_StudyCardCourseId",
                table: "StudentCourses",
                column: "StudyCardCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLanguages_LanguageId",
                table: "StudentLanguages",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentOrgInfo_DepartmentGroupId",
                table: "StudentOrgInfo",
                column: "DepartmentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentOrgInfo_OrganizationId",
                table: "StudentOrgInfo",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentParentsInfo_StudentBasicInfoId",
                table: "StudentParentsInfo",
                column: "StudentBasicInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCardCourses_CyclePartCourseId",
                table: "StudyCardCourses",
                column: "CyclePartCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCardCourses_InstructorBasicInfoId",
                table: "StudyCardCourses",
                column: "InstructorBasicInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCardCourses_InstructorUserId",
                table: "StudyCardCourses",
                column: "InstructorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCardCourses_StudyCardId",
                table: "StudyCardCourses",
                column: "StudyCardId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCards_CyclePartCourseId",
                table: "StudyCards",
                column: "CyclePartCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCards_DepartmentGroupId",
                table: "StudyCards",
                column: "DepartmentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCards_OrganizationId",
                table: "StudyCards",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCards_SemesterId",
                table: "StudyCards",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferCourses_CyclePartCourseId",
                table: "TransferCourses",
                column: "CyclePartCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferCourses_GradeId",
                table: "TransferCourses",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferCourses_OrganizationId",
                table: "TransferCourses",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferCourses_StudentUserId",
                table: "TransferCourses",
                column: "StudentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferCourses_UniversityId",
                table: "TransferCourses",
                column: "UniversityId");

            migrationBuilder.CreateIndex(
                name: "IX_Universities_CountryId",
                table: "Universities",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBasicInfo_ApplicationUserId",
                table: "UserBasicInfo",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBasicInfo_CitizenshipId",
                table: "UserBasicInfo",
                column: "CitizenshipId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBasicInfo_NationalityId",
                table: "UserBasicInfo",
                column: "NationalityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTypeOrganizations_OrganizationId",
                table: "UserTypeOrganizations",
                column: "OrganizationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdviserStudents");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CoursePrerequisites");

            migrationBuilder.DropTable(
                name: "DeanAdvisers");

            migrationBuilder.DropTable(
                name: "DeanDepartments");

            migrationBuilder.DropTable(
                name: "InstructorContactInfo");

            migrationBuilder.DropTable(
                name: "InstructorEducationInfo");

            migrationBuilder.DropTable(
                name: "InstructorOrgInfo");

            migrationBuilder.DropTable(
                name: "InstructorOtherJobInfo");

            migrationBuilder.DropTable(
                name: "SemesterPeriods");

            migrationBuilder.DropTable(
                name: "StaffBasicInfo");

            migrationBuilder.DropTable(
                name: "StudentArrears");

            migrationBuilder.DropTable(
                name: "StudentContactInfo");

            migrationBuilder.DropTable(
                name: "StudentCourseGrades");

            migrationBuilder.DropTable(
                name: "StudentCourses");

            migrationBuilder.DropTable(
                name: "StudentLanguages");

            migrationBuilder.DropTable(
                name: "StudentOrgInfo");

            migrationBuilder.DropTable(
                name: "StudentParentsInfo");

            migrationBuilder.DropTable(
                name: "TransferCourses");

            migrationBuilder.DropTable(
                name: "UserBasicInfo");

            migrationBuilder.DropTable(
                name: "UserTypeOrganizations");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "EducationTypes");

            migrationBuilder.DropTable(
                name: "StudentCourseRegistrations");

            migrationBuilder.DropTable(
                name: "StudyCardCourses");

            migrationBuilder.DropTable(
                name: "StudentBasicInfo");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "Universities");

            migrationBuilder.DropTable(
                name: "Nationalities");

            migrationBuilder.DropTable(
                name: "InstructorBasicInfo");

            migrationBuilder.DropTable(
                name: "StudyCards");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "CyclePartCourses");

            migrationBuilder.DropTable(
                name: "DepartmentGroups");

            migrationBuilder.DropTable(
                name: "Semesters");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "CycleParts");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "AcademicPlans");

            migrationBuilder.DropTable(
                name: "Cycles");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
