namespace Phoenix.Domain.Catalog.ValueObjects;

/// <summary>
/// Face(s) imprimée(s) sur un produit personnalisé.
/// </summary>
/// <remarks>
/// Stocké en base PostgreSQL sous forme de colonne <c>varchar</c> via
/// <c>.HasConversion&lt;string&gt;()</c> dans la configuration EF Core.
/// </remarks>
public enum PrintSide
{
    /// <summary>Impression recto uniquement.</summary>
    SingleSide,

    /// <summary>Impression recto-verso.</summary>
    DoubleSide
}
