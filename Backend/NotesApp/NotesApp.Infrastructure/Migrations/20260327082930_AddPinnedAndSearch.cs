using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotesApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPinnedAndSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Pinned",
                table: "Notes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pinned",
                table: "Notes");
        }
    }
}
