using MediatR;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Products.Entities;

namespace Phoenix.Application.Products.Commands.DeactivateProduct;

/// <summary>
/// Gère la <see cref="DeactivateProductCommand"/> : charge le produit,
/// appelle <c>Deactivate()</c> et persiste le changement d'état.
/// </summary>
public sealed class DeactivateProductCommandHandler(
    IProductRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeactivateProductCommand, Unit>
{
    /// <inheritdoc/>
    public async Task<Unit> Handle(
        DeactivateProductCommand command,
        CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), command.Id);

        product.Deactivate();

        await repository.UpdateAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
