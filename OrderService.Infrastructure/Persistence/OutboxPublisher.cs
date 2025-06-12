using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OrderService.Infrastructure.Persistence;

/// <summary>
/// Фоновый сервис, выгружающий сообщения из transactional outbox в очередь.
/// </summary>
public sealed class OutboxPublisher : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<OutboxPublisher> _logger;

    public OutboxPublisher(IServiceProvider provider, ILogger<OutboxPublisher> logger)
    {
        _provider = provider;
        _logger   = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var delay = TimeSpan.FromSeconds(2);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _provider.CreateScope();
                var db      = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
                var publish = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                var msgs = await db.OutboxMessages
                                   .Where(m => !m.Processed)
                                   .Take(20)
                                   .ToListAsync(stoppingToken);

                foreach (var m in msgs)
                {
                    var type = Type.GetType(m.Type);
                    if (type == null) continue;

                    var payload = JsonSerializer.Deserialize(m.Payload, type);
                    if (payload == null) continue;

                    await publish.Publish(payload, type, stoppingToken);
                    m.Processed = true;
                }

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка публикации сообщений Outbox.");
            }

            await Task.Delay(delay, stoppingToken);
        }
    }
}
