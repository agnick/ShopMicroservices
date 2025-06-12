using Contracts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using PaymentsService.Application.Commands.ProcessPaymentTask;

namespace PaymentsService.Infrastructure.Messaging;

/// <summary>Консьюмер входящих задач оплат.</summary>
public sealed class PaymentTaskConsumer : IConsumer<PaymentTask>
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentTaskConsumer> _log;

    public PaymentTaskConsumer(IMediator mediator, ILogger<PaymentTaskConsumer> log)
    {
        _mediator = mediator;
        _log      = log;
    }

    public async Task Consume(ConsumeContext<PaymentTask> ctx)
    {
        var inboxId = ctx.MessageId ?? Guid.NewGuid();   // идемпотентность
        _log.LogInformation("Payment task received {@Msg}", ctx.Message);

        await _mediator.Send(new ProcessPaymentTaskCommand(
            inboxId,
            ctx.Message.OrderId,
            ctx.Message.UserId,
            ctx.Message.Amount));
    }
}