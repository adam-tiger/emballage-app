using MediatR;
using Phoenix.Application.Products.Dtos;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Application.Products.Queries.GetProductFamilies;

/// <summary>
/// Gère la <see cref="GetProductFamiliesQuery"/> : énumère les valeurs de l'enum
/// <see cref="ProductFamily"/> et les mappe en <see cref="ProductFamilyDto"/>.
/// Aucun round-trip base de données.
/// </summary>
public sealed class GetProductFamiliesQueryHandler
    : IRequestHandler<GetProductFamiliesQuery, IReadOnlyList<ProductFamilyDto>>
{
    private static readonly IReadOnlyDictionary<ProductFamily, string> FrenchLabels =
        new Dictionary<ProductFamily, string>
        {
            [ProductFamily.KraftBagHandled]   = "Sac kraft avec anses",
            [ProductFamily.KraftBagSOS]       = "Sac kraft SOS fond carré",
            [ProductFamily.GourmetRange]      = "Gamme gastronomique",
            [ProductFamily.KraftBowl]         = "Bol kraft",
            [ProductFamily.GreaseproofPaper]  = "Papier siliconé antigraisse",
            [ProductFamily.ChirashiBowl]      = "Bol chirashi",
            [ProductFamily.HingedTray]        = "Barquette à charnière",
            [ProductFamily.SushiTray]         = "Plateau sushis",
            [ProductFamily.MicroTray]         = "Plateau micro-ondable",
            [ProductFamily.BioPack]           = "Emballage biodégradable",
            [ProductFamily.PastaPouch]        = "Sachet pâtes / féculents",
            [ProductFamily.SoupPouch]         = "Sachet soupes isolant",
            [ProductFamily.SaucePouch]        = "Sachet sauces",
            [ProductFamily.FriesCone]         = "Cornet frites",
            [ProductFamily.MilkshakeCup]      = "Gobelet milkshake",
            [ProductFamily.CoffeeCup]         = "Gobelet boissons chaudes",
            [ProductFamily.DessertPot]        = "Pot desserts / glaces",
            [ProductFamily.PizzaBox]          = "Boîte pizza",
            [ProductFamily.GreaseproofBag]    = "Sac antigraisse viennoiseries",
            [ProductFamily.SandwichBag]       = "Sachet sandwichs",
            [ProductFamily.ReusableBag]       = "Sac réutilisable",
            [ProductFamily.Napkin]            = "Serviette papier",
            [ProductFamily.WoodenCutlery]     = "Couverts bois",
            [ProductFamily.Bottle]            = "Bouteille",
            [ProductFamily.GarbageBag]        = "Sac poubelle",
            [ProductFamily.FoodWrap]          = "Film alimentaire",
            [ProductFamily.HygieneMisc]       = "Articles hygiène divers",
        };

    /// <inheritdoc/>
    public Task<IReadOnlyList<ProductFamilyDto>> Handle(
        GetProductFamiliesQuery request,
        CancellationToken cancellationToken)
    {
        var result = Enum.GetValues<ProductFamily>()
            .Select(f => new ProductFamilyDto(
                Value: f.ToString(),
                LabelFr: FrenchLabels.TryGetValue(f, out var label) ? label : f.ToString()))
            .ToList();

        return Task.FromResult<IReadOnlyList<ProductFamilyDto>>(result);
    }
}
