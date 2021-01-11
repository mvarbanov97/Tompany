using Microsoft.EntityFrameworkCore.Migrations;

namespace Tompany.Data.Migrations
{
    public partial class AddUserRatings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRatings",
                columns: table => new
                {
                    Username = table.Column<string>(nullable: false),
                    RaterUsername = table.Column<string>(nullable: false),
                    Stars = table.Column<int>(maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRatings", x => new { x.Username, x.RaterUsername });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRatings");
        }
    }
}
