using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.IDP.Data.Persistence.Migrations.MySqlMigrations
{
    public partial class InactiveEmailsColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InactiveEmailsSent",
                table: "User",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InactiveEmailsSent",
                table: "User");
        }
    }
}
