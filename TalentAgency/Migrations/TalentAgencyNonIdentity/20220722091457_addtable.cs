using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TalentAgency.Migrations.TalentAgencyNonIdentity
{
    public partial class addtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Apply",
                columns: table => new
                {
                    Apply_id = table.Column<string>(nullable: false),
                    Talent_id = table.Column<string>(nullable: true),
                    Event_id = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Introduction = table.Column<string>(nullable: false),
                    Date_created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apply", x => x.Apply_id);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Event_id = table.Column<string>(nullable: false),
                    Event_name = table.Column<string>(nullable: false),
                    email = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: false),
                    date_created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Event_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Apply");

            migrationBuilder.DropTable(
                name: "Event");
        }
    }
}
