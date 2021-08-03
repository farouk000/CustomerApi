using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerApi.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackOfficeCertificat",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "FrontOfficeCertificat",
                table: "Customer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackOfficeCertificat",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FrontOfficeCertificat",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
