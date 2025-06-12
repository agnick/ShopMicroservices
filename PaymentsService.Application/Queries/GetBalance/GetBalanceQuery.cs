using MediatR;

namespace PaymentsService.Application.Queries.GetBalance;

/// <summary>
/// Запрос баланса счёта пользователя.
/// </summary>
public record GetBalanceQuery(Guid UserId) : IRequest<decimal>;