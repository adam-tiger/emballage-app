using MediatR;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Customizations.Dtos;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Customizations.Repositories;

namespace Phoenix.Application.Customizations.Commands.UploadCustomerLogo;

/// <summary>
/// Handler de la commande <see cref="UploadCustomerLogoCommand"/>.
/// Upload le logo dans Azure Blob Storage, met à jour le job de personnalisation
/// et génère une URL SAS temporaire (1h) pour le preview côté Angular / Konva.js.
/// </summary>
public sealed class UploadCustomerLogoCommandHandler(
    ICustomizationRepository customizationRepository,
    IBlobStorageService      blobStorageService,
    IUnitOfWork              unitOfWork,
    ICurrentUserService      currentUserService)
    : IRequestHandler<UploadCustomerLogoCommand, LogoUploadResponse>
{
    private const string LogoContainerName = "phoenix-customer-logos";

    /// <summary>
    /// Exécute l'upload du logo en 8 étapes.
    /// </summary>
    public async Task<LogoUploadResponse> Handle(
        UploadCustomerLogoCommand command,
        CancellationToken         ct)
    {
        // 1. Récupérer le job
        var job = await customizationRepository.GetByIdAsync(command.JobId, ct);
        if (job is null)
            throw new NotFoundException(nameof(job), command.JobId);

        // 2. Vérifier l'ownership :
        //    Un client connecté ne peut uploader que sur ses propres jobs.
        //    Les jobs invités (CustomerId == null) sont accessibles sans restriction.
        var currentUserId = currentUserService.UserId;
        if (job.CustomerId is not null
            && currentUserId is not null
            && job.CustomerId != currentUserId)
        {
            throw new ForbiddenException(
                "Vous n'êtes pas autorisé à modifier ce job de personnalisation.");
        }

        // 3. Uploader le logo dans Azure Blob Storage
        var uploadResult = await blobStorageService.UploadCustomerLogoAsync(
            customerId: job.CustomerId ?? Guid.Empty,
            jobId:      job.Id,
            stream:     command.FileStream,
            fileName:   command.FileName,
            ct:         ct);

        // 4. Mettre à jour l'agrégat
        job.UploadLogo(
            filePath:    uploadResult.BlobPath,
            fileName:    command.FileName,
            contentType: command.ContentType);

        // 5. Persister
        await customizationRepository.UpdateAsync(job, ct);

        // 6. SaveChanges
        await unitOfWork.SaveChangesAsync(ct);

        // 7. Générer l'URL SAS pour le preview Angular (valide 1h)
        var sasUrl = await blobStorageService.GenerateSasUrlAsync(
            containerName: LogoContainerName,
            blobPath:      uploadResult.BlobPath,
            duration:      TimeSpan.FromHours(1),
            ct:            ct);

        // 8. Retourner la réponse
        return new LogoUploadResponse(
            JobId:        job.Id,
            LogoFilePath: uploadResult.BlobPath,
            LogoSasUrl:   sasUrl,
            LogoFileName: command.FileName,
            Status:       job.Status.ToString());
    }
}
