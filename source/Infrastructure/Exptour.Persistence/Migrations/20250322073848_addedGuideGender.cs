using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exptour.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addedGuideGender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Guides",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Guides");
        }
    }
}
