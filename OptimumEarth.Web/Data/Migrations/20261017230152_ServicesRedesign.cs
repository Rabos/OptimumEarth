using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OptimumEarth.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class ServicesRedesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AerialBody",
                table: "Settings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AerialHeadline",
                table: "Settings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CaseStudyCaption",
                table: "Settings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CaseStudyStat",
                table: "Settings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CaseStudyStatText",
                table: "Settings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FleetBody",
                table: "Settings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FleetHeadline",
                table: "Settings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FleetImagePath",
                table: "Settings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FleetInsetImagePath",
                table: "Settings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<List<string>>(
                name: "FleetItems",
                table: "Settings",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());

            migrationBuilder.AddColumn<string>(
                name: "ServicesCtaImagePath",
                table: "Settings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServicesHeadline",
                table: "Settings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServicesLede",
                table: "Settings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServicesPosterPath",
                table: "Settings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ShowClientLogos",
                table: "Settings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<List<string>>(
                name: "Highlights",
                table: "Services",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());

            migrationBuilder.AddColumn<string>(
                name: "Layout",
                table: "Services",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PracticeProjectId",
                table: "Services",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortTitle",
                table: "Services",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Services",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StatLabel",
                table: "Services",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StatValue",
                table: "Services",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tagline",
                table: "Services",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TileImagePath",
                table: "Services",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<List<string>>(
                name: "TileItems",
                table: "Services",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());

            migrationBuilder.AddColumn<string>(
                name: "LinkUrl",
                table: "Capabilities",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AerialPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Caption = table.Column<string>(type: "text", nullable: false),
                    ImagePath = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AerialPhotos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LogoPath = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sectors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Blurb = table.Column<string>(type: "text", nullable: false),
                    ImagePath = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Services_PracticeProjectId",
                table: "Services",
                column: "PracticeProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Projects_PracticeProjectId",
                table: "Services",
                column: "PracticeProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Projects_PracticeProjectId",
                table: "Services");

            migrationBuilder.DropTable(
                name: "AerialPhotos");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Sectors");

            migrationBuilder.DropIndex(
                name: "IX_Services_PracticeProjectId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "AerialBody",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "AerialHeadline",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "CaseStudyCaption",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "CaseStudyStat",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "CaseStudyStatText",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FleetBody",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FleetHeadline",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FleetImagePath",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FleetInsetImagePath",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FleetItems",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ServicesCtaImagePath",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ServicesHeadline",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ServicesLede",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ServicesPosterPath",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ShowClientLogos",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Highlights",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Layout",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "PracticeProjectId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ShortTitle",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "StatLabel",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "StatValue",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Tagline",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "TileImagePath",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "TileItems",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "LinkUrl",
                table: "Capabilities");
        }
    }
}
