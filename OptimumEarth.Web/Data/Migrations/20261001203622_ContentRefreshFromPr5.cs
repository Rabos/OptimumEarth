using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OptimumEarth.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class ContentRefreshFromPr5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContentVersion",
                table: "Settings",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<List<string>>(
                name: "Deliverables",
                table: "Services",
                type: "text[]",
                nullable: false,
                // Existing rows need a value; without a default Postgres refuses to add a NOT NULL column.
                defaultValueSql: "'{}'");

            migrationBuilder.AddColumn<string>(
                name: "DocumentPath",
                table: "SdgGoals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "CountryPageServices",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentVersion",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Deliverables",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "DocumentPath",
                table: "SdgGoals");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "CountryPageServices");
        }
    }
}
