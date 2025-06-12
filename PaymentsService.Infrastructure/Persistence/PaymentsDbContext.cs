using Microsoft.EntityFrameworkCore;
using PaymentsService.Domain.Entities;

namespace PaymentsService.Infrastructure.Persistence;

/// <summary>
/// EF Core контекст платежного сервиса.
/// </summary>
public class PaymentsDbContext : DbContext
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InboxMessage>().HasIndex(x => x.Processed);
        modelBuilder.Entity<OutboxMessage>().HasIndex(x => x.Processed);
        base.OnModelCreating(modelBuilder);
    }
}