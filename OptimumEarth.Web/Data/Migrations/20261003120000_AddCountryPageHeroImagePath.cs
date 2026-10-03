using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using OptimumEarth.Web.Data;

#nullable disable

namespace OptimumEarth.Web.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20261003120000_AddCountryPageHeroImagePath")]
    public partial class AddCountryPageHeroImagePath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeroImagePath",
                table: "CountryPages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE "CountryPages"
                SET "HeroImagePath" = '/img/optimum-earth-images/heroslide1.jpg'
                WHERE "Slug" = 'uganda' AND "HeroImagePath" = '';
                """);

            migrationBuilder.Sql("""
                UPDATE "CountryPages"
                SET "HeroImagePath" = '/img/optimum-earth-images/heroslide2.jpg'
                WHERE "Slug" = 'zambia' AND "HeroImagePath" = '';
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeroImagePath",
                table: "CountryPages");
        }
    }
}
