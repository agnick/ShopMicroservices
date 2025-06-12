using System.Text.Json;
using MediatR;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Infrastructure.Persistence;
using Contracts;

namespace OrderService.Application.Commands.CreateOrder;

/// <summary>Обработчик создания заказа + запись outbox.</summary>
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly OrdersDbContext _db;
    public CreateOrderCommandHandler(OrdersDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateOrderCommand req, CancellationToken ct)
    {
        var order = new Order
        {
            UserId = req.UserId,
            Amount = req.Amount,
            Description = req.Description,
            Status = OrderStatus.New
        };

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        _db.Orders.Add(order);

        var task = new PaymentTask(order.Id, order.UserId, order.Amount);

        _db.OutboxMessages.Add(new OutboxMessage
        {
            Type = typeof(PaymentTask).AssemblyQualifiedName!, // «Contracts.PaymentTask»
            Payload = JsonSerializer.Serialize(task)
        });

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return order.Id;
    }
}