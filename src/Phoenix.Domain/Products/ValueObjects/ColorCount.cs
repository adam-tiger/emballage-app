namespace Phoenix.Domain.Products.ValueObjects;

// PostgreSQL : stocké en string via HasConversion<string>()

/// <summary>
/// Nombre de couleurs d'impression d'une variante de produit.
/// Le coefficient appliqué au prix de base est indiqué pour chaque valeur.
/// </summary>
public enum ColorCount
{
    /// <summary>
    /// 1 couleur Pantone ou noir.
    /// Coefficient de prix : <c>1.00</c> (pas de surcoût).
    /// </summary>
    One,

    /// <summary>
    /// 2 couleurs Pantone.
    /// Coefficient de prix : <c>1.10</c> (+10 %).
    /// </summary>
    Two,

    /// <summary>
    /// 3 couleurs Pantone.
    /// Coefficient de prix : <c>1.18</c> (+18 %).
    /// </summary>
    Three,

    /// <summary>
    /// 4 couleurs quadrichromie CMJN (Cyan, Magenta, Jaune, Noir).
    /// Coefficient de prix : <c>1.25</c> (+25 %) — implique un surcoût de photogravure.
    /// </summary>
    FourCMYK
}
