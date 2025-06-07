using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazorPagesEvents.Migrations
{
    /// <inheritdoc />
    public partial class FixUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "EventUser");

            migrationBuilder.AddForeignKey(
                name: "FK_EventUser_AspNetUsers",
                table: "EventUser",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventUser_AspNetUsers",
                table: "EventUser");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "EventUser",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
