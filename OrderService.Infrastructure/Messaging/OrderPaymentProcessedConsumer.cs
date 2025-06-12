using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Enums;
using OrderService.Infrastructure.Persistence;
using Contracts;

namespace OrderService.Infrastructure.Messaging;

/// <summary>Обновляет статус заказа по событию от Payments-сервиса.</summary>
public sealed class OrderPaymentProcessedConsumer : IConsumer<OrderPaymentProcessed>
{
    private readonly OrdersDbContext _db;
    private readonly ILogger<OrderPaymentProcessedConsumer> _log;

    public OrderPaymentProcessedConsumer(OrdersDbContext db, ILogger<OrderPaymentProcessedConsumer> log)
    {
        _db  = db;
        _log = log;
    }

    public async Task Consume(ConsumeContext<OrderPaymentProcessed> ctx)
    {
        var msg = ctx.Message;

        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == msg.OrderId);
        if (order == null) return;

        order.Status = msg.Success ? OrderStatus.Finished : OrderStatus.Cancelled;
        await _db.SaveChangesAsync();

        _log.LogInformation("Order {OrderId} set to {Status}", msg.OrderId, order.Status);
    }
}