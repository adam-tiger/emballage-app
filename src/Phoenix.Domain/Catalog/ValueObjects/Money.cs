namespace Phoenix.Domain.Catalog.ValueObjects;

/// <summary>
/// Value object immuable représentant un montant monétaire hors-taxes.
/// </summary>
/// <remarks>
/// Stocké en base PostgreSQL sous forme de deux colonnes via <c>OwnsOne</c> EF Core :
/// <list type="bullet">
///   <item><c>Amount</c> → <c>numeric(18,4)</c></item>
///   <item><c>Currency</c> → <c>varchar(3)</c> (code ISO 4217, ex : "EUR")</item>
/// </list>
/// Ce value object est intentionnellement en euros par défaut conformément
/// à la politique commerciale Phoenix (marché France / UE).
/// </remarks>
public record Money
{
    /// <summary>Montant zéro en euros. Utile comme valeur par défaut.</summary>
    public static readonly Money Zero = new(0m, "EUR");

    /// <summary>Montant du prix (>= 0).</summary>
    public decimal Amount { get; init; }

    /// <summary>Code devise ISO 4217 (ex : "EUR"). Toujours en majuscules.</summary>
    public string Currency { get; init; }

    /// <summary>
    /// Constructeur principal avec validation des invariants.
    /// </summary>
    /// <param name="amount">Montant (doit être >= 0).</param>
    /// <param name="currency">Code devise ISO 4217 (défaut : "EUR").</param>
    public Money(decimal amount, string currency = "EUR")
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount),
                $"Le montant ne peut pas être négatif (valeur fournie : {amount}).");

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Le code devise ne peut pas être vide.", nameof(currency));

        if (currency.Trim().Length != 3)
            throw new ArgumentException(
                $"Le code devise doit contenir exactement 3 caractères ISO 4217 (fourni : '{currency}').",
                nameof(currency));

        Amount = amount;
        Currency = currency.Trim().ToUpperInvariant();
    }

    /// <summary>
    /// Constructeur sans paramètre réservé à EF Core.
    /// Ne jamais appeler directement dans le code métier.
    /// </summary>
    private Money()
    {
        Amount = 0;
        Currency = "EUR";
    }

    // ----- Opérations arithmétiques -----

    /// <summary>Addition de deux montants de même devise.</summary>
    /// <exception cref="InvalidOperationException">Si les devises diffèrent.</exception>
    public static Money operator +(Money left, Money right)
    {
        GuardSameCurrency(left, right);
        return new Money(left.Amount + right.Amount, left.Currency);
    }

    /// <summary>Soustraction de deux montants de même devise (résultat >= 0).</summary>
    /// <exception cref="InvalidOperationException">Si les devises diffèrent.</exception>
    /// <exception cref="InvalidOperationException">Si le résultat serait négatif.</exception>
    public static Money operator -(Money left, Money right)
    {
        GuardSameCurrency(left, right);
        if (left.Amount < right.Amount)
            throw new InvalidOperationException(
                $"La soustraction produirait un montant négatif ({left} - {right}).");
        return new Money(left.Amount - right.Amount, left.Currency);
    }

    /// <summary>Multiplication par un coefficient scalaire (>= 0).</summary>
    public static Money operator *(Money money, decimal multiplier)
    {
        if (multiplier < 0)
            throw new ArgumentOutOfRangeException(nameof(multiplier),
                "Le multiplicateur ne peut pas être négatif.");
        return new Money(money.Amount * multiplier, money.Currency);
    }

    /// <summary>Multiplication (ordre inverse).</summary>
    public static Money operator *(decimal multiplier, Money money) => money * multiplier;

    // ----- Comparaison -----

    public static bool operator >(Money left, Money right)
    {
        GuardSameCurrency(left, right);
        return left.Amount > right.Amount;
    }

    public static bool operator <(Money left, Money right)
    {
        GuardSameCurrency(left, right);
        return left.Amount < right.Amount;
    }

    public static bool operator >=(Money left, Money right)
    {
        GuardSameCurrency(left, right);
        return left.Amount >= right.Amount;
    }

    public static bool operator <=(Money left, Money right)
    {
        GuardSameCurrency(left, right);
        return left.Amount <= right.Amount;
    }

    // ----- Affichage -----

    /// <summary>Retourne une représentation lisible (ex : "12,50 EUR").</summary>
    public override string ToString() => $"{Amount:F2} {Currency}";

    // ----- Helpers privés -----

    private static void GuardSameCurrency(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException(
                $"Impossible d'opérer sur des devises différentes : {left.Currency} ≠ {right.Currency}.");
    }
}
