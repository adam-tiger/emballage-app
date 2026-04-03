using System.Globalization;

namespace Phoenix.Domain.Products.ValueObjects;

/// <summary>
/// Value object immuable représentant un montant monétaire hors-taxes.
/// </summary>
/// <remarks>
/// Stocké en PostgreSQL via <c>OwnsOne</c> EF Core sous deux colonnes :
/// <list type="bullet">
///   <item><c>Amount</c> → <c>numeric(18,4)</c></item>
///   <item><c>Currency</c> → <c>varchar(3)</c> — code ISO 4217</item>
/// </list>
/// </remarks>
/// <param name="Amount">Montant (doit être >= 0).</param>
/// <param name="Currency">Code devise ISO 4217, ex : "EUR" (défaut).</param>
public record Money(decimal Amount, string Currency = "EUR")
{
    /// <summary>
    /// Valide que le montant est positif ou nul lors de la construction.
    /// La redéclaration de la propriété positionnelle permet l'injection de validation.
    /// </summary>
    public decimal Amount { get; init; } = Amount >= 0
        ? Amount
        : throw new ArgumentOutOfRangeException(nameof(Amount),
            $"Le montant ne peut pas être négatif (valeur fournie : {Amount}).");

    // ── Méthodes statiques ──────────────────────────────────────────────────

    /// <summary>Retourne un montant zéro en euros.</summary>
    public static Money Zero() => new(0m, "EUR");

    // ── Opérateurs ──────────────────────────────────────────────────────────

    /// <summary>
    /// Additionne deux montants de même devise.
    /// </summary>
    /// <exception cref="InvalidOperationException">Si les devises diffèrent.</exception>
    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException(
                $"Impossible d'additionner des montants de devises différentes : {a.Currency} ≠ {b.Currency}.");
        return a with { Amount = a.Amount + b.Amount };
    }

    /// <summary>
    /// Multiplie un montant par un coefficient scalaire.
    /// </summary>
    /// <param name="money">Montant de base.</param>
    /// <param name="factor">Coefficient multiplicateur (doit être >= 0).</param>
    /// <exception cref="ArgumentOutOfRangeException">Si le coefficient est négatif.</exception>
    public static Money operator *(Money money, decimal factor)
    {
        if (factor < 0)
            throw new ArgumentOutOfRangeException(nameof(factor),
                "Le coefficient multiplicateur ne peut pas être négatif.");
        return money with { Amount = Math.Round(money.Amount * factor, 4) };
    }

    // ── Méthodes d'instance ─────────────────────────────────────────────────

    /// <summary>
    /// Calcule le montant TTC en appliquant la TVA.
    /// </summary>
    /// <param name="taxRate">Taux de TVA (défaut : 0.20 = 20 %).</param>
    /// <returns>Nouveau <see cref="Money"/> représentant le montant TTC arrondi à 4 décimales.</returns>
    public Money WithTax(decimal taxRate = 0.20m) =>
        this with { Amount = Math.Round(Amount * (1 + taxRate), 4) };

    // ── Affichage ───────────────────────────────────────────────────────────

    /// <summary>
    /// Retourne une représentation lisible en format français (ex : "12,50 €").
    /// </summary>
    public override string ToString() =>
        $"{Amount.ToString("N2", CultureInfo.GetCultureInfo("fr-FR"))} €";
}
