using System.Threading;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Application.Commands.CreateAccount;
using PaymentsService.Infrastructure.Handlers;
using PaymentsService.Infrastructure.Persistence;
using Xunit;

public class CreateAccountCommandHandlerTests
{
    [Fact]
    public async void Handle_ShouldCreateAccount_IfNotExists()
    {
        var options = new DbContextOptionsBuilder<PaymentsDbContext>()
            .UseInMemoryDatabase("pay_db")
            .Options;
        await using var db = new PaymentsDbContext(options);

        var handler = new CreateAccountCommandHandler(db);
        var userId = Guid.NewGuid();

        await handler.Handle(new CreateAccountCommand(userId), CancellationToken.None);

        db.Accounts.Should().ContainSingle(a => a.UserId == userId);
    }
}