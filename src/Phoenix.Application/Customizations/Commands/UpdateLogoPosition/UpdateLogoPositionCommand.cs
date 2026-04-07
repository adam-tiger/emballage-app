using MediatR;

namespace Phoenix.Application.Customizations.Commands.UpdateLogoPosition;

/// <summary>
/// Commande de mise à jour de la position et de la transformation du logo
/// sur le canvas 2D du configurateur.
/// Requiert que le logo ait déjà été uploadé (statut ≠ Draft).
/// </summary>
public sealed record UpdateLogoPositionCommand : IRequest<Unit>
{
    /// <summary>Identifiant du job de personnalisation à mettre à jour (obligatoire).</summary>
    public required Guid JobId { get; init; }

    /// <summary>Position horizontale du centre du logo en % du canvas (0-100).</summary>
    public required decimal PositionX { get; init; }

    /// <summary>Position verticale du centre du logo en % du canvas (0-100).</summary>
    public required decimal PositionY { get; init; }

    /// <summary>Facteur d'échelle horizontal du logo (0.1 – 3.0).</summary>
    public required decimal ScaleX { get; init; }

    /// <summary>Facteur d'échelle vertical du logo (0.1 – 3.0).</summary>
    public required decimal ScaleY { get; init; }

    /// <summary>Rotation du logo en degrés (-180 à 180).</summary>
    public required decimal Rotation { get; init; }
}
