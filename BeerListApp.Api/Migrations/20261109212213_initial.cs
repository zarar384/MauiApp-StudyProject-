using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BeerListApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Beers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    AlcoholContent = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Beers",
                columns: new[] { "Id", "AlcoholContent", "Description", "Name" },
                values: new object[,]
                {
                    { 1, 5.0, "A light and refreshing ale with a balanced hop flavor.", "Pale Ale" },
                    { 2, 6.5, "A dark and rich beer with roasted malt flavors.", "Stout" },
                    { 3, 7.0, "An India Pale Ale with a strong hop aroma and bitterness.", "IPA" },
                    { 4, 4.5, "A crisp and clean beer with a smooth finish.", "Lager" },
                    { 5, 4.7999999999999998, "A light and fruity beer made with wheat malt.", "Wheat Beer" },
                    { 6, 4.9000000000000004, "A pale lager with a light body and a slightly bitter taste.", "Pilsner" },
                    { 7, 5.5, "A dark beer with a rich and malty flavor.", "Porter" },
                    { 8, 6.0, "A farmhouse ale with a fruity and spicy character.", "Saison" },
                    { 9, 5.2000000000000002, "A medium-bodied ale with a balanced malt and hop profile.", "Amber Ale" },
                    { 10, 4.7000000000000002, "A light and easy-drinking ale with a mild hop flavor.", "Blonde Ale" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Beers");
        }
    }
}
