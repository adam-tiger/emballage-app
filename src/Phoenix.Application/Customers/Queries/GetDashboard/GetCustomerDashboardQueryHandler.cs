using MediatR;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Customers.Dtos;
using Phoenix.Application.Customers.Mappings;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Customers.Repositories;

namespace Phoenix.Application.Customers.Queries.GetDashboard;

/// <summary>
/// Handler de la requête <see cref="GetCustomerDashboardQuery"/>.
/// Construit le tableau de bord avec les données disponibles et des compteurs
/// à <c>0</c> pour les modules non encore développés.
/// </summary>
public sealed class GetCustomerDashboardQueryHandler(
    ICustomerRepository customerRepository,
    ICurrentUserService currentUserService,
    CustomerMapper      customerMapper)
    : IRequestHandler<GetCustomerDashboardQuery, CustomerDashboardDto>
{
    /// <summary>Retourne le tableau de bord du client connecté.</summary>
    public async Task<CustomerDashboardDto> Handle(
        GetCustomerDashboardQuery query, CancellationToken ct)
    {
        if (currentUserService.UserId is null)
            throw new ForbiddenException();

        var userId = currentUserService.UserId.Value;

        var customer = await customerRepository.GetByApplicationUserIdAsync(userId, ct)
            ?? throw new NotFoundException("Customer", userId);

        // Adresse par défaut (si existante)
        var defaultAddress = customer.Addresses.FirstOrDefault(a => a.IsDefault);
        var defaultAddressDto = defaultAddress is not null
            ? customerMapper.ToAddressDto(defaultAddress)
            : null;

        return new CustomerDashboardDto(
            CustomerId:    customer.Id,
            FullName:      customer.FullName,
            TotalOrders:   0,   // TODO : Module 5 — Orders
            PendingOrders: 0,   // TODO : Module 5 — Orders
            TotalQuotes:   0,   // TODO : Module 4 — Quotes
            PendingQuotes: 0,   // TODO : Module 4 — Quotes
            DefaultAddress: defaultAddressDto);
    }
}
