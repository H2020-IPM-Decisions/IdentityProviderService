using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.IDP.Data.Persistence.Migrations.MySqlMigrations
{
    public partial class fixingMissingColum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "RefreshToken",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
               name: "IX_RefreshToken_UserId",
               table: "RefreshToken",
               column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RefreshToken");

            migrationBuilder.DropIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshToken");
        }
    }
}
