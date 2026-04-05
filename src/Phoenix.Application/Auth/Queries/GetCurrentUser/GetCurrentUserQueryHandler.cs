using MediatR;
using Microsoft.AspNetCore.Identity;
using Phoenix.Application.Auth.Dtos;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Common.Identity;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Customers.Repositories;

namespace Phoenix.Application.Auth.Queries.GetCurrentUser;

/// <summary>
/// Handler de la requête <see cref="GetCurrentUserQuery"/>.
/// Résout l'utilisateur courant via <see cref="ICurrentUserService"/>
/// et construit le <see cref="UserProfileDto"/> avec les rôles et le profil client.
/// </summary>
public sealed class GetCurrentUserQueryHandler(
    UserManager<ApplicationUser> userManager,
    ICurrentUserService          currentUserService,
    ICustomerRepository          customerRepository)
    : IRequestHandler<GetCurrentUserQuery, UserProfileDto>
{
    /// <summary>Retourne le profil de l'utilisateur authentifié courant.</summary>
    public async Task<UserProfileDto> Handle(GetCurrentUserQuery query, CancellationToken ct)
    {
        // 1. Vérifier l'authentification
        if (currentUserService.UserId is null)
            throw new ForbiddenException("Vous devez être connecté pour accéder à cette ressource.");

        var userId = currentUserService.UserId.Value;

        // 2. Récupérer l'ApplicationUser
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            throw new NotFoundException("User", userId);

        // 3. Récupérer le profil Customer domaine (optionnel — peut être null pour Admin/Employee)
        var customer = await customerRepository.GetByApplicationUserIdAsync(userId, ct);

        // 4. Récupérer les rôles
        var roles = await userManager.GetRolesAsync(user);

        // 5. Construire et retourner le DTO
        return new UserProfileDto(
            Id:          user.Id,
            Email:       user.Email!,
            FirstName:   customer?.FirstName  ?? user.FirstName,
            LastName:    customer?.LastName   ?? user.LastName,
            FullName:    customer?.FullName   ?? user.FullName,
            CompanyName: customer?.CompanyName ?? user.CompanyName,
            Segment:     user.Segment.ToString(),
            Roles:       roles.ToArray(),
            IsActive:    user.IsActive,
            CreatedAtUtc: user.CreatedAtUtc);
    }
}
