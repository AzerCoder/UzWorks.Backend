using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UzWorks.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddConversationSoftDeletePerUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeletedByParticipantOne",
                table: "Conversations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeletedByParticipantTwo",
                table: "Conversations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeletedByParticipantOne",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "IsDeletedByParticipantTwo",
                table: "Conversations");
        }
    }
}
