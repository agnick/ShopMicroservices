using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// ---------- DI ----------
builder.Services.AddControllers();

builder.Services.AddDbContext<OrdersDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateOrderCommand>());

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<OrderPaymentProcessedConsumer>();
    cfg.SetKebabCaseEndpointNameFormatter();
    cfg.UsingRabbitMq((ctx, bus) =>
    {
        bus.Host(builder.Configuration["Rabbit:Host"]!, "/", h =>
        {
            h.Username(builder.Configuration["Rabbit:Username"]);
            h.Password(builder.Configuration["Rabbit:Password"]);
        });
        bus.ConfigureEndpoints(ctx);
    });
});

builder.Services.AddHostedService<OutboxPublisher>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ---------- создаём таблицы ----------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    db.Database.EnsureCreated(); // замените на Migrate() при использовании миграций
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();