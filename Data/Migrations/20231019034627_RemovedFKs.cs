using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventsAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Admissions_AdmissionId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Musics_MusicId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Outlets_TicketOutletId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Sponsors_SponsorId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_AdmissionId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_MusicId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_SponsorId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_TicketOutletId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TicketOutletId",
                table: "Events");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TicketOutletId",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_AdmissionId",
                table: "Events",
                column: "AdmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_MusicId",
                table: "Events",
                column: "MusicId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_SponsorId",
                table: "Events",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_TicketOutletId",
                table: "Events",
                column: "TicketOutletId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Admissions_AdmissionId",
                table: "Events",
                column: "AdmissionId",
                principalTable: "Admissions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Musics_MusicId",
                table: "Events",
                column: "MusicId",
                principalTable: "Musics",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Outlets_TicketOutletId",
                table: "Events",
                column: "TicketOutletId",
                principalTable: "Outlets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Sponsors_SponsorId",
                table: "Events",
                column: "SponsorId",
                principalTable: "Sponsors",
                principalColumn: "Id");
        }
    }
}
