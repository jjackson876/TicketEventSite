using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventsAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class MinorChangesToAdmissionEvHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_Events_EventListingId",
                table: "Admissions");

            migrationBuilder.RenameColumn(
                name: "EventListingId",
                table: "Admissions",
                newName: "EventHeaderId");

            migrationBuilder.RenameIndex(
                name: "IX_Admissions_EventListingId",
                table: "Admissions",
                newName: "IX_Admissions_EventHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_Events_EventHeaderId",
                table: "Admissions",
                column: "EventHeaderId",
                principalTable: "Events",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_Events_EventHeaderId",
                table: "Admissions");

            migrationBuilder.RenameColumn(
                name: "EventHeaderId",
                table: "Admissions",
                newName: "EventListingId");

            migrationBuilder.RenameIndex(
                name: "IX_Admissions_EventHeaderId",
                table: "Admissions",
                newName: "IX_Admissions_EventListingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_Events_EventListingId",
                table: "Admissions",
                column: "EventListingId",
                principalTable: "Events",
                principalColumn: "Id");
        }
    }
}
