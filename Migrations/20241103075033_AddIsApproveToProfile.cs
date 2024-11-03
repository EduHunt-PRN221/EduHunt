using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eduhunt.Migrations
{
    /// <inheritdoc />
    public partial class AddIsApproveToProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApprove",
                table: "Profile",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApprove",
                table: "Profile");
        }
    }
}
