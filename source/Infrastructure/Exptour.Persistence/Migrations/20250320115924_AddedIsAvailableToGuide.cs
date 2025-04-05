using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exptour.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsAvailableToGuide : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "GuideAvailabilities");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Guides",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Guides");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "GuideAvailabilities",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
