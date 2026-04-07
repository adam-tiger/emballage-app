using MediatR;
using Phoenix.Application.Customizations.Dtos;

namespace Phoenix.Application.Customizations.Queries.GetCustomizationJob;

/// <summary>
/// Query pour récupérer un job de personnalisation par son identifiant.
/// Inclut optionnellement une URL SAS fraîche (1h) pour le logo.
/// </summary>
public sealed record GetCustomizationJobQuery : IRequest<CustomizationJobDto>
{
    /// <summary>Identifiant du job de personnalisation à récupérer (obligatoire).</summary>
    public required Guid JobId { get; init; }

    /// <summary>
    /// Si <c>true</c> (défaut), génère une nouvelle URL SAS (1h) pour le logo
    /// afin de permettre l'affichage dans le configurateur Angular.
    /// Si <c>false</c>, retourne le DTO sans SAS URL — plus performant pour
    /// les listes ou les vérifications de statut.
    /// </summary>
    public bool RefreshSasUrl { get; init; } = true;
}
