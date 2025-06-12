using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentsService.Infrastructure.Persistence;

namespace PaymentsService.Infrastructure.Messaging;

/// <summary>
/// Фоновый сервис, публикующий события OrderPaymentProcessed.
/// </summary>
public sealed class PaymentStatusPublisher : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<PaymentStatusPublisher> _logger;

    public PaymentStatusPublisher(IServiceProvider provider,
        ILogger<PaymentStatusPublisher> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var delay = TimeSpan.FromSeconds(2);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _provider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
                var publish = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                var msgs = await db.OutboxMessages
                    .Where(m => !m.Processed)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                foreach (var m in msgs)
                {
                    var type = Type.GetType(m.Type);
                    if (type == null) continue;

                    var obj = JsonSerializer.Deserialize(m.Payload, type)!;
                    await publish.Publish(obj, type, stoppingToken);
                    m.Processed = true;
                }

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка публикации статуса оплаты.");
            }

            await Task.Delay(delay, stoppingToken);
        }
    }
}