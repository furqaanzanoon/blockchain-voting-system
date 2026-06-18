using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAutoActivateColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoActivate",
                table: "Elections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AutoActivateFailReason",
                table: "Elections",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoActivate",
                table: "Elections");

            migrationBuilder.DropColumn(
                name: "AutoActivateFailReason",
                table: "Elections");
        }
    }
}
