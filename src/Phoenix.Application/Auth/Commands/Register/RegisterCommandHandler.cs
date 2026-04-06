using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Phoenix.Application.Auth.Dtos;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Common.Identity;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Customers.Entities;
using Phoenix.Domain.Customers.Repositories;

namespace Phoenix.Application.Auth.Commands.Register;

/// <summary>
/// Handler de la commande <see cref="RegisterCommand"/>.
/// Crée le compte Identity, le profil Customer, génère les tokens et envoie l'email de bienvenue.
/// </summary>
public sealed class RegisterCommandHandler(
    UserManager<ApplicationUser>  userManager,
    RoleManager<ApplicationRole>  roleManager,
    ICustomerRepository           customerRepository,
    IJwtTokenService              jwtTokenService,
    IRefreshTokenStore            refreshTokenStore,
    IUnitOfWork                   unitOfWork,
    IEmailService                 emailService)
    : IRequestHandler<RegisterCommand, AuthResponse>
{
    /// <summary>Exécute l'inscription en 10 étapes atomiques.</summary>
    public async Task<AuthResponse> Handle(RegisterCommand command, CancellationToken ct)
    {
        // 1. Email non existant
        var existing = await userManager.FindByEmailAsync(command.Email);
        if (existing is not null)
            throw new ValidationException(
            [
                new ValidationFailure("Email", "Un compte existe déjà avec cette adresse e-mail.")
            ]);

        // 2. Créer ApplicationUser
        var user = new ApplicationUser
        {
            Email          = command.Email,
            UserName       = command.Email,
            FirstName      = command.FirstName,
            LastName       = command.LastName,
            CompanyName    = command.CompanyName,
            Segment        = command.Segment,
            CreatedAtUtc   = DateTime.UtcNow,
            IsActive       = true,
            EmailConfirmed = true   // auto-confirmé à l'inscription B2B
        };

        // 3. Persister le compte Identity avec le mot de passe hashé
        var createResult = await userManager.CreateAsync(user, command.Password);
        if (!createResult.Succeeded)
            throw new ValidationException(
                createResult.Errors.Select(e =>
                    new ValidationFailure("Identity", e.Description)));

        // 4. Attribuer le rôle Customer (créer s'il n'existe pas)
        if (!await roleManager.RoleExistsAsync("Customer"))
            await roleManager.CreateAsync(new ApplicationRole("Customer"));

        await userManager.AddToRoleAsync(user, "Customer");

        // 5. Créer le Customer domaine
        var customer = Customer.Create(
            applicationUserId: Guid.Parse(user.Id),
            firstName:         command.FirstName,
            lastName:          command.LastName,
            email:             command.Email,
            companyName:       command.CompanyName,
            segment:           command.Segment);

        // Lier l'ApplicationUser au Customer domaine
        user.CustomerId = customer.Id;
        await userManager.UpdateAsync(user);

        // 6-7. Persister Customer + SaveChanges
        await customerRepository.AddAsync(customer, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // 8. Générer les tokens JWT
        var roles  = await userManager.GetRolesAsync(user);
        var claims = new UserClaims(
            UserId:    user.Id,
            Email:     user.Email!,
            FirstName: user.FirstName,
            LastName:  user.LastName,
            Roles:     roles.ToArray());

        var tokens = jwtTokenService.GenerateTokens(claims);

        // 9. Stocker le refresh token
        await refreshTokenStore.StoreAsync(new RefreshTokenData(
            Token:     tokens.RefreshToken,
            UserId:    user.Id,
            ExpiresAt: tokens.RefreshTokenExpiresAt,
            IsRevoked: false,
            CreatedAt: DateTime.UtcNow), ct);

        // 10. Email de bienvenue (best-effort, ne bloque pas la réponse)
        _ = emailService.SendWelcomeEmailAsync(user.Email!, user.FirstName, ct)
                        .ContinueWith(_ => { }, TaskContinuationOptions.OnlyOnFaulted);

        // 11. Retourner AuthResponse
        var profile = new UserProfileDto(
            Id:          user.Id,
            Email:       user.Email!,
            FirstName:   user.FirstName,
            LastName:    user.LastName,
            FullName:    user.FullName,
            CompanyName: user.CompanyName,
            Segment:     user.Segment.ToString(),
            Roles:       roles.ToArray(),
            IsActive:    user.IsActive,
            CreatedAtUtc: user.CreatedAtUtc);

        var expiresIn = (int)(tokens.AccessTokenExpiresAt - DateTime.UtcNow).TotalSeconds;
        return new AuthResponse(tokens.AccessToken, tokens.RefreshToken, expiresIn, profile);
    }
}
