using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.IDP.Data.Persistence.Migrations.MySqlMigrations
{
    public partial class ChangeColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "ApplicationClient");

            migrationBuilder.AddColumn<string>(
                name: "JWTAudienceCategory",
                table: "ApplicationClient",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JWTAudienceCategory",
                table: "ApplicationClient");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "ApplicationClient",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                defaultValue: "");
        }
    }
}
