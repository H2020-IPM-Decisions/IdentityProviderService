using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.IDP.Data.Persistence.Migrations.MySqlMigrations
{
    public partial class ChangeIdToGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationClient",
                table: "ApplicationClient");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ApplicationClient");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ApplicationClient",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationClient",
                table: "ApplicationClient",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationClient",
                table: "ApplicationClient");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ApplicationClient");

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "ApplicationClient",
                type: "varchar(32) CHARACTER SET utf8mb4",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationClient",
                table: "ApplicationClient",
                column: "ClientId");
        }
    }
}
