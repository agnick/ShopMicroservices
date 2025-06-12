using Contracts;
using FluentAssertions;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Persistence;

public class OrderPaymentProcessedConsumerTests
{
    [Fact]
    public async Task Consume_ShouldUpdateStatus()
    {
        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var db = new OrdersDbContext(options);

        var order = new Order { Amount = 100, UserId = Guid.NewGuid() };
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        using var harness = new InMemoryTestHarness();
        harness.Consumer(() => new OrderPaymentProcessedConsumer(db, null!));

        await harness.Start();
        await harness.InputQueueSendEndpoint.Send(
            new OrderPaymentProcessed(order.Id, order.UserId, true));

        (await harness.Consumed.Any<OrderPaymentProcessed>()).Should().BeTrue();
        (await db.Orders.FirstAsync(o => o.Id == order.Id)).Status
            .Should().Be(OrderStatus.Finished);
    }
}