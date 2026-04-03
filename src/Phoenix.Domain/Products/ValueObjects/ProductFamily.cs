namespace Phoenix.Domain.Products.ValueObjects;

// PostgreSQL : stocké en string via HasConversion<string>()

/// <summary>
/// Famille de produits du catalogue Phoenix Emballage (27 familles).
/// Chaque valeur correspond à une ligne de produits avec ses caractéristiques propres.
/// </summary>
public enum ProductFamily
{
    /// <summary>Sac kraft avec anses torsadées — Sac à anses torsadées.</summary>
    KraftBagHandled,

    /// <summary>Sac kraft SOS à fond carré — Sac SOS fond carré.</summary>
    KraftBagSOS,

    /// <summary>Gamme gastronomique premium — Gamme traiteur.</summary>
    GourmetRange,

    /// <summary>Bol kraft rond pour plats chauds — Bol kraft.</summary>
    KraftBowl,

    /// <summary>Papier siliconé antigraisse — Papier antigraisse.</summary>
    GreaseproofPaper,

    /// <summary>Bol chirashi rectangulaire pour sushis/poké — Bol chirashi.</summary>
    ChirashiBowl,

    /// <summary>Barquette à charnière en carton — Barquette charnière.</summary>
    HingedTray,

    /// <summary>Plateau à sushis avec couvercle transparent — Plateau sushis.</summary>
    SushiTray,

    /// <summary>Plateau micro-ondable pour plats préparés — Plateau micro.</summary>
    MicroTray,

    /// <summary>Emballage biodégradable certifié compostable — Emballage bio.</summary>
    BioPack,

    /// <summary>Sachet kraft pour pâtes et féculents — Sachet pâtes.</summary>
    PastaPouch,

    /// <summary>Sachet isolant pour soupes et potages — Sachet soupe.</summary>
    SoupPouch,

    /// <summary>Sachet pour sauces et condiments — Sachet sauce.</summary>
    SaucePouch,

    /// <summary>Cornet en papier pour frites et snacks — Cornet frites.</summary>
    FriesCone,

    /// <summary>Gobelet isolant pour milkshakes — Gobelet milkshake.</summary>
    MilkshakeCup,

    /// <summary>Gobelet en carton pour boissons chaudes — Gobelet café.</summary>
    CoffeeCup,

    /// <summary>Pot en carton pour desserts et glaces — Pot dessert.</summary>
    DessertPot,

    /// <summary>Boîte à pizza en carton ondulé — Boîte pizza.</summary>
    PizzaBox,

    /// <summary>Sac antigraisse pour viennoiseries et beignets — Sac antigraisse.</summary>
    GreaseproofBag,

    /// <summary>Sachet pour sandwichs et bagels — Sachet sandwich.</summary>
    SandwichBag,

    /// <summary>Sac réutilisable en coton ou non-tissé — Sac réutilisable.</summary>
    ReusableBag,

    /// <summary>Serviette en papier pour couverts — Serviette.</summary>
    Napkin,

    /// <summary>Couverts en bois (fourchette, couteau, cuillère) — Couverts bois.</summary>
    WoodenCutlery,

    /// <summary>Bouteille en plastique ou verre pour boissons — Bouteille.</summary>
    Bottle,

    /// <summary>Sac poubelle pour restauration — Sac poubelle.</summary>
    GarbageBag,

    /// <summary>Film alimentaire pour emballage comptoir — Film alimentaire.</summary>
    FoodWrap,

    /// <summary>Articles d'hygiène divers (gants, charlotte, tablier…) — Hygiène divers.</summary>
    HygieneMisc
}
