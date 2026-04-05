using MediatR;
using Phoenix.Application.Auth.Dtos;

namespace Phoenix.Application.Auth.Queries.GetCurrentUser;

/// <summary>
/// Requête retournant le profil complet de l'utilisateur authentifié courant.
/// L'identité est résolue depuis <c>ICurrentUserService</c> dans le handler.
/// </summary>
public sealed record GetCurrentUserQuery : IRequest<UserProfileDto>;
