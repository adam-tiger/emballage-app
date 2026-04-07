using MediatR;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Customizations.Repositories;

namespace Phoenix.Application.Customizations.Commands.UpdatePrintOptions;

/// <summary>
/// Handler de la commande <see cref="UpdatePrintOptionsCommand"/>.
/// Met à jour les options d'impression (face et couleurs) du job de personnalisation.
/// </summary>
public sealed class UpdatePrintOptionsCommandHandler(
    ICustomizationRepository customizationRepository,
    IUnitOfWork              unitOfWork)
    : IRequestHandler<UpdatePrintOptionsCommand, Unit>
{
    /// <summary>
    /// Exécute la mise à jour des options d'impression.
    /// </summary>
    public async Task<Unit> Handle(
        UpdatePrintOptionsCommand command,
        CancellationToken         ct)
    {
        // 1. Récupérer le job
        var job = await customizationRepository.GetByIdAsync(command.JobId, ct);
        if (job is null)
            throw new NotFoundException(nameof(job), command.JobId);

        // 2. Déléguer à l'agrégat
        job.UpdatePrintOptions(command.PrintSide, command.ColorCount);

        // 3. Persister
        await customizationRepository.UpdateAsync(job, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
