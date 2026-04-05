namespace Phoenix.Application.Customers.Dtos;

/// <summary>
/// DTO du tableau de bord client — résumé de l'activité du client connecté.
/// Retourné par <c>GET /api/v1/customers/me/dashboard</c>.
/// </summary>
/// <param name="CustomerId">Identifiant unique du profil client (domaine).</param>
/// <param name="FullName">Nom complet du client.</param>
/// <param name="TotalOrders">
/// Nombre total de commandes passées.
/// <c>0</c> tant que le Module 5 (Orders) n'est pas développé.
/// </param>
/// <param name="PendingOrders">
/// Nombre de commandes en cours (statut non terminal).
/// <c>0</c> tant que le Module 5 (Orders) n'est pas développé.
/// </param>
/// <param name="TotalQuotes">
/// Nombre total de devis soumis.
/// <c>0</c> tant que le Module 4 (Quotes) n'est pas développé.
/// </param>
/// <param name="PendingQuotes">
/// Nombre de devis en attente de réponse.
/// <c>0</c> tant que le Module 4 (Quotes) n'est pas développé.
/// </param>
/// <param name="DefaultAddress">Adresse de livraison par défaut du client, si définie.</param>
public sealed record CustomerDashboardDto(
    Guid                 CustomerId,
    string               FullName,
    int                  TotalOrders,
    int                  PendingOrders,
    int                  TotalQuotes,
    int                  PendingQuotes,
    CustomerAddressDto?  DefaultAddress);
