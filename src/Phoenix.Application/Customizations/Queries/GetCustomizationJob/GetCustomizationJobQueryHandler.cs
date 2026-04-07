using MediatR;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Customizations.Dtos;
using Phoenix.Application.Customizations.Mappings;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Customizations.Repositories;

namespace Phoenix.Application.Customizations.Queries.GetCustomizationJob;

/// <summary>
/// Handler de la query <see cref="GetCustomizationJobQuery"/>.
/// Récupère le job, vérifie l'ownership et enrichit le DTO avec
/// une URL SAS fraîche si demandé.
/// </summary>
public sealed class GetCustomizationJobQueryHandler(
    ICustomizationRepository customizationRepository,
    IBlobStorageService      blobStorageService,
    CustomizationMapper      mapper,
    ICurrentUserService      currentUserService)
    : IRequestHandler<GetCustomizationJobQuery, CustomizationJobDto>
{
    private const string LogoContainerName = "phoenix-customer-logos";

    /// <summary>
    /// Exécute la récupération du job de personnalisation.
    /// </summary>
    public async Task<CustomizationJobDto> Handle(
        GetCustomizationJobQuery query,
        CancellationToken        ct)
    {
        // 1. Récupérer le job
        var job = await customizationRepository.GetByIdAsync(query.JobId, ct);
        if (job is null)
            throw new NotFoundException(nameof(job), query.JobId);

        // 2. Vérifier ownership si le job appartient à un client identifié
        var currentUserId = currentUserService.UserId;
        if (job.CustomerId is not null
            && currentUserId is not null
            && job.CustomerId != currentUserId)
        {
            throw new ForbiddenException(
                "Vous n'êtes pas autorisé à accéder à ce job de personnalisation.");
        }

        // 3. Mapper → DTO enrichi avec PrintCoefficient
        var dto = mapper.ToDtoEnriched(job);

        // 4. Générer une URL SAS fraîche si demandé et si un logo est présent
        if (query.RefreshSasUrl && job.LogoFilePath is not null)
        {
            dto.LogoSasUrl = await blobStorageService.GenerateSasUrlAsync(
                containerName: LogoContainerName,
                blobPath:      job.LogoFilePath,
                duration:      TimeSpan.FromHours(1),
                ct:            ct);
        }

        return dto;
    }
}
