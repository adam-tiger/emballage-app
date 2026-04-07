using MediatR;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Customizations.Exceptions;
using Phoenix.Domain.Customizations.Repositories;
using Phoenix.Domain.Customizations.ValueObjects;

namespace Phoenix.Application.Customizations.Commands.UpdateLogoPosition;

/// <summary>
/// Handler de la commande <see cref="UpdateLogoPositionCommand"/>.
/// Construit le value object <see cref="LogoPosition"/> et délègue
/// la mise à jour à l'agrégat <c>CustomizationJob</c>.
/// </summary>
public sealed class UpdateLogoPositionCommandHandler(
    ICustomizationRepository customizationRepository,
    IUnitOfWork              unitOfWork)
    : IRequestHandler<UpdateLogoPositionCommand, Unit>
{
    /// <summary>
    /// Exécute la mise à jour de la position du logo.
    /// </summary>
    public async Task<Unit> Handle(
        UpdateLogoPositionCommand command,
        CancellationToken         ct)
    {
        // 1. Récupérer le job
        var job = await customizationRepository.GetByIdAsync(command.JobId, ct);
        if (job is null)
            throw new NotFoundException(nameof(job), command.JobId);

        // 2. Construire le value object — lève CustomizationDomainException si invalide
        //    (double filet de sécurité après la validation FluentValidation)
        var position = new LogoPosition(
            positionX: command.PositionX,
            positionY: command.PositionY,
            scaleX:    command.ScaleX,
            scaleY:    command.ScaleY,
            rotation:  command.Rotation);

        // 3. Déléguer à l'agrégat — lève CustomizationDomainException si logo non uploadé
        job.UpdateLogoPosition(position);

        // 4. Persister
        await customizationRepository.UpdateAsync(job, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
