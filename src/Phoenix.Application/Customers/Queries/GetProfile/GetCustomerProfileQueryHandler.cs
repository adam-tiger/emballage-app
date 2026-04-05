using MediatR;
using Phoenix.Application.Common.Exceptions;
using Phoenix.Application.Customers.Dtos;
using Phoenix.Application.Customers.Mappings;
using Phoenix.Domain.Common.Interfaces;
using Phoenix.Domain.Customers.Repositories;

namespace Phoenix.Application.Customers.Queries.GetProfile;

/// <summary>
/// Handler de la requête <see cref="GetCustomerProfileQuery"/>.
/// Récupère le Customer courant et le mappe en <see cref="CustomerProfileDto"/>.
/// </summary>
public sealed class GetCustomerProfileQueryHandler(
    ICustomerRepository customerRepository,
    ICurrentUserService currentUserService,
    CustomerMapper      customerMapper)
    : IRequestHandler<GetCustomerProfileQuery, CustomerProfileDto>
{
    /// <summary>Retourne le profil du client connecté.</summary>
    public async Task<CustomerProfileDto> Handle(
        GetCustomerProfileQuery query, CancellationToken ct)
    {
        if (currentUserService.UserId is null)
            throw new ForbiddenException();

        var userId = currentUserService.UserId.Value;

        var customer = await customerRepository.GetByApplicationUserIdAsync(userId, ct)
            ?? throw new NotFoundException("Customer", userId);

        return customerMapper.ToProfileDto(customer);
    }
}
