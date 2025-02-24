using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exptour.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingCar_Bookings_BookingId",
                table: "BookingCar");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "BookingCar",
                newName: "BookingsId");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingCar_Bookings_BookingsId",
                table: "BookingCar",
                column: "BookingsId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingCar_Bookings_BookingsId",
                table: "BookingCar");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "BookingsId",
                table: "BookingCar",
                newName: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingCar_Bookings_BookingId",
                table: "BookingCar",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
