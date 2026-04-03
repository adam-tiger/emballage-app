namespace Phoenix.Domain.Products.ValueObjects;

// PostgreSQL : stocké en string via HasConversion<string>()

/// <summary>
/// Face(s) imprimée(s) d'un produit personnalisé.
/// Le coefficient appliqué au prix de base est indiqué pour chaque valeur.
/// </summary>
public enum PrintSide
{
    /// <summary>
    /// Impression recto uniquement.
    /// Coefficient de prix : <c>1.00</c> (pas de surcoût).
    /// </summary>
    SingleSide,

    /// <summary>
    /// Impression recto-verso.
    /// Coefficient de prix : <c>1.15</c> (+15 % sur le prix de base).
    /// </summary>
    DoubleSide
}
