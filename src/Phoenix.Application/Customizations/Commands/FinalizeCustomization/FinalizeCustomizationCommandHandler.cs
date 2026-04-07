using MediatR;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Customizations.Dtos;
using Phoenix.Application.Customizations.Mappings;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Customizations.Exceptions;
using Phoenix.Domain.Customizations.Repositories;

namespace Phoenix.Application.Customizations.Commands.FinalizeCustomization;

/// <summary>
/// Handler de la commande <see cref="FinalizeCustomizationCommand"/>.
/// Finalise le job, génère une URL SAS fraîche pour le logo et retourne
/// le <see cref="CustomizationJobDto"/> complet.
/// </summary>
public sealed class FinalizeCustomizationCommandHandler(
    ICustomizationRepository customizationRepository,
    IBlobStorageService      blobStorageService,
    IUnitOfWork              unitOfWork,
    CustomizationMapper      mapper)
    : IRequestHandler<FinalizeCustomizationCommand, CustomizationJobDto>
{
    private const string LogoContainerName = "phoenix-customer-logos";

    /// <summary>
    /// Exécute la finalisation du job de personnalisation en 7 étapes.
    /// </summary>
    public async Task<CustomizationJobDto> Handle(
        FinalizeCustomizationCommand command,
        CancellationToken            ct)
    {
        // 1. Récupérer le job
        var job = await customizationRepository.GetByIdAsync(command.JobId, ct);
        if (job is null)
            throw new NotFoundException(nameof(job), command.JobId);

        // 2. Finaliser — lève CustomizationDomainException si logo non uploadé
        job.Finalize();

        // 3. Persister
        await customizationRepository.UpdateAsync(job, ct);

        // 4. SaveChanges
        await unitOfWork.SaveChangesAsync(ct);

        // 5. Générer une URL SAS fraîche (1h) pour le logo
        string? sasUrl = null;
        if (job.LogoFilePath is not null)
        {
            sasUrl = await blobStorageService.GenerateSasUrlAsync(
                containerName: LogoContainerName,
                blobPath:      job.LogoFilePath,
                duration:      TimeSpan.FromHours(1),
                ct:            ct);
        }

        // 6. Mapper → DTO enrichi avec PrintCoefficient
        var dto = mapper.ToDtoEnriched(job);

        // 7. Enrichir LogoSasUrl
        dto.LogoSasUrl = sasUrl;

        return dto;
    }
}
