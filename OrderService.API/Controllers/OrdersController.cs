using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace OrderService.API.Controllers;

/// <summary>
/// Управление заказами.
/// </summary>
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly OrdersDbContext _db;

    public OrdersController(IMediator mediator, OrdersDbContext db)
    {
        _mediator = mediator;
        _db = db;
    }

    /// <summary>
    /// Создать новый заказ и запустить оплату.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateOrderRequest req,
        CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateOrderCommand(
            req.UserId, req.Amount, req.Description), ct);
        return CreatedAtAction(nameof(GetStatus), new { orderId = id }, id);
    }

    /// <summary>
    /// Получить все заказы пользователя.
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<Order>>> List(Guid userId, CancellationToken ct)
    {
        var data = await _db.Orders.Where(o => o.UserId == userId).ToListAsync(ct);
        return Ok(data);
    }

    /// <summary>
    /// Получить статус заказа.
    /// </summary>
    [HttpGet("{orderId:guid}/status")]
    public async Task<ActionResult<string>> GetStatus(Guid orderId, CancellationToken ct)
    {
        var status = await _db.Orders
            .Where(o => o.Id == orderId)
            .Select(o => o.Status.ToString())
            .FirstOrDefaultAsync(ct);

        return status is null ? NotFound() : Ok(status);
    }
}

public record CreateOrderRequest(Guid UserId, decimal Amount, string Description);