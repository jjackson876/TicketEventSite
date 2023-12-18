using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventsAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmissionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    EventListingId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromoImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PromoImage2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PromoImage3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MusicId = table.Column<int>(type: "int", nullable: true),
                    SponsorId = table.Column<int>(type: "int", nullable: true),
                    AdmissionId = table.Column<int>(type: "int", nullable: true),
                    OutletId = table.Column<int>(type: "int", nullable: true),
                    TicketOutletId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Admissions_AdmissionId",
                        column: x => x.AdmissionId,
                        principalTable: "Admissions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Musics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MusicProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventListingId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Musics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Musics_Events_EventListingId",
                        column: x => x.EventListingId,
                        principalTable: "Events",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Outlets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutletName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventListingId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outlets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Outlets_Events_EventListingId",
                        column: x => x.EventListingId,
                        principalTable: "Events",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Sponsors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventSponsor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventListingId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sponsors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sponsors_Events_EventListingId",
                        column: x => x.EventListingId,
                        principalTable: "Events",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_EventListingId",
                table: "Admissions",
                column: "EventListingId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Musics_EventListingId",
                table: "Musics",
                column: "EventListingId");

            migrationBuilder.CreateIndex(
                name: "IX_Outlets_EventListingId",
                table: "Outlets",
                column: "EventListingId");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_EventListingId",
                table: "Sponsors",
                column: "EventListingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_Events_EventListingId",
                table: "Admissions",
                column: "EventListingId",
                principalTable: "Events",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_Events_EventListingId",
                table: "Admissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Musics_Events_EventListingId",
                table: "Musics");

            migrationBuilder.DropForeignKey(
                name: "FK_Outlets_Events_EventListingId",
                table: "Outlets");

            migrationBuilder.DropForeignKey(
                name: "FK_Sponsors_Events_EventListingId",
                table: "Sponsors");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Admissions");

            migrationBuilder.DropTable(
                name: "Musics");

            migrationBuilder.DropTable(
                name: "Outlets");

            migrationBuilder.DropTable(
                name: "Sponsors");
        }
    }
}
