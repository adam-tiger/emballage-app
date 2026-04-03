using Phoenix.Application.Products.Dtos;
using Phoenix.Domain.Products.Entities;
using Phoenix.Domain.Products.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace Phoenix.Application.Products.Mappings;

/// <summary>
/// Mapper source-généré (Mapperly) pour le domaine Products.
/// Toute la logique de mapping est résolue à la compilation — zéro réflexion à l'exécution.
/// </summary>
[Mapper]
public sealed partial class ProductMapper
{
    // ── ProductImage → ProductImageDto ────────────────────────────────────

    /// <summary>Mappe une <see cref="ProductImage"/> vers <see cref="ProductImageDto"/>.</summary>
    public partial ProductImageDto ToImageDto(ProductImage image);

    // ── PriceTier → PriceTierDto ──────────────────────────────────────────

    /// <summary>Mappe un <see cref="PriceTier"/> vers <see cref="PriceTierDto"/>.</summary>
    public partial PriceTierDto ToPriceTierDto(PriceTier tier);

    // ── ProductVariant → ProductVariantDto ────────────────────────────────

    /// <summary>
    /// Mappe une <see cref="ProductVariant"/> vers <see cref="ProductVariantDto"/>
    /// en incluant le coefficient d'impression calculé et les paliers tarifaires.
    /// </summary>
    public ProductVariantDto ToVariantDto(ProductVariant variant) => new(
        Id: variant.Id,
        Sku: variant.Sku,
        NameFr: variant.NameFr,
        MinimumOrderQuantity: variant.MinimumOrderQuantity,
        PrintSide: variant.PrintSide.ToString(),
        ColorCount: variant.ColorCount.ToString(),
        PrintCoefficient: variant.CalculatePrintCoefficient(),
        PriceTiers: variant.PriceTiers
            .OrderBy(t => t.MinQuantity)
            .Select(ToPriceTierDto)
            .ToList()
            .AsReadOnly());

    // ── Product → ProductSummaryDto ───────────────────────────────────────

    /// <summary>
    /// Noyau de mapping <see cref="Product"/> → <see cref="ProductSummaryDto"/> généré par Mapperly.
    /// Les propriétés calculées sont ignorées ici et peuplées dans <see cref="ToSummaryDto"/>.
    /// </summary>
    [MapProperty(nameof(Product.Family), nameof(ProductSummaryDto.Family),
        Use = nameof(MapFamilyToString))]
    [MapProperty(nameof(Product.Family), nameof(ProductSummaryDto.FamilyLabel),
        Use = nameof(MapFamilyToLabel))]
    [MapperIgnoreTarget(nameof(ProductSummaryDto.MainImageUrl))]
    [MapperIgnoreTarget(nameof(ProductSummaryDto.MinUnitPriceHT))]
    [MapperIgnoreTarget(nameof(ProductSummaryDto.MinimumOrderQuantity))]
    public partial ProductSummaryDto ToSummaryDtoCore(Product product);

    /// <summary>
    /// Mappe un <see cref="Product"/> vers <see cref="ProductSummaryDto"/>
    /// en incluant toutes les propriétés calculées.
    /// </summary>
    public ProductSummaryDto ToSummaryDto(Product product)
    {
        var dto = ToSummaryDtoCore(product);
        PopulateComputedProperties(product, dto);
        return dto;
    }

    // ── Product → ProductDetailDto ────────────────────────────────────────

    /// <summary>
    /// Noyau de mapping <see cref="Product"/> → <see cref="ProductDetailDto"/> généré par Mapperly.
    /// Les collections et propriétés calculées sont ignorées ici.
    /// </summary>
    [MapProperty(nameof(Product.Family), nameof(ProductDetailDto.Family),
        Use = nameof(MapFamilyToString))]
    [MapProperty(nameof(Product.Family), nameof(ProductDetailDto.FamilyLabel),
        Use = nameof(MapFamilyToLabel))]
    [MapperIgnoreTarget(nameof(ProductDetailDto.MainImageUrl))]
    [MapperIgnoreTarget(nameof(ProductDetailDto.MinUnitPriceHT))]
    [MapperIgnoreTarget(nameof(ProductDetailDto.MinimumOrderQuantity))]
    [MapperIgnoreTarget(nameof(ProductDetailDto.Variants))]
    [MapperIgnoreTarget(nameof(ProductDetailDto.Images))]
    public partial ProductDetailDto ToDetailDtoCore(Product product);

