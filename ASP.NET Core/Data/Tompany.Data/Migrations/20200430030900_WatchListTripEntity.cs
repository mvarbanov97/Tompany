using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tompany.Data.Migrations
{
    public partial class WatchListTripEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WatchListTrips",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    TripId = table.Column<string>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedOn = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchListTrips", x => new { x.UserId, x.TripId });
                    table.ForeignKey(
                        name: "FK_WatchListTrips_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WatchListTrips_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WatchListTrips_TripId",
                table: "WatchListTrips",
                column: "TripId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WatchListTrips");
        }
    }
}
