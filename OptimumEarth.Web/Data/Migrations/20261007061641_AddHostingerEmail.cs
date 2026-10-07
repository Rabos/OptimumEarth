using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OptimumEarth.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHostingerEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultEmailDestination",
                table: "Settings",
                type: "text",
                nullable: false,
                defaultValue: "Uganda");

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailAttemptUtc",
                table: "Inquiries",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmailDelivery",
                table: "Inquiries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EmailDeliveryError",
                table: "Inquiries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailSentUtc",
                table: "Inquiries",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmailMailboxes",
                columns: table => new
                {
                    Destination = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    ProtectedPassword = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailMailboxes", x => x.Destination);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inquiries_EmailDelivery",
                table: "Inquiries",
                column: "EmailDelivery");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailMailboxes");

            migrationBuilder.DropIndex(
                name: "IX_Inquiries_EmailDelivery",
                table: "Inquiries");

            migrationBuilder.DropColumn(
                name: "DefaultEmailDestination",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "EmailAttemptUtc",
                table: "Inquiries");

            migrationBuilder.DropColumn(
                name: "EmailDelivery",
                table: "Inquiries");

            migrationBuilder.DropColumn(
                name: "EmailDeliveryError",
                table: "Inquiries");

            migrationBuilder.DropColumn(
                name: "EmailSentUtc",
                table: "Inquiries");
        }
    }
}
