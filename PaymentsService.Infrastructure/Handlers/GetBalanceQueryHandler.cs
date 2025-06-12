using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Application.Queries.GetBalance;
using PaymentsService.Infrastructure.Persistence;

namespace PaymentsService.Infrastructure.Handlers;

/// <summary>
/// Обработчик запроса баланса.
/// </summary>
public sealed class GetBalanceQueryHandler
    : IRequestHandler<GetBalanceQuery, decimal>
{
    private readonly PaymentsDbContext _db;

    public GetBalanceQueryHandler(PaymentsDbContext db) => _db = db;

    public async Task<decimal> Handle(GetBalanceQuery q, CancellationToken ct)
    {
        var bal = await _db.Accounts
            .Where(a => a.UserId == q.UserId)
            .Select(a => a.Balance)
            .FirstOrDefaultAsync(ct);

        return bal;
    }
}