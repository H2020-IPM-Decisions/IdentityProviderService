using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.IDP.Data.Persistence.Migrations.MySqlMigrations
{
    public partial class AddLastAccessColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastValidAccess",
                table: "User",
                type: "timestamp",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastValidAccess",
                table: "User");
        }
    }
}
