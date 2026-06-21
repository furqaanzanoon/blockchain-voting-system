using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddZKFieldsToVotersAndElections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityCommitment",
                table: "Voters",
                type: "nvarchar(78)",
                maxLength: 78,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MerkleRoot",
                table: "Elections",
                type: "nvarchar(78)",
                maxLength: 78,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VoteTransactions_ElectionId",
                table: "VoteTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Elections_Status",
                table: "Elections");

            migrationBuilder.DropIndex(
                name: "IX_Elections_Status_EndTime",
                table: "Elections");

            migrationBuilder.DropIndex(
                name: "IX_Elections_Status_StartTime",
                table: "Elections");

            migrationBuilder.DropColumn(
                name: "BlockNumber",
                table: "Voters");

            migrationBuilder.DropColumn(
                name: "IdentityCommitment",
                table: "Voters");

            migrationBuilder.DropColumn(
                name: "TxHash",
                table: "Voters");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PartyAffiliation",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MerkleRoot",
                table: "Elections");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Candidates");

            migrationBuilder.AddColumn<Guid>(
                name: "VoterId",
                table: "VoteTransactions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_VoteTransactions_ElectionId_VoterId",
                table: "VoteTransactions",
                columns: new[] { "ElectionId", "VoterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoteTransactions_VoterId",
                table: "VoteTransactions",
                column: "VoterId");

            migrationBuilder.AddForeignKey(
                name: "FK_VoteTransactions_Users_VoterId",
                table: "VoteTransactions",
                column: "VoterId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
