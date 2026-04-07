using MediatR;
using Phoenix.Application.Customizations.Dtos;

namespace Phoenix.Application.Customizations.Commands.CreateCustomizationJob;

/// <summary>
/// Commande de création d'un job de personnalisation.
/// Si un job actif (Draft ou LogoUploaded) existe déjà pour la combinaison
/// client + variante, le job existant est retourné sans doublon.
/// </summary>
public sealed record CreateCustomizationJobCommand : IRequest<CreateCustomizationJobResponse>
{
    /// <summary>Identifiant du produit du catalogue (obligatoire).</summary>
    public required Guid ProductId { get; init; }

    /// <summary>Identifiant de la variante produit sélectionnée (obligatoire).</summary>
    public required Guid ProductVariantId { get; init; }

    /// <summary>
    /// Identifiant du client propriétaire.
    /// <c>null</c> pour les sessions invitées (configurateur sans compte).
    /// Si <c>null</c> et que l'utilisateur est connecté, le handler utilisera
    /// <c>ICurrentUserService.UserId</c>.
    /// </summary>
    public Guid? CustomerId { get; init; }
}
