using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Infrastructure.Persistence;

public class CreateOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateOrder_And_OutboxMessage()
    {
        // SQLite-in-memory поддерживает транзакции, в отличие от EF InMemory
        using var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new OrdersDbContext(options);
        db.Database.EnsureCreated();

        var handler = new CreateOrderCommandHandler(db);
        var cmd = new CreateOrderCommand(Guid.NewGuid(), 200m, "test-order");

        var orderId = await handler.Handle(cmd, CancellationToken.None);

        db.Orders.Should().ContainSingle(o => o.Id == orderId);
        db.OutboxMessages.Should().ContainSingle(m => m.Processed == false);
    }
}