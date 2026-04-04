using Microsoft.EntityFrameworkCore;
using Phoenix.Domain.Products.Entities;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Infrastructure.Persistence.DataSeed;

/// <summary>
/// Données de référence initiales pour le module Products &amp; Catalog.
/// Toutes les clés primaires sont déterministes (jamais <c>Guid.NewGuid()</c>).
/// Toutes les dates sont en UTC (<c>DateTimeKind.Utc</c>).
/// </summary>
/// <remarks>
/// <b>Convention de Guid :</b>
/// <list type="bullet">
///   <item>Produits : <c>11111111-00XX-0000-0000-000000000000</c></item>
///   <item>Variantes : <c>22222222-00XX-0001-0000-000000000000</c></item>
///   <item>Paliers : <c>33333333-00XX-00YY-0000-000000000000</c> (XX = produit, YY = palier)</item>
/// </list>
/// </remarks>
internal static class ProductDataSeed
{
    private static readonly DateTime SeedDate =
        new(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc);

    // ── Guid helpers ─────────────────────────────────────────────────────────

    private static Guid ProductId(int n) =>
        new($"11111111-{n:D4}-0000-0000-000000000000");

    private static Guid VariantId(int n) =>
        new($"22222222-{n:D4}-0001-0000-000000000000");

    private static Guid TierId(int product, int tier) =>
        new($"33333333-{product:D4}-{tier:D4}-0000-000000000000");

    // ── Entry point ──────────────────────────────────────────────────────────

    /// <summary>
    /// Injecte les données de référence dans le <see cref="ModelBuilder"/> EF Core.
    /// Appelé depuis <c>PhoenixDbContext.OnModelCreating</c>.
    /// </summary>
    internal static void Seed(ModelBuilder modelBuilder)
    {
        SeedProducts(modelBuilder);
        SeedVariants(modelBuilder);
        SeedPriceTiers(modelBuilder);
    }

    // ── Products (anonymous objects — constructeur privé) ─────────────────────

    private static void SeedProducts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new
            {
                Id = ProductId(1),
                Sku = "SAC-KRAFT-TORSADE-M",
                NameFr = "Sac kraft avec anses torsadées (M)",
                DescriptionFr = "Sac en papier kraft naturel avec anses torsadées. Format médium idéal pour la vente à emporter en restauration et boulangerie.",
                Family = ProductFamily.KraftBagHandled,
                IsCustomizable = true,
                IsGourmetRange = false,
                IsBulkOnly = false,
                IsEcoFriendly = true,
                IsFoodApproved = true,
                SoldByWeight = false,
                HasExpressDelivery = true,
                IsActive = true,
                CreatedAtUtc = SeedDate,
                UpdatedAtUtc = SeedDate
            },
            new
            {
                Id = ProductId(2),
                Sku = "SAC-KRAFT-SOS-S",
                NameFr = "Sac kraft SOS fond carré (S)",
                DescriptionFr = "Sac kraft à fond carré SOS en format small. Stable, robuste, idéal pour boulangeries et épiceries fines.",
                Family = ProductFamily.KraftBagSOS,
                IsCustomizable = true,
                IsGourmetRange = false,
                IsBulkOnly = false,
                IsEcoFriendly = true,
                IsFoodApproved = true,
                SoldByWeight = false,
                HasExpressDelivery = true,
                IsActive = true,
                CreatedAtUtc = SeedDate,
                UpdatedAtUtc = SeedDate
            },
            new
            {
                Id = ProductId(3),
                Sku = "BOL-KRAFT-500ML",
                NameFr = "Bol kraft 500 ml",
                DescriptionFr = "Bol kraft rond 500 ml pour plats chauds type soupe, curry ou poke bowl. Certifié contact alimentaire.",
                Family = ProductFamily.KraftBowl,
                IsCustomizable = true,
                IsGourmetRange = false,
                IsBulkOnly = false,
                IsEcoFriendly = true,
                IsFoodApproved = true,
                SoldByWeight = false,
                HasExpressDelivery = true,
                IsActive = true,
                CreatedAtUtc = SeedDate,
                UpdatedAtUtc = SeedDate
            },
            new
            {
                Id = ProductId(4),
                Sku = "BOL-CHIRASHI-L",
                NameFr = "Bol chirashi rectangulaire (L)",
                DescriptionFr = "Bol chirashi rectangulaire grand format pour sushis, poké et chirashi. Couvercle transparent inclus.",
                Family = ProductFamily.ChirashiBowl,
                IsCustomizable = true,
                IsGourmetRange = true,
                IsBulkOnly = false,
                IsEcoFriendly = false,
                IsFoodApproved = true,
                SoldByWeight = false,
                HasExpressDelivery = false,
                IsActive = true,
                CreatedAtUtc = SeedDate,
                UpdatedAtUtc = SeedDate
            },
            new
            {
                Id = ProductId(5),
                Sku = "BOITE-PIZZA-30",
                NameFr = "Boîte pizza 30 cm",
                DescriptionFr = "Boîte pizza en carton ondulé double cannelure 30 cm. Impression intérieure Food-safe, maintien de chaleur optimisé.",
                Family = ProductFamily.PizzaBox,
                IsCustomizable = true,
                IsGourmetRange = false,
                IsBulkOnly = false,
                IsEcoFriendly = false,
                IsFoodApproved = true,
                SoldByWeight = false,
                HasExpressDelivery = true,
                IsActive = true,
                CreatedAtUtc = SeedDate,
                UpdatedAtUtc = SeedDate
            },
            new
            {
                Id = ProductId(6),
                Sku = "GOB-CAFE-8OZ",
                NameFr = "Gobelet café 8 oz",
                DescriptionFr = "Gobelet en carton double paroi 8 oz (24 cl) pour boissons chaudes. Poignée thermique intégrée.",
                Family = ProductFamily.CoffeeCup,
                IsCustomizable = true,
                IsGourmetRange = false,
                IsBulkOnly = false,
                IsEcoFriendly = false,
                IsFoodApproved = true,
                SoldByWeight = false,
                HasExpressDelivery = true,
                IsActive = true,
                CreatedAtUtc = SeedDate,
                UpdatedAtUtc = SeedDate
            },
            new
            {
                Id = ProductId(7),
                Sku = "PLATEAU-SUSHI-M",
                NameFr = "Plateau sushis 9 pièces",
                DescriptionFr = "Plateau polypropylène avec couvercle transparent pour 9 pièces de sushis. Barquette hermétique, empilable.",
                Family = ProductFamily.SushiTray,
                IsCustomizable = false,
                IsGourmetRange = true,
                IsBulkOnly = false,
                IsEcoFriendly = false,
                IsFoodApproved = true,
                SoldByWeight = false,
                HasExpressDelivery = false,
                IsActive = true,
                CreatedAtUtc = SeedDate,
                UpdatedAtUtc = SeedDate
            },
            new
            {
                Id = ProductId(8),
                Sku = "CORNET-FRITES-M",
                NameFr = "Cornet frites moyen",
                DescriptionFr = "Cornet en papier kraft naturel pour frites et snacks. Format moyen — capacité 220 g. Antigraisse naturel.",
                Family = ProductFamily.FriesCone,
                IsCustomizable = true,
                IsGourmetRange = false,
                IsBulkOnly = false,
                IsEcoFriendly = true,
                IsFoodApproved = true,
                SoldByWeight = false,
                HasExpressDelivery = true,
                IsActive = true,
                CreatedAtUtc = SeedDate,
                UpdatedAtUtc = SeedDate
            },
            new
            {
                Id = ProductId(9),
                Sku = "SACHET-SANDWICH-STD",
                NameFr = "Sachet sandwich standard",
                DescriptionFr = "Sachet kraft pour sandwich, bagel et panini. Ouverture facile, fond plat autoportant.",
                Family = ProductFamily.SandwichBag,
                IsCustomizable = true,
                IsGourmetRange = false,
                IsBulkOnly = false,
                IsEcoFriendly = true,
                IsFoodApproved = true,
                SoldByWeight = false,
                HasExpressDelivery = true,
                IsActive = true,
                CreatedAtUtc = SeedDate,
                UpdatedAtUtc = SeedDate
            },
            new
            {
                Id = ProductId(10),
                Sku = "SERVIETTE-33X33",
                NameFr = "Serviette papier 33×33 cm",
                DescriptionFr = "Serviette en papier ouate 2 plis 33×33 cm. Coloris blanc. Idéale pour la restauration rapide et les food trucks.",
                Family = ProductFamily.Napkin,
                IsCustomizable = true,
                IsGourmetRange = false,
                IsBulkOnly = true,
                IsEcoFriendly = true,
                IsFoodApproved = true,
                SoldByWeight = false,
                HasExpressDelivery = true,
                IsActive = true,
                CreatedAtUtc = SeedDate,
                UpdatedAtUtc = SeedDate
            }
        );
    }

    // ── ProductVariants (constructeur public — validation intégrée) ───────────

    private static void SeedVariants(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductVariant>().HasData(
            new ProductVariant(VariantId(1), ProductId(1), "SAC-KRAFT-TORSADE-M-1C", "Sans impression", 500, PrintSide.SingleSide, ColorCount.One, SeedDate),
            new ProductVariant(VariantId(2), ProductId(2), "SAC-KRAFT-SOS-S-1C", "Sans impression", 250, PrintSide.SingleSide, ColorCount.One, SeedDate),
            new ProductVariant(VariantId(3), ProductId(3), "BOL-KRAFT-500ML-1C", "Sans impression", 250, PrintSide.SingleSide, ColorCount.One, SeedDate),
            new ProductVariant(VariantId(4), ProductId(4), "BOL-CHIRASHI-L-1C", "Sans impression", 100, PrintSide.SingleSide, ColorCount.One, SeedDate),
            new ProductVariant(VariantId(5), ProductId(5), "BOITE-PIZZA-30-2C", "Impression 2 couleurs recto", 50, PrintSide.SingleSide, ColorCount.Two, SeedDate),
            new ProductVariant(VariantId(6), ProductId(6), "GOB-CAFE-8OZ-1C", "Sans impression", 1000, PrintSide.SingleSide, ColorCount.One, SeedDate),
            new ProductVariant(VariantId(7), ProductId(7), "PLATEAU-SUSHI-M-STD", "Standard", 100, PrintSide.SingleSide, ColorCount.One, SeedDate),
            new ProductVariant(VariantId(8), ProductId(8), "CORNET-FRITES-M-1C", "Sans impression", 500, PrintSide.SingleSide, ColorCount.One, SeedDate),
            new ProductVariant(VariantId(9), ProductId(9), "SACHET-SANDWICH-STD-1C", "Sans impression", 500, PrintSide.SingleSide, ColorCount.One, SeedDate),
            new ProductVariant(VariantId(10), ProductId(10), "SERVIETTE-33X33-1C", "Sans impression", 1000, PrintSide.SingleSide, ColorCount.One, SeedDate)
        );
    }

    // ── PriceTiers (constructeur public — 3 paliers par variante) ─────────────

    private static void SeedPriceTiers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PriceTier>().HasData(

            // ── P1 : Sac kraft anses torsadées M ────────────────────────────
            new PriceTier(TierId(1, 1), VariantId(1), 500,  1999, 0.1500m),
            new PriceTier(TierId(1, 2), VariantId(1), 2000, 4999, 0.1200m),
            new PriceTier(TierId(1, 3), VariantId(1), 5000, null, 0.0900m),

            // ── P2 : Sac kraft SOS S ─────────────────────────────────────────
            new PriceTier(TierId(2, 1), VariantId(2), 250,  999,  0.1200m),
            new PriceTier(TierId(2, 2), VariantId(2), 1000, 4999, 0.0900m),
            new PriceTier(TierId(2, 3), VariantId(2), 5000, null, 0.0700m),

            // ── P3 : Bol kraft 500 ml ────────────────────────────────────────
            new PriceTier(TierId(3, 1), VariantId(3), 250,  999,  0.1800m),
            new PriceTier(TierId(3, 2), VariantId(3), 1000, 2999, 0.1400m),
            new PriceTier(TierId(3, 3), VariantId(3), 3000, null, 0.1100m),

            // ── P4 : Bol chirashi L ──────────────────────────────────────────
            new PriceTier(TierId(4, 1), VariantId(4), 100,  499,  0.2200m),
            new PriceTier(TierId(4, 2), VariantId(4), 500,  1999, 0.1800m),
            new PriceTier(TierId(4, 3), VariantId(4), 2000, null, 0.1400m),

            // ── P5 : Boîte pizza 30 cm ───────────────────────────────────────
            new PriceTier(TierId(5, 1), VariantId(5), 50,   199,  0.4500m),
            new PriceTier(TierId(5, 2), VariantId(5), 200,  499,  0.3800m),
            new PriceTier(TierId(5, 3), VariantId(5), 500,  null, 0.3000m),

            // ── P6 : Gobelet café 8 oz ───────────────────────────────────────
            new PriceTier(TierId(6, 1), VariantId(6), 1000, 4999, 0.0800m),
            new PriceTier(TierId(6, 2), VariantId(6), 5000, 9999, 0.0650m),
            new PriceTier(TierId(6, 3), VariantId(6), 10000, null, 0.0520m),

            // ── P7 : Plateau sushis M ────────────────────────────────────────
            new PriceTier(TierId(7, 1), VariantId(7), 100,  499,  0.3500m),
            new PriceTier(TierId(7, 2), VariantId(7), 500,  1999, 0.2800m),
            new PriceTier(TierId(7, 3), VariantId(7), 2000, null, 0.2200m),

            // ── P8 : Cornet frites M ─────────────────────────────────────────
            new PriceTier(TierId(8, 1), VariantId(8), 500,  1999, 0.0600m),
            new PriceTier(TierId(8, 2), VariantId(8), 2000, 4999, 0.0480m),
            new PriceTier(TierId(8, 3), VariantId(8), 5000, null, 0.0380m),

            // ── P9 : Sachet sandwich STD ─────────────────────────────────────
            new PriceTier(TierId(9, 1), VariantId(9), 500,  1999, 0.0700m),
            new PriceTier(TierId(9, 2), VariantId(9), 2000, 4999, 0.0550m),
            new PriceTier(TierId(9, 3), VariantId(9), 5000, null, 0.0420m),

            // ── P10 : Serviette 33×33 ────────────────────────────────────────
            new PriceTier(TierId(10, 1), VariantId(10), 1000,  4999, 0.0400m),
            new PriceTier(TierId(10, 2), VariantId(10), 5000,  9999, 0.0320m),
            new PriceTier(TierId(10, 3), VariantId(10), 10000, null, 0.0250m)
        );
    }
}
