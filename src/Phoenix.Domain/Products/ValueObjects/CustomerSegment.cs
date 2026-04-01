namespace Phoenix.Domain.Catalog.ValueObjects;

/// <summary>
/// Segment client cible pour lequel un produit est principalement adapté (10 segments).
/// </summary>
/// <remarks>
/// Stocké en base PostgreSQL sous forme de colonne <c>varchar</c> via
/// <c>.HasConversion&lt;string&gt;()</c> dans la configuration EF Core.
/// </remarks>
public enum CustomerSegment
{
    /// <summary>Artisans et auto-entrepreneurs (volumes très faibles).</summary>
    Artisan,

    /// <summary>Commerce de détail (boutiques, épiceries fines, etc.).</summary>
    CommerceDetail,

    /// <summary>Commerce de gros (cash &amp; carry, négoce).</summary>
    CommerceGros,

    /// <summary>Petites et moyennes entreprises (10–250 salariés).</summary>
    PME,

    /// <summary>Entreprises de taille intermédiaire (250–5 000 salariés).</summary>
    ETI,

    /// <summary>Grands comptes et groupes (> 5 000 salariés).</summary>
    GrandCompte,

    /// <summary>Distributeurs revendant les emballages Phoenix sous leur propre marque.</summary>
    Distributeur,

    /// <summary>Grossistes approvisionnant d'autres revendeurs.</summary>
    Grossiste,

    /// <summary>Revendeurs spécialisés emballage (courtiers, agents).</summary>
    Revendeur,

    /// <summary>Industriels avec besoins de conditionnement récurrents et volumes élevés.</summary>
    Industrie
}
