using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Application.Commands.CreateAccount;
using PaymentsService.Infrastructure.Handlers;
using PaymentsService.Infrastructure.Messaging;
using PaymentsService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// ---------- DI ----------
builder.Services.AddControllers();

builder.Services.AddDbContext<PaymentsDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(CreateAccountCommand).Assembly, // Application
    typeof(CreateAccountCommandHandler).Assembly)); // Infrastructure

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<PaymentTaskConsumer>();
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

builder.Services.AddHostedService<PaymentStatusPublisher>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ---------- создаём таблицы, если их нет ----------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
    db.Database.EnsureCreated(); // быстрый способ; замените на Migrate() после добавления миграций
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();