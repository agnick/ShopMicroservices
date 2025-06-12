using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Application.Commands.Deposit;
using PaymentsService.Infrastructure.Persistence;

namespace PaymentsService.Infrastructure.Handlers;

/// <summary>
/// Обработчик пополнения счёта.
/// </summary>
public sealed class DepositCommandHandler
    : IRequestHandler<DepositCommand, Unit>
{
    private readonly PaymentsDbContext _db;

    public DepositCommandHandler(PaymentsDbContext db) => _db = db;

    public async Task<Unit> Handle(DepositCommand cmd, CancellationToken ct)
    {
        var acc = await _db.Accounts.FirstOrDefaultAsync(a => a.UserId == cmd.UserId, ct)
                  ?? throw new InvalidOperationException("Счёт не найден.");

        acc.Balance += cmd.Amount;
        _db.Accounts.Update(acc);
        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}