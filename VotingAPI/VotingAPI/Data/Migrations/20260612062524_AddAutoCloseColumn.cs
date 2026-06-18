using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAutoCloseColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoClose",
                table: "Elections",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoClose",
                table: "Elections");
        }
    }
}
