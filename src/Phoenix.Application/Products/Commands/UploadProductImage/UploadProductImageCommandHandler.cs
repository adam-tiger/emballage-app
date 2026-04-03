using MediatR;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Products.Dtos;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Products.Entities;

namespace Phoenix.Application.Products.Commands.UploadProductImage;

/// <summary>
/// Gère la <see cref="UploadProductImageCommand"/> : upload l'image vers le blob storage
/// via <see cref="IBlobStorageService.UploadProductImageAsync"/>, crée l'entité
/// <see cref="ProductImage"/> via <c>product.AddImage</c> et persiste.
/// </summary>
public sealed class UploadProductImageCommandHandler(
    IProductRepository repository,
    IBlobStorageService blobStorageService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UploadProductImageCommand, UploadProductImageResponse>
{
    /// <inheritdoc/>
    public async Task<UploadProductImageResponse> Handle(
        UploadProductImageCommand command,
        CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(command.ProductId, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), command.ProductId);

        var uploadResult = await blobStorageService.UploadProductImageAsync(
            command.ProductId,
            command.FileStream,
            command.FileName,
            cancellationToken);

        var image = new ProductImage(
            Guid.CreateVersion7(),
            product.Id,
            uploadResult.BlobPath,
            uploadResult.PublicUrl,
            isMain: command.SetAsMain,
            createdAtUtc: DateTime.UtcNow,
            thumbBlobPath: uploadResult.ThumbBlobPath,
            thumbPublicUrl: uploadResult.ThumbPublicUrl);

        product.AddImage(image);

        await repository.UpdateAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UploadProductImageResponse(
            product.Id,
            image.Id,
            uploadResult.PublicUrl,
            uploadResult.ThumbPublicUrl);
    }
}
