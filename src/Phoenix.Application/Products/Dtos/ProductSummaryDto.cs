namespace Phoenix.Application.Products.Dtos;

/// <summary>
/// Représentation allégée d'un produit utilisée dans les réponses de liste catalogue.
/// </summary>
public class ProductSummaryDto
{
    /// <summary>Identifiant du produit.</summary>
    public Guid Id { get; set; }

    /// <summary>Référence SKU unique du produit.</summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>Libellé commercial en français.</summary>
    public string NameFr { get; set; } = string.Empty;

    /// <summary>Nom de membre de l'enum <c>ProductFamily</c>.</summary>
    public string Family { get; set; } = string.Empty;

    /// <summary>Libellé français de la famille produit.</summary>
    public string FamilyLabel { get; set; } = string.Empty;

    /// <summary>Le produit accepte une personnalisation par impression client.</summary>
    public bool IsCustomizable { get; set; }

    /// <summary>Appartient à la gamme gastronomique/traiteur premium.</summary>
    public bool IsGourmetRange { get; set; }

    /// <summary>Certifié éco-responsable.</summary>
    public bool IsEcoFriendly { get; set; }

    /// <summary>Certifié contact alimentaire (CE 1935/2004).</summary>
    public bool IsFoodApproved { get; set; }

    /// <summary>Vendu au poids plutôt qu'à l'unité.</summary>
    public bool SoldByWeight { get; set; }

    /// <summary>Disponible en livraison express J+1.</summary>
    public bool HasExpressDelivery { get; set; }

    /// <summary>Le produit est visible et commandable dans le catalogue public.</summary>
    public bool IsActive { get; set; }

    /// <summary>URL CDN de l'image principale du produit, si définie.</summary>
    public string? MainImageUrl { get; set; }

    /// <summary>
    /// Prix unitaire HT minimum parmi tous les paliers de toutes les variantes.
    /// <c>null</c> si aucun palier n'est défini.
    /// </summary>
    public decimal? MinUnitPriceHT { get; set; }

    /// <summary>
    /// MOQ minimum parmi toutes les variantes du produit.
    /// <c>null</c> si aucune variante n'est définie.
    /// </summary>
    public int? MinimumOrderQuantity { get; set; }
}
