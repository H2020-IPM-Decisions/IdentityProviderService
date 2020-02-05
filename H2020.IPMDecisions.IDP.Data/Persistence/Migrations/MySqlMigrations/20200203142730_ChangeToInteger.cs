using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.IDP.Data.Persistence.Migrations.MySqlMigrations
{
    public partial class ChangeToInteger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RefreshTokenLifeTime",
                table: "ApplicationClient",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ApplicationClient",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(32)",
                oldMaxLength: 32);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "RefreshTokenLifeTime",
                table: "ApplicationClient",
                type: "double",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ApplicationClient",
                type: "char(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(Guid));
        }
    }
}
