using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Application.Commands.Deposit;
using PaymentsService.Domain.Entities;
using PaymentsService.Infrastructure.Handlers;
using PaymentsService.Infrastructure.Persistence;

public class DepositCommandHandlerTests
{
    [Fact]
    public async void Handle_ShouldIncreaseBalance()
    {
        var options = new DbContextOptionsBuilder<PaymentsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var db = new PaymentsDbContext(options);

        var acc = new Account { UserId = Guid.NewGuid(), Balance = 0 };
        db.Accounts.Add(acc);
        await db.SaveChangesAsync();

        var handler = new DepositCommandHandler(db);
        await handler.Handle(new DepositCommand(acc.UserId, 500m), CancellationToken.None);

        (await db.Accounts.FirstAsync(a => a.UserId == acc.UserId)).Balance
            .Should().Be(500m);
    }
}