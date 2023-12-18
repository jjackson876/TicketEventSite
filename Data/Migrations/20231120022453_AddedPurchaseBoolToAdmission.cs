using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventsAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedPurchaseBoolToAdmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_Events_EventListingId",
                table: "Admissions");

            migrationBuilder.AddColumn<bool>(
                name: "IsNotPurchasable",
                table: "Admissions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_Events_EventListingId",
                table: "Admissions",
                column: "EventListingId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_Events_EventListingId",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "IsNotPurchasable",
                table: "Admissions");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_Events_EventListingId",
                table: "Admissions",
                column: "EventListingId",
                principalTable: "Events",
                principalColumn: "Id");
        }
    }
}
