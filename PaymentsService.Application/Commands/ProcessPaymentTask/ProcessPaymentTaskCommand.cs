using MediatR;

namespace PaymentsService.Application.Commands.ProcessPaymentTask;

/// <summary>
/// Команда обработки задачи оплаты заказа из Inbox.
/// </summary>
public record ProcessPaymentTaskCommand(
    Guid Id,
    Guid OrderId,
    Guid UserId,
    decimal Amount) : IRequest<Unit>;