using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exptour.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addedGuideStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Guides",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Guides");
        }
    }
}
