using MediatR;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Customers.Entities;
using Phoenix.Domain.Customers.Repositories;

namespace Phoenix.Application.Customers.Commands.AddAddress;

/// <summary>
/// Handler de la commande <see cref="AddCustomerAddressCommand"/>.
/// Crée l'adresse, l'ajoute à l'agrégat Customer et persiste.
/// </summary>
public sealed class AddCustomerAddressCommandHandler(
    ICustomerRepository customerRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork         unitOfWork)
    : IRequestHandler<AddCustomerAddressCommand, Guid>
{
    /// <summary>
    /// Ajoute l'adresse au client courant et retourne son identifiant.
    /// </summary>
    public async Task<Guid> Handle(AddCustomerAddressCommand command, CancellationToken ct)
    {
        // 1. Vérifier l'authentification
        if (currentUserService.UserId is null)
            throw new ForbiddenException();

        var userId = currentUserService.UserId.Value;

        // 2. Récupérer le Customer (avec ses adresses)
        var customer = await customerRepository.GetByApplicationUserIdAsync(userId, ct)
            ?? throw new NotFoundException("Customer", userId);

        // 3. Créer l'entité adresse (invariants validés dans le constructeur)
        var address = new CustomerAddress(
            customerId: customer.Id,
            label:      command.Label,
            street:     command.Street,
            city:       command.City,
            postalCode: command.PostalCode,
            country:    command.Country);

        // 4. Ajouter via l'agrégat (gère la limite max et le défaut automatique)
        customer.AddAddress(address);

        // Si IsDefault demandé explicitement et ce n'est pas la première adresse
        if (command.IsDefault && customer.Addresses.Count > 1)
            customer.SetDefaultAddress(address.Id);

        // 5. Persister
        await customerRepository.UpdateAsync(customer, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // 6. Retourner l'Id de la nouvelle adresse
        return address.Id;
    }
}
