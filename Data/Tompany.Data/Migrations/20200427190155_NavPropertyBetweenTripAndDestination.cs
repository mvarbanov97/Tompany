using Microsoft.EntityFrameworkCore.Migrations;

namespace Tompany.Data.Migrations
{
    public partial class NavPropertyBetweenTripAndDestination : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromCity",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ToCity",
                table: "Trips");

            migrationBuilder.AddColumn<string>(
                name: "FromDestinationName",
                table: "Trips",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToDestinationName",
                table: "Trips",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromDestinationName",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ToDestinationName",
                table: "Trips");

            migrationBuilder.AddColumn<string>(
                name: "FromCity",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToCity",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
