using MediatR;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Customers.Repositories;

namespace Phoenix.Application.Customers.Commands.SetDefaultAddress;

/// <summary>
/// Handler de la commande <see cref="SetDefaultAddressCommand"/>.
/// Délègue la logique de promotion de l'adresse par défaut à l'agrégat <c>Customer</c>.
/// </summary>
public sealed class SetDefaultAddressCommandHandler(
    ICustomerRepository customerRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork         unitOfWork)
    : IRequestHandler<SetDefaultAddressCommand, Unit>
{
    /// <summary>Définit l'adresse par défaut et retourne <see cref="Unit.Value"/>.</summary>
    public async Task<Unit> Handle(SetDefaultAddressCommand command, CancellationToken ct)
    {
        if (currentUserService.UserId is null)
            throw new ForbiddenException();

        var userId = currentUserService.UserId.Value;

        var customer = await customerRepository.GetByApplicationUserIdAsync(userId, ct)
            ?? throw new NotFoundException("Customer", userId);

        // Délègue la validation (lève CustomerDomainException si introuvable)
        customer.SetDefaultAddress(command.AddressId);

        await customerRepository.UpdateAsync(customer, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
