namespace Phoenix.Domain.Catalog.ValueObjects;

/// <summary>
/// Nombre de couleurs d'impression pour une variante de produit.
/// </summary>
/// <remarks>
/// Stocké en base PostgreSQL sous forme de colonne <c>varchar</c> via
/// <c>.HasConversion&lt;string&gt;()</c> dans la configuration EF Core.
/// <br/>
/// <b>Règle métier :</b> <see cref="FourCMYK"/> correspond à une impression en quadrichromie
/// (Cyan, Magenta, Jaune, Noir) et implique un surcoût de photogravure.
/// </remarks>
public enum ColorCount
{
    /// <summary>1 couleur Pantone ou noir.</summary>
    One,

    /// <summary>2 couleurs Pantone.</summary>
    Two,

    /// <summary>3 couleurs Pantone.</summary>
    Three,

    /// <summary>4 couleurs quadrichromie (CMJN / CMYK).</summary>
    FourCMYK
}
