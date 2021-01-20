using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tompany.Data.Migrations
{
    public partial class AddedDestinationentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FromDestinationId",
                table: "Trips",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToDestinationId",
                table: "Trips",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Destinations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Region = table.Column<string>(nullable: true),
                    Population = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destinations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trips_FromDestinationId",
                table: "Trips",
                column: "FromDestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_ToDestinationId",
                table: "Trips",
                column: "ToDestinationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Destinations_FromDestinationId",
                table: "Trips",
                column: "FromDestinationId",
                principalTable: "Destinations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Destinations_ToDestinationId",
                table: "Trips",
                column: "ToDestinationId",
                principalTable: "Destinations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Destinations_FromDestinationId",
                table: "Trips");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Destinations_ToDestinationId",
                table: "Trips");

            migrationBuilder.DropTable(
                name: "Destinations");

            migrationBuilder.DropIndex(
                name: "IX_Trips_FromDestinationId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_ToDestinationId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "FromDestinationId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ToDestinationId",
                table: "Trips");
        }
    }
}
