using Microsoft.EntityFrameworkCore.Migrations;

namespace TalentAgency.Migrations
{
    public partial class adduserrole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cv_file",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "profile_picture",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "user_role",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user_role",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "cv_file",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "profile_picture",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
