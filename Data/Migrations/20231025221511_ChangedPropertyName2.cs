using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventsAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedPropertyName2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_Events_EventHeaderId",
                table: "Admissions");

            migrationBuilder.DropIndex(
                name: "IX_Admissions_EventHeaderId",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "EventHeaderId",
                table: "Admissions");

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_EventListingId",
                table: "Admissions",
                column: "EventListingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_Events_EventListingId",
                table: "Admissions",
                column: "EventListingId",
                principalTable: "Events",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_Events_EventListingId",
                table: "Admissions");

            migrationBuilder.DropIndex(
                name: "IX_Admissions_EventListingId",
                table: "Admissions");

            migrationBuilder.AddColumn<int>(
                name: "EventHeaderId",
                table: "Admissions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_EventHeaderId",
                table: "Admissions",
                column: "EventHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_Events_EventHeaderId",
                table: "Admissions",
                column: "EventHeaderId",
                principalTable: "Events",
                principalColumn: "Id");
        }
    }
}
