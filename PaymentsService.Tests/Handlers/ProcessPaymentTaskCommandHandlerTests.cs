using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Application.Commands.ProcessPaymentTask;
using PaymentsService.Domain.Entities;
using PaymentsService.Infrastructure.Handlers;
using PaymentsService.Infrastructure.Persistence;

public class ProcessPaymentTaskCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldWithdrawMoney_And_WriteOutbox()
    {
        using var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<PaymentsDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new PaymentsDbContext(options);
        db.Database.EnsureCreated();

        var userId = Guid.NewGuid();
        db.Accounts.Add(new Account { UserId = userId, Balance = 1000m });
        await db.SaveChangesAsync();

        var handler = new ProcessPaymentTaskCommandHandler(db);
        var cmd = new ProcessPaymentTaskCommand(
            Guid.NewGuid(), // inbox Id
            Guid.NewGuid(), // order Id
            userId,
            400m);

        await handler.Handle(cmd, CancellationToken.None);

        (await db.Accounts.FirstAsync(a => a.UserId == userId)).Balance.Should().Be(600m);
        db.OutboxMessages.Should().ContainSingle();
    }
}