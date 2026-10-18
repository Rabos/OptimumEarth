using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OptimumEarth.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProjectsRedesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProjectsCtaImagePath",
                table: "Settings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProjectsHeadline",
                table: "Settings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProjectsLede",
                table: "Settings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProjectsStatFigure",
                table: "Settings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProjectsStatLabel",
                table: "Settings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Client",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FlagshipFigure",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FlagshipLabel",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FlagshipOrder",
                table: "Projects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Ongoing",
                table: "Projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RepresentativePhoto",
                table: "Projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Sector",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<List<string>>(
                name: "Services",
                table: "Projects",
                type: "text[]",
                nullable: false,
                defaultValue: new List<string>());

            migrationBuilder.CreateTable(
                name: "FieldPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kicker = table.Column<string>(type: "text", nullable: false),
                    Caption = table.Column<string>(type: "text", nullable: false),
                    ImagePath = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldPhotos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldPhotos");

            migrationBuilder.DropColumn(
                name: "ProjectsCtaImagePath",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ProjectsHeadline",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ProjectsLede",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ProjectsStatFigure",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ProjectsStatLabel",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Client",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "FlagshipFigure",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "FlagshipLabel",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "FlagshipOrder",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Ongoing",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "RepresentativePhoto",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Sector",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Services",
                table: "Projects");
        }
    }
}
