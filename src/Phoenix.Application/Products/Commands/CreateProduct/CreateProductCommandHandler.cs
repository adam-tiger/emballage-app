using MediatR;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Products.Entities;

namespace Phoenix.Application.Products.Commands.CreateProduct;

/// <summary>
/// Gère la <see cref="CreateProductCommand"/> : vérifie l'unicité du SKU,
/// crée l'agrégat Product, persiste et retourne son identifiant.
/// </summary>
public sealed class CreateProductCommandHandler(
    IProductRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateProductCommand, Guid>
{
    /// <inheritdoc/>
    public async Task<Guid> Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        if (await repository.ExistsAsync(command.Sku, cancellationToken))
            throw new ValidationException(
                [new FluentValidation.Results.ValidationFailure(
                    nameof(CreateProductCommand.Sku),
                    $"Le SKU '{command.Sku}' est déjà utilisé dans le catalogue.")]);

        var product = Product.Create(
            command.Sku,
            command.NameFr,
            command.Family,
            command.IsCustomizable,
            command.IsGourmetRange,
            command.IsBulkOnly,
            command.IsEcoFriendly,
            command.IsFoodApproved,
            command.SoldByWeight,
            command.HasExpressDelivery);

        // La description n'est pas dans Create() — on l'applique via Update si fournie.
        if (!string.IsNullOrWhiteSpace(command.DescriptionFr))
            product.Update(command.NameFr, command.DescriptionFr);

        await repository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
