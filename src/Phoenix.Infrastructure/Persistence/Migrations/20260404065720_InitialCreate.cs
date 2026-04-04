using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Phoenix.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Sku = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NameFr = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DescriptionFr = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false, defaultValue: ""),
                    Family = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsCustomizable = table.Column<bool>(type: "boolean", nullable: false),
                    IsGourmetRange = table.Column<bool>(type: "boolean", nullable: false),
                    IsBulkOnly = table.Column<bool>(type: "boolean", nullable: false),
                    IsEcoFriendly = table.Column<bool>(type: "boolean", nullable: false),
                    IsFoodApproved = table.Column<bool>(type: "boolean", nullable: false),
                    SoldByWeight = table.Column<bool>(type: "boolean", nullable: false),
                    HasExpressDelivery = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "product_images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    BlobPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PublicUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ThumbBlobPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ThumbPublicUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsMain = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_product_images_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_variants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Sku = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    NameFr = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MinimumOrderQuantity = table.Column<int>(type: "integer", nullable: false),
                    PrintSide = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ColorCount = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_variants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_product_variants_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "price_tiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductVariantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MinQuantity = table.Column<int>(type: "integer", nullable: false),
                    MaxQuantity = table.Column<int>(type: "integer", nullable: true),
                    UnitPriceHT = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_price_tiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_price_tiers_product_variants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "product_variants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "Id", "CreatedAtUtc", "DescriptionFr", "Family", "HasExpressDelivery", "IsActive", "IsBulkOnly", "IsCustomizable", "IsEcoFriendly", "IsFoodApproved", "IsGourmetRange", "NameFr", "Sku", "SoldByWeight", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("11111111-0001-0000-0000-000000000000"), new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Sac en papier kraft naturel avec anses torsadées. Format médium idéal pour la vente à emporter en restauration et boulangerie.", "KraftBagHandled", true, true, false, true, true, true, false, "Sac kraft avec anses torsadées (M)", "SAC-KRAFT-TORSADE-M", false, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-0002-0000-0000-000000000000"), new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Sac kraft à fond carré SOS en format small. Stable, robuste, idéal pour boulangeries et épiceries fines.", "KraftBagSOS", true, true, false, true, true, true, false, "Sac kraft SOS fond carré (S)", "SAC-KRAFT-SOS-S", false, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-0003-0000-0000-000000000000"), new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Bol kraft rond 500 ml pour plats chauds type soupe, curry ou poke bowl. Certifié contact alimentaire.", "KraftBowl", true, true, false, true, true, true, false, "Bol kraft 500 ml", "BOL-KRAFT-500ML", false, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-0004-0000-0000-000000000000"), new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Bol chirashi rectangulaire grand format pour sushis, poké et chirashi. Couvercle transparent inclus.", "ChirashiBowl", false, true, false, true, false, true, true, "Bol chirashi rectangulaire (L)", "BOL-CHIRASHI-L", false, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-0005-0000-0000-000000000000"), new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Boîte pizza en carton ondulé double cannelure 30 cm. Impression intérieure Food-safe, maintien de chaleur optimisé.", "PizzaBox", true, true, false, true, false, true, false, "Boîte pizza 30 cm", "BOITE-PIZZA-30", false, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-0006-0000-0000-000000000000"), new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Gobelet en carton double paroi 8 oz (24 cl) pour boissons chaudes. Poignée thermique intégrée.", "CoffeeCup", true, true, false, true, false, true, false, "Gobelet café 8 oz", "GOB-CAFE-8OZ", false, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-0007-0000-0000-000000000000"), new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Plateau polypropylène avec couvercle transparent pour 9 pièces de sushis. Barquette hermétique, empilable.", "SushiTray", false, true, false, false, false, true, true, "Plateau sushis 9 pièces", "PLATEAU-SUSHI-M", false, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-0008-0000-0000-000000000000"), new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Cornet en papier kraft naturel pour frites et snacks. Format moyen — capacité 220 g. Antigraisse naturel.", "FriesCone", true, true, false, true, true, true, false, "Cornet frites moyen", "CORNET-FRITES-M", false, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-0009-0000-0000-000000000000"), new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Sachet kraft pour sandwich, bagel et panini. Ouverture facile, fond plat autoportant.", "SandwichBag", true, true, false, true, true, true, false, "Sachet sandwich standard", "SACHET-SANDWICH-STD", false, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("11111111-0010-0000-0000-000000000000"), new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Serviette en papier ouate 2 plis 33×33 cm. Coloris blanc. Idéale pour la restauration rapide et les food trucks.", "Napkin", true, true, true, true, true, true, false, "Serviette papier 33×33 cm", "SERVIETTE-33X33", false, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "product_variants",
                columns: new[] { "Id", "ColorCount", "CreatedAtUtc", "MinimumOrderQuantity", "NameFr", "PrintSide", "ProductId", "Sku" },
                values: new object[,]
                {
                    { new Guid("22222222-0001-0001-0000-000000000000"), "One", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 500, "Sans impression", "SingleSide", new Guid("11111111-0001-0000-0000-000000000000"), "SAC-KRAFT-TORSADE-M-1C" },
                    { new Guid("22222222-0002-0001-0000-000000000000"), "One", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 250, "Sans impression", "SingleSide", new Guid("11111111-0002-0000-0000-000000000000"), "SAC-KRAFT-SOS-S-1C" },
                    { new Guid("22222222-0003-0001-0000-000000000000"), "One", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 250, "Sans impression", "SingleSide", new Guid("11111111-0003-0000-0000-000000000000"), "BOL-KRAFT-500ML-1C" },
                    { new Guid("22222222-0004-0001-0000-000000000000"), "One", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 100, "Sans impression", "SingleSide", new Guid("11111111-0004-0000-0000-000000000000"), "BOL-CHIRASHI-L-1C" },
                    { new Guid("22222222-0005-0001-0000-000000000000"), "Two", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 50, "Impression 2 couleurs recto", "SingleSide", new Guid("11111111-0005-0000-0000-000000000000"), "BOITE-PIZZA-30-2C" },
                    { new Guid("22222222-0006-0001-0000-000000000000"), "One", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1000, "Sans impression", "SingleSide", new Guid("11111111-0006-0000-0000-000000000000"), "GOB-CAFE-8OZ-1C" },
                    { new Guid("22222222-0007-0001-0000-000000000000"), "One", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 100, "Standard", "SingleSide", new Guid("11111111-0007-0000-0000-000000000000"), "PLATEAU-SUSHI-M-STD" },
                    { new Guid("22222222-0008-0001-0000-000000000000"), "One", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 500, "Sans impression", "SingleSide", new Guid("11111111-0008-0000-0000-000000000000"), "CORNET-FRITES-M-1C" },
                    { new Guid("22222222-0009-0001-0000-000000000000"), "One", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 500, "Sans impression", "SingleSide", new Guid("11111111-0009-0000-0000-000000000000"), "SACHET-SANDWICH-STD-1C" },
                    { new Guid("22222222-0010-0001-0000-000000000000"), "One", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1000, "Sans impression", "SingleSide", new Guid("11111111-0010-0000-0000-000000000000"), "SERVIETTE-33X33-1C" }
                });

            migrationBuilder.InsertData(
                table: "price_tiers",
                columns: new[] { "Id", "MaxQuantity", "MinQuantity", "ProductVariantId", "UnitPriceHT" },
                values: new object[,]
                {
                    { new Guid("33333333-0001-0001-0000-000000000000"), 1999, 500, new Guid("22222222-0001-0001-0000-000000000000"), 0.1500m },
                    { new Guid("33333333-0001-0002-0000-000000000000"), 4999, 2000, new Guid("22222222-0001-0001-0000-000000000000"), 0.1200m },
                    { new Guid("33333333-0001-0003-0000-000000000000"), null, 5000, new Guid("22222222-0001-0001-0000-000000000000"), 0.0900m },
                    { new Guid("33333333-0002-0001-0000-000000000000"), 999, 250, new Guid("22222222-0002-0001-0000-000000000000"), 0.1200m },
                    { new Guid("33333333-0002-0002-0000-000000000000"), 4999, 1000, new Guid("22222222-0002-0001-0000-000000000000"), 0.0900m },
                    { new Guid("33333333-0002-0003-0000-000000000000"), null, 5000, new Guid("22222222-0002-0001-0000-000000000000"), 0.0700m },
                    { new Guid("33333333-0003-0001-0000-000000000000"), 999, 250, new Guid("22222222-0003-0001-0000-000000000000"), 0.1800m },
                    { new Guid("33333333-0003-0002-0000-000000000000"), 2999, 1000, new Guid("22222222-0003-0001-0000-000000000000"), 0.1400m },
                    { new Guid("33333333-0003-0003-0000-000000000000"), null, 3000, new Guid("22222222-0003-0001-0000-000000000000"), 0.1100m },
                    { new Guid("33333333-0004-0001-0000-000000000000"), 499, 100, new Guid("22222222-0004-0001-0000-000000000000"), 0.2200m },
                    { new Guid("33333333-0004-0002-0000-000000000000"), 1999, 500, new Guid("22222222-0004-0001-0000-000000000000"), 0.1800m },
                    { new Guid("33333333-0004-0003-0000-000000000000"), null, 2000, new Guid("22222222-0004-0001-0000-000000000000"), 0.1400m },
                    { new Guid("33333333-0005-0001-0000-000000000000"), 199, 50, new Guid("22222222-0005-0001-0000-000000000000"), 0.4500m },
                    { new Guid("33333333-0005-0002-0000-000000000000"), 499, 200, new Guid("22222222-0005-0001-0000-000000000000"), 0.3800m },
                    { new Guid("33333333-0005-0003-0000-000000000000"), null, 500, new Guid("22222222-0005-0001-0000-000000000000"), 0.3000m },
                    { new Guid("33333333-0006-0001-0000-000000000000"), 4999, 1000, new Guid("22222222-0006-0001-0000-000000000000"), 0.0800m },
                    { new Guid("33333333-0006-0002-0000-000000000000"), 9999, 5000, new Guid("22222222-0006-0001-0000-000000000000"), 0.0650m },
                    { new Guid("33333333-0006-0003-0000-000000000000"), null, 10000, new Guid("22222222-0006-0001-0000-000000000000"), 0.0520m },
                    { new Guid("33333333-0007-0001-0000-000000000000"), 499, 100, new Guid("22222222-0007-0001-0000-000000000000"), 0.3500m },
                    { new Guid("33333333-0007-0002-0000-000000000000"), 1999, 500, new Guid("22222222-0007-0001-0000-000000000000"), 0.2800m },
                    { new Guid("33333333-0007-0003-0000-000000000000"), null, 2000, new Guid("22222222-0007-0001-0000-000000000000"), 0.2200m },
                    { new Guid("33333333-0008-0001-0000-000000000000"), 1999, 500, new Guid("22222222-0008-0001-0000-000000000000"), 0.0600m },
                    { new Guid("33333333-0008-0002-0000-000000000000"), 4999, 2000, new Guid("22222222-0008-0001-0000-000000000000"), 0.0480m },
                    { new Guid("33333333-0008-0003-0000-000000000000"), null, 5000, new Guid("22222222-0008-0001-0000-000000000000"), 0.0380m },
                    { new Guid("33333333-0009-0001-0000-000000000000"), 1999, 500, new Guid("22222222-0009-0001-0000-000000000000"), 0.0700m },
                    { new Guid("33333333-0009-0002-0000-000000000000"), 4999, 2000, new Guid("22222222-0009-0001-0000-000000000000"), 0.0550m },
                    { new Guid("33333333-0009-0003-0000-000000000000"), null, 5000, new Guid("22222222-0009-0001-0000-000000000000"), 0.0420m },
                    { new Guid("33333333-0010-0001-0000-000000000000"), 4999, 1000, new Guid("22222222-0010-0001-0000-000000000000"), 0.0400m },
                    { new Guid("33333333-0010-0002-0000-000000000000"), 9999, 5000, new Guid("22222222-0010-0001-0000-000000000000"), 0.0320m },
                    { new Guid("33333333-0010-0003-0000-000000000000"), null, 10000, new Guid("22222222-0010-0001-0000-000000000000"), 0.0250m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_price_tiers_ProductVariantId",
                table: "price_tiers",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_product_images_ProductId",
                table: "product_images",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_product_variants_ProductId",
                table: "product_variants",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_product_variants_Sku",
                table: "product_variants",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_Sku",
                table: "products",
                column: "Sku",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "price_tiers");

            migrationBuilder.DropTable(
                name: "product_images");

            migrationBuilder.DropTable(
                name: "product_variants");

            migrationBuilder.DropTable(
                name: "products");
        }
    }
}
