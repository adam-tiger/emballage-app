using MediatR;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Products.Entities;

namespace Phoenix.Application.Products.Commands.AddProductVariant;

/// <summary>
/// Gère la <see cref="AddProductVariantCommand"/> : charge le produit, crée et ajoute
/// la variante via l'agrégat, puis persiste.
/// </summary>
public sealed class AddProductVariantCommandHandler(
    IProductRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddProductVariantCommand, Guid>
{
    /// <inheritdoc/>
    public async Task<Guid> Handle(
        AddProductVariantCommand command,
        CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(command.ProductId, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), command.ProductId);

        var variant = new ProductVariant(
            Guid.CreateVersion7(),
            product.Id,
            command.Sku,
            command.NameFr,
            command.MinimumOrderQuantity,
            command.PrintSide,
            command.ColorCount,
            DateTime.UtcNow);

        product.AddVariant(variant);

        await repository.UpdateAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return variant.Id;
    }
}
