using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UzWorks.Persistence.Migrations
{
    /// <summary>
    /// Safety migration: adds IsDeletedByParticipantOne/Two to Conversations using
    /// IF NOT EXISTS so it is safe to run even if columns already exist
    /// (handles the case where the previous empty migration was recorded but did nothing).
    /// </summary>
    public partial class EnsureSoftDeleteColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Use raw SQL with IF NOT EXISTS — idempotent, safe to run multiple times.
            migrationBuilder.Sql(@"
                ALTER TABLE ""Conversations""
                ADD COLUMN IF NOT EXISTS ""IsDeletedByParticipantOne"" BOOLEAN NOT NULL DEFAULT FALSE,
                ADD COLUMN IF NOT EXISTS ""IsDeletedByParticipantTwo""  BOOLEAN NOT NULL DEFAULT FALSE;
            ");
        }

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
