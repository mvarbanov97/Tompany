using Microsoft.EntityFrameworkCore.Migrations;

namespace Tompany.Data.Migrations
{
    public partial class TripEntityFieldsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripRequest_AspNetUsers_SenderId",
                table: "TripRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_TripRequest_Trips_TripId",
                table: "TripRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TripRequest",
                table: "TripRequest");

            migrationBuilder.RenameTable(
                name: "TripRequest",
                newName: "TripRequests");

            migrationBuilder.RenameIndex(
                name: "IX_TripRequest_TripId",
                table: "TripRequests",
                newName: "IX_TripRequests_TripId");

            migrationBuilder.RenameIndex(
                name: "IX_TripRequest_SenderId",
                table: "TripRequests",
                newName: "IX_TripRequests_SenderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TripRequests",
                table: "TripRequests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TripRequests_AspNetUsers_SenderId",
                table: "TripRequests",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TripRequests_Trips_TripId",
                table: "TripRequests",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripRequests_AspNetUsers_SenderId",
                table: "TripRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TripRequests_Trips_TripId",
                table: "TripRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TripRequests",
                table: "TripRequests");

            migrationBuilder.RenameTable(
                name: "TripRequests",
                newName: "TripRequest");

            migrationBuilder.RenameIndex(
                name: "IX_TripRequests_TripId",
                table: "TripRequest",
                newName: "IX_TripRequest_TripId");

            migrationBuilder.RenameIndex(
                name: "IX_TripRequests_SenderId",
                table: "TripRequest",
                newName: "IX_TripRequest_SenderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TripRequest",
                table: "TripRequest",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TripRequest_AspNetUsers_SenderId",
                table: "TripRequest",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TripRequest_Trips_TripId",
                table: "TripRequest",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
