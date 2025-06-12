using MediatR;

namespace PaymentsService.Application.Commands.Deposit;

/// <summary>
/// Команда пополнения счёта пользователя на указанную сумму.
/// </summary>
public record DepositCommand(Guid UserId, decimal Amount) : IRequest<Unit>;