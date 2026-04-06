using MediatR;
using Phoenix.Application.Customers.Dtos;

namespace Phoenix.Application.Customers.Queries.GetDashboard;

/// <summary>
/// Requête retournant le tableau de bord du client authentifié courant.
/// Contient un résumé des commandes, devis et de l'adresse par défaut.
/// </summary>
/// <remarks>
/// Les compteurs de commandes et devis sont à 0 tant que les modules
/// correspondants (Module 4 et 5) ne sont pas développés.
/// </remarks>
public sealed record GetCustomerDashboardQuery : IRequest<CustomerDashboardDto>;
