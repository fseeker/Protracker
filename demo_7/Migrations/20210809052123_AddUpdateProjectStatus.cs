using Microsoft.EntityFrameworkCore.Migrations;

namespace demo_7.Migrations
{
    public partial class AddUpdateProjectStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "UpdateProjects",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "UpdateProjects");
        }
    }
}
