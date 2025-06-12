using MediatR;

namespace PaymentsService.Application.Commands.CreateAccount;

/// <summary>
/// Команда создания счёта пользователя.
/// </summary>
public record CreateAccountCommand(Guid UserId) : IRequest<Unit>;