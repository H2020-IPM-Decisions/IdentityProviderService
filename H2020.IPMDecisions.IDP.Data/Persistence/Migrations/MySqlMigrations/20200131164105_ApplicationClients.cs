using Microsoft.EntityFrameworkCore.Migrations;

namespace H2020.IPMDecisions.IDP.Data.Persistence.Migrations.MySqlMigrations
{
    public partial class ApplicationClients : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationClient",
                columns: table => new
                {
                    ClientId = table.Column<string>(maxLength: 32, nullable: false),
                    Base64Secret = table.Column<string>(maxLength: 80, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ApplicationClientType = table.Column<int>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    RefreshTokenLifeTime = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationClient", x => x.ClientId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationClient");
        }
    }
}
