namespace Phoenix.Domain.Catalog.ValueObjects;

/// <summary>
/// Famille de produits du catalogue Phoenix (27 familles).
/// </summary>
/// <remarks>
/// Stocké en base PostgreSQL sous forme de colonne <c>varchar</c> via
/// <c>.HasConversion&lt;string&gt;()</c> dans la configuration EF Core.
/// Toute nouvelle famille doit être ajoutée ici ET dans la migration correspondante.
/// </remarks>
public enum ProductFamily
{
    /// <summary>Boîtes pliantes en carton.</summary>
    BoitePliante,

    /// <summary>Boîtes rigides (carton contrecollé).</summary>
    BoiteRigide,

    /// <summary>Boîtes d'expédition / caisses américaines.</summary>
    BoiteExpedition,

    /// <summary>Sacs papier à anses torsadées.</summary>
    SacPapierTorsade,

    /// <summary>Sacs papier à anses plates.</summary>
    SacPapierPlat,

    /// <summary>Sacs en plastique (PEBD, PP, etc.).</summary>
    SacPlastique,

    /// <summary>Sachets refermables avec zip.</summary>
    SachetZip,

    /// <summary>Sachets stand-up (Doypack).</summary>
    SachetStandUp,

    /// <summary>Sachets cellophane / OPP transparents.</summary>
    SachetCellophane,

    /// <summary>Enveloppes matelassées (bulles d'air ou kraft moulé).</summary>
    EnveloppeMatelassee,

    /// <summary>Enveloppes kraft auto-adhésives.</summary>
    EnveloppeKraft,

    /// <summary>Tubes en carton (ronds ou ovales).</summary>
    TubeCarton,

    /// <summary>Étuis carton plat (sleeve / fourreaux).</summary>
    EtuiCarton,

    /// <summary>Pochettes de présentation / intercalaires.</summary>
    PochettePresentation,

    /// <summary>Papier de soie pour garnissage.</summary>
    PapierTissu,

    /// <summary>Papier kraft en feuille ou en rouleau.</summary>
    PapierKraft,

    /// <summary>Ruban adhésif transparent, kraft ou coloré.</summary>
    RubanAdhesif,

    /// <summary>Ruban imprimé personnalisé (logo, texte).</summary>
    RubanImprime,

    /// <summary>Étiquettes autocollantes (fond blanc, kraft, holographiques…).</summary>
    EtiquetteAutocollante,

    /// <summary>Film étirable pour palettisation.</summary>
    FilmEtirable,

    /// <summary>Film rétractable thermorétractable.</summary>
    FilmRetractable,

    /// <summary>Mousses et calages de protection (mousse PE, mousse polyuréthane).</summary>
    MousseCalage,

    /// <summary>Papier bulle (film à bulles d'air).</summary>
    PapierBulle,

    /// <summary>Intercalaires et alvéoles carton.</summary>
    IntercalaireCarton,

    /// <summary>Carton ondulé simple ou double cannelure.</summary>
    CartonOndule,

    /// <summary>Plateaux de présentation et présentoirs.</summary>
    PlateauPresentation,

    /// <summary>Accessoires d'emballage divers (sangles, boucles, coins, etc.).</summary>
    Accessoire
}
