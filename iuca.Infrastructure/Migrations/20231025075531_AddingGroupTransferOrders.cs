using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingGroupTransferOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupTransferOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentUserId = table.Column<string>(type: "text", nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    SemesterId = table.Column<int>(type: "integer", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Comment = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SourceGroupId = table.Column<int>(type: "integer", nullable: false),
                    TargetGroupId = table.Column<int>(type: "integer", nullable: false),
                    IsApplied = table.Column<bool>(type: "boolean", nullable: false),
                    PreviousAdvisorsJson = table.Column<IEnumerable<string>>(type: "jsonb", nullable: true),
                    FutureAdvisorsJson = table.Column<IEnumerable<string>>(type: "jsonb", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: true),
                    ModifiedById = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupTransferOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupTransferOrders_AspNetUsers_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupTransferOrders_DepartmentGroups_SourceGroupId",
                        column: x => x.SourceGroupId,
                        principalTable: "DepartmentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupTransferOrders_DepartmentGroups_TargetGroupId",
                        column: x => x.TargetGroupId,
                        principalTable: "DepartmentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupTransferOrders_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupTransferOrders_Semesters_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semesters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupTransferOrders_OrganizationId",
                table: "GroupTransferOrders",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupTransferOrders_SemesterId",
                table: "GroupTransferOrders",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupTransferOrders_SourceGroupId",
                table: "GroupTransferOrders",
                column: "SourceGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupTransferOrders_StudentUserId",
                table: "GroupTransferOrders",
                column: "StudentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupTransferOrders_TargetGroupId",
                table: "GroupTransferOrders",
                column: "TargetGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupTransferOrders");
        }
    }
}
