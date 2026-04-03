using MediatR;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Products.Entities;

namespace Phoenix.Application.Products.Commands.UpdateProduct;

/// <summary>
/// Gère la <see cref="UpdateProductCommand"/> : charge le produit, applique la mise à jour et persiste.
/// </summary>
public sealed class UpdateProductCommandHandler(
    IProductRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProductCommand, Unit>
{
    /// <inheritdoc/>
    public async Task<Unit> Handle(
        UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), command.Id);

        product.Update(command.NameFr, command.DescriptionFr);

        await repository.UpdateAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
