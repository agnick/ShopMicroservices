using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Application.Commands.CreateAccount;
using PaymentsService.Domain.Entities;
using PaymentsService.Infrastructure.Persistence;

namespace PaymentsService.Infrastructure.Handlers;

/// <summary>
/// Обработчик создания счёта пользователя.
/// </summary>
public sealed class CreateAccountCommandHandler
    : IRequestHandler<CreateAccountCommand, Unit>
{
    private readonly PaymentsDbContext _db;

    public CreateAccountCommandHandler(PaymentsDbContext db) => _db = db;

    public async Task<Unit> Handle(CreateAccountCommand request, CancellationToken ct)
    {
        var exists = await _db.Accounts.AnyAsync(a => a.UserId == request.UserId, ct);
        if (!exists)
        {
            _db.Accounts.Add(new Account { UserId = request.UserId, Balance = 0m });
            await _db.SaveChangesAsync(ct);
        }
        return Unit.Value;
    }
}