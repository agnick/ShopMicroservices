using MediatR;

namespace OrderService.Application.Commands.CreateOrder;

/// <summary>
/// Команда создания нового заказа.
/// </summary>
public record CreateOrderCommand(Guid UserId, decimal Amount, string Description) : IRequest<Guid>;