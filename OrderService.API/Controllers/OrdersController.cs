using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace OrderService.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly OrdersDbContext _db;
    public OrdersController(IMediator mediator, OrdersDbContext db)
    {
        _mediator = mediator;
        _db       = db;
    }

    /// <summary>Создать заказ</summary>
    [HttpPost]
    public async Task<ActionResult<CreateOrderResponse>> Create(
        [FromBody] CreateOrderRequest req,
        CancellationToken ct)
    {
        var id = await _mediator.Send(
            new CreateOrderCommand(req.UserId, req.Amount, req.Description), ct);

        return CreatedAtAction(nameof(GetStatus), new { }, new CreateOrderResponse(id));
    }

    /// <summary>Получить список заказов пользователя</summary>
    [HttpPost("user")]
    public async Task<ActionResult<IEnumerable<Order>>> List(
        [FromBody] OrdersByUserRequest dto,
        CancellationToken ct)
    {
        var data = await _db.Orders
            .Where(o => o.UserId == dto.UserId)
            .ToListAsync(ct);

        return Ok(data);
    }

    /// <summary>Получить статус заказа</summary>
    [HttpPost("status")]
    public async Task<ActionResult<StatusResponse>> GetStatus(
        [FromBody] OrderStatusRequest dto,
        CancellationToken ct)
    {
        var status = await _db.Orders
            .Where(o => o.Id == dto.OrderId)
            .Select(o => o.Status.ToString())
            .FirstOrDefaultAsync(ct);

        return status is null
            ? NotFound()
            : Ok(new StatusResponse(status));
    }
}

/* ---------- DTO ---------- */
public record CreateOrderRequest(Guid UserId, decimal Amount, string Description);
public record CreateOrderResponse(Guid OrderId);
public record OrdersByUserRequest(Guid UserId);
public record OrderStatusRequest(Guid OrderId);
public record StatusResponse(string Status);