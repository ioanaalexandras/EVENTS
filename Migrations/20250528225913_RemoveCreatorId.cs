using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazorPagesEvents.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCreatorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_AspNetUsers_CreatorId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_CreatorId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Event");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Event",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_CreatorId",
                table: "Event",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_AspNetUsers_CreatorId",
                table: "Event",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
