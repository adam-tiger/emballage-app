using MediatR;
using Phoenix.Domain.Products.ValueObjects;

namespace Phoenix.Application.Customizations.Commands.UpdatePrintOptions;

/// <summary>
/// Commande de mise à jour des options d'impression d'un job de personnalisation.
/// Peut être appelée à tout moment du cycle de vie du job (Draft, LogoUploaded, ReadyForOrder).
/// </summary>
public sealed record UpdatePrintOptionsCommand : IRequest<Unit>
{
    /// <summary>Identifiant du job de personnalisation à mettre à jour (obligatoire).</summary>
    public required Guid JobId { get; init; }

    /// <summary>Face(s) à imprimer (recto ou recto-verso).</summary>
    public required PrintSide PrintSide { get; init; }

    /// <summary>Nombre de couleurs d'impression (1, 2, 3 ou 4 couleurs CMJN).</summary>
    public required ColorCount ColorCount { get; init; }
}
