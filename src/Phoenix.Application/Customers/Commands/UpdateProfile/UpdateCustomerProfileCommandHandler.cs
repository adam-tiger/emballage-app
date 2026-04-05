using MediatR;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Customers.Repositories;

namespace Phoenix.Application.Customers.Commands.UpdateProfile;

/// <summary>
/// Handler de la commande <see cref="UpdateCustomerProfileCommand"/>.
/// Récupère le Customer courant, applique les modifications et persiste.
/// </summary>
public sealed class UpdateCustomerProfileCommandHandler(
    ICustomerRepository customerRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork         unitOfWork)
    : IRequestHandler<UpdateCustomerProfileCommand, Unit>
{
    /// <summary>Met à jour le profil client et retourne <see cref="Unit.Value"/>.</summary>
    public async Task<Unit> Handle(UpdateCustomerProfileCommand command, CancellationToken ct)
    {
        // 1. Vérifier l'authentification
        if (currentUserService.UserId is null)
            throw new ForbiddenException();

        var userId = currentUserService.UserId.Value;

        // 2. Récupérer le Customer
        var customer = await customerRepository.GetByApplicationUserIdAsync(userId, ct)
            ?? throw new NotFoundException("Customer", userId);

        // 3. Appliquer les modifications (invariants validés dans le domaine)
        customer.UpdateProfile(
            command.FirstName,
            command.LastName,
            command.CompanyName,
            command.Segment);

        // 4-5. Persister
        await customerRepository.UpdateAsync(customer, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
