using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace iuca.Infrastructure.Migrations
{
    public partial class AddingPolicy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Policies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameRus = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameEng = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameKir = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DescriptionRus = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    DescriptionEng = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    DescriptionKir = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Policies");
        }
    }
}
