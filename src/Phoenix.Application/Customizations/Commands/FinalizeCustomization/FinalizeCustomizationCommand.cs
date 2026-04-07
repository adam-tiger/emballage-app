using MediatR;
using Phoenix.Application.Customizations.Dtos;

namespace Phoenix.Application.Customizations.Commands.FinalizeCustomization;

/// <summary>
/// Commande de finalisation d'un job de personnalisation.
/// Passe le job au statut <c>ReadyForOrder</c> — il peut ensuite être
/// référencé dans une <c>OrderLine</c> au Module 5.
/// Requiert que le logo ait été uploadé au préalable (statut <c>LogoUploaded</c>).
/// </summary>
public sealed record FinalizeCustomizationCommand : IRequest<CustomizationJobDto>
{
    /// <summary>Identifiant du job de personnalisation à finaliser (obligatoire).</summary>
    public required Guid JobId { get; init; }
}