    /// <summary>
    /// Mappe un <see cref="Product"/> vers <see cref="ProductDetailDto"/>
    /// en incluant toutes les collections et propriétés calculées.
    /// </summary>
    public ProductDetailDto ToDetailDto(Product product)
    {
        var dto = ToDetailDtoCore(product);
        PopulateComputedProperties(product, dto);

        dto.Variants = product.Variants
            .Select(ToVariantDto)
            .ToList()
            .AsReadOnly();

        dto.Images = product.Images
            .Select(ToImageDto)
            .ToList()
            .AsReadOnly();

        return dto;
    }

    // ── After-map helper ──────────────────────────────────────────────────

    private static void PopulateComputedProperties(Product product, ProductSummaryDto dto)
    {
        dto.MainImageUrl = product.Images.FirstOrDefault(i => i.IsMain)?.PublicUrl;

        dto.MinimumOrderQuantity = product.Variants.Count > 0
            ? product.Variants.Min(v => v.MinimumOrderQuantity)
            : null;

        dto.MinUnitPriceHT = product.Variants.Count > 0
            ? product.Variants
                .SelectMany(v => v.PriceTiers)
                .Select(t => t.UnitPriceHT)
                .DefaultIfEmpty()
                .Min() is decimal m and > 0 ? m : null
            : null;
    }

    // ── Private converters ────────────────────────────────────────────────

    private static string MapFamilyToString(ProductFamily family) => family.ToString();

    private static string MapFamilyToLabel(ProductFamily family) => family switch
    {
        ProductFamily.KraftBagHandled     => "Sac kraft avec anses",
        ProductFamily.KraftBagSOS         => "Sac kraft SOS fond carré",
        ProductFamily.GourmetRange        => "Gamme gastronomique",
        ProductFamily.KraftBowl           => "Bol kraft",
        ProductFamily.GreaseproofPaper    => "Papier siliconé antigraisse",
        ProductFamily.ChirashiBowl        => "Bol chirashi",
        ProductFamily.HingedTray          => "Barquette à charnière",
        ProductFamily.SushiTray           => "Plateau sushis",
        ProductFamily.MicroTray           => "Plateau micro-ondable",
        ProductFamily.BioPack             => "Emballage biodégradable",
        ProductFamily.PastaPouch          => "Sachet pâtes / féculents",
        ProductFamily.SoupPouch           => "Sachet soupes isolant",
        ProductFamily.SaucePouch          => "Sachet sauces",
        ProductFamily.FriesCone           => "Cornet frites",
        ProductFamily.MilkshakeCup        => "Gobelet milkshake",
        ProductFamily.CoffeeCup           => "Gobelet boissons chaudes",
        ProductFamily.DessertPot          => "Pot desserts / glaces",
        ProductFamily.PizzaBox            => "Boîte pizza",
        ProductFamily.GreaseproofBag      => "Sac antigraisse viennoiseries",
        ProductFamily.SandwichBag         => "Sachet sandwichs",
        ProductFamily.ReusableBag         => "Sac réutilisable",
        ProductFamily.Napkin              => "Serviette papier",
        ProductFamily.WoodenCutlery       => "Couverts bois",
        ProductFamily.Bottle              => "Bouteille",
        ProductFamily.GarbageBag          => "Sac poubelle",
        ProductFamily.FoodWrap            => "Film alimentaire",
        ProductFamily.HygieneMisc         => "Articles hygiène divers",
        _                                 => family.ToString()
    };
}
