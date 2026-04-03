using MediatR;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Products.Entities;

namespace Phoenix.Application.Products.Commands.AddPriceTier;

/// <summary>
/// Gère la <see cref="AddPriceTierCommand"/> : charge le produit, localise la variante,
/// crée et ajoute le palier tarifaire (qui vérifie l'absence de chevauchement) puis persiste.
/// </summary>
public sealed class AddPriceTierCommandHandler(
    IProductRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddPriceTierCommand, Guid>
{
    /// <inheritdoc/>
    public async Task<Guid> Handle(
        AddPriceTierCommand command,
        CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(command.ProductId, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), command.ProductId);

        var variant = product.Variants.FirstOrDefault(v => v.Id == command.VariantId)
            ?? throw new NotFoundException(nameof(ProductVariant), command.VariantId);

        var tier = new PriceTier(
            Guid.CreateVersion7(),
            variant.Id,
            command.MinQuantity,
            command.MaxQuantity,
            command.UnitPriceHT);

        variant.AddPriceTier(tier);

        await repository.UpdateAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return tier.Id;
    }
}
