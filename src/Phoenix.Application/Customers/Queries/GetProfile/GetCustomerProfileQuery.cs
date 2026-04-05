using MediatR;
using Phoenix.Application.Customers.Dtos;

namespace Phoenix.Application.Customers.Queries.GetProfile;

/// <summary>
/// Requête retournant le profil complet du client authentifié courant,
/// incluant ses adresses de livraison.
/// </summary>
public sealed record GetCustomerProfileQuery : IRequest<CustomerProfileDto>;
