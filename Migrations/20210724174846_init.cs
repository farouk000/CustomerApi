using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerApi.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    BackOfficeUrl = table.Column<string>(nullable: false),
                    BackOfficeSite = table.Column<string>(nullable: false),
                    BackOfficePort = table.Column<int>(nullable: false),
                    BackOfficeProtocol = table.Column<string>(nullable: true),
                    BackOfficeCertificat = table.Column<string>(nullable: true),
                    FrontOfficeUrl = table.Column<string>(nullable: false),
                    FrontOfficeSite = table.Column<string>(nullable: false),
                    FrontOfficePort = table.Column<int>(nullable: false),
                    FrontOfficeProtocol = table.Column<string>(nullable: true),
                    FrontOfficeCertificat = table.Column<string>(nullable: true),
                    DBName = table.Column<string>(nullable: false),
                    BackupFile = table.Column<string>(nullable: false),
                    VersionId = table.Column<int>(nullable: false),
                    VersionNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(maxLength: 100, nullable: true),
                    LastName = table.Column<string>(maxLength: 100, nullable: true),
                    Email = table.Column<string>(nullable: false),
                    Password = table.Column<string>(maxLength: 100, nullable: false),
                    RefreshToken = table.Column<string>(nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Version",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VersionNumber = table.Column<string>(nullable: false),
                    Date = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Version", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Version");
        }
    }
}
