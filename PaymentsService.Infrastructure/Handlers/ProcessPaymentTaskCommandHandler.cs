using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Contracts;
using PaymentsService.Application.Commands.ProcessPaymentTask;
using PaymentsService.Domain.Enums;
using PaymentsService.Infrastructure.Persistence;

namespace PaymentsService.Infrastructure.Handlers;

/// <summary>Exactly-once обработка задачи оплаты.</summary>
public sealed class ProcessPaymentTaskCommandHandler
    : IRequestHandler<ProcessPaymentTaskCommand, Unit>
{
    private readonly PaymentsDbContext _db;
    public ProcessPaymentTaskCommandHandler(PaymentsDbContext db) => _db = db;

    public async Task<Unit> Handle(ProcessPaymentTaskCommand cmd, CancellationToken ct)
    {
        var inbox = await _db.InboxMessages.FindAsync(new object[] { cmd.Id }, ct);

        // если уже обработали — выходим
        if (inbox is { Processed: true }) return Unit.Value;

        if (inbox == null) // первая встреча
        {
            inbox = new InboxMessage
            {
                Id = cmd.Id,
                Type = typeof(PaymentTask).FullName!,
                Payload = JsonSerializer.Serialize(new PaymentTask(cmd.OrderId, cmd.UserId, cmd.Amount)),
                Processed = false
            };
            _db.InboxMessages.Add(inbox);
        }

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var result = await Process(cmd, ct);
        inbox.Processed = true;

        var evt = new OrderPaymentProcessed(cmd.OrderId, cmd.UserId, result == PaymentResult.Success);
        _db.OutboxMessages.Add(new OutboxMessage
        {
            Type = typeof(OrderPaymentProcessed).AssemblyQualifiedName!,
            Payload = JsonSerializer.Serialize(evt)
        });

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return Unit.Value;
    }

    // бизнес-логика списания
    private async Task<PaymentResult> Process(ProcessPaymentTaskCommand cmd, CancellationToken ct)
    {
        var acc = await _db.Accounts.FirstOrDefaultAsync(a => a.UserId == cmd.UserId, ct);
        if (acc == null) return PaymentResult.AccountNotFound;
        if (acc.Balance < cmd.Amount) return PaymentResult.NotEnoughFunds;

        acc.Balance -= cmd.Amount;
        return PaymentResult.Success;
    }
}