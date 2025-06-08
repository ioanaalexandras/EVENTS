using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazorPagesEvents.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToPhotoGallery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PhotoGallery",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhotoGallery_UserId",
                table: "PhotoGallery",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoGallery_AspNetUsers_UserId",
                table: "PhotoGallery",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhotoGallery_AspNetUsers_UserId",
                table: "PhotoGallery");

            migrationBuilder.DropIndex(
                name: "IX_PhotoGallery_UserId",
                table: "PhotoGallery");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PhotoGallery");
        }
    }
}
