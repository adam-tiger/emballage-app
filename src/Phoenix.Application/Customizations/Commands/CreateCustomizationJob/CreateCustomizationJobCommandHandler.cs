using MediatR;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Customizations.Dtos;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Customizations.Entities;
using Phoenix.Domain.Customizations.Repositories;

namespace Phoenix.Application.Customizations.Commands.CreateCustomizationJob;

/// <summary>
/// Handler de la commande <see cref="CreateCustomizationJobCommand"/>.
/// Crée un nouveau job de personnalisation ou retourne le job actif existant
/// pour éviter les doublons (idempotence).
/// </summary>
public sealed class CreateCustomizationJobCommandHandler(
    ICustomizationRepository customizationRepository,
    IProductRepository       productRepository,
    IUnitOfWork              unitOfWork,
    ICurrentUserService      currentUserService)
    : IRequestHandler<CreateCustomizationJobCommand, CreateCustomizationJobResponse>
{
    /// <summary>
    /// Exécute la création du job en 7 étapes.
    /// </summary>
    public async Task<CreateCustomizationJobResponse> Handle(
        CreateCustomizationJobCommand command,
        CancellationToken             ct)
    {
        // 1. Vérifier que le produit existe
        var product = await productRepository.GetByIdAsync(command.ProductId, ct);
        if (product is null)
            throw new NotFoundException(nameof(product), command.ProductId);

        // 2. Résoudre le CustomerId
        //    → command.CustomerId fourni explicitement → utiliser celui-là
        //    → Sinon → currentUserService.UserId (null si invité)
        var customerId = command.CustomerId ?? currentUserService.UserId;

        // 3. Si un job actif existe déjà pour ce client + cette variante → retourner l'existant
        if (customerId is not null)
        {
            var existing = await customizationRepository
                .GetActiveJobForVariantAsync(customerId.Value, command.ProductVariantId, ct);

            if (existing is not null)
                return ToResponse(existing);
        }

        // 4. Créer le job via la factory domain
        var job = CustomizationJob.Create(
            productId:        command.ProductId,
            productVariantId: command.ProductVariantId,
            customerId:       customerId);

        // 5. Persister
        await customizationRepository.AddAsync(job, ct);

        // 6. SaveChanges
        await unitOfWork.SaveChangesAsync(ct);

        // 7. Retourner la réponse
        return ToResponse(job);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static CreateCustomizationJobResponse ToResponse(CustomizationJob job) =>
        new(
            JobId:            job.Id,
            ProductId:        job.ProductId,
            ProductVariantId: job.ProductVariantId,
            CustomerId:       job.CustomerId,
            Status:           job.Status.ToString(),
            PrintCoefficient: job.CalculatePrintCoefficient(),
            PrintSide:        job.PrintSide.ToString(),
            ColorCount:       job.ColorCount.ToString());
}
