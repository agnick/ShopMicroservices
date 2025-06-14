using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentsService.Application.Commands.CreateAccount;
using PaymentsService.Application.Commands.Deposit;
using PaymentsService.Application.Queries.GetBalance;

namespace PaymentsService.API.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AccountsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Создать счёт</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest dto)
    {
        await _mediator.Send(new CreateAccountCommand(dto.UserId));
        return Ok(new { created = true });
    }

    /// <summary>Пополнить счёт</summary>
    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] DepositRequest dto)
    {
        await _mediator.Send(new DepositCommand(dto.UserId, dto.Amount));
        return Ok(new { deposited = true });
    }

    /// <summary>Получить баланс счёта</summary>
    [HttpPost("balance")]
    public async Task<IActionResult> Balance([FromBody] BalanceRequest dto)
    {
        var bal = await _mediator.Send(new GetBalanceQuery(dto.UserId));
        return Ok(new BalanceResponse(bal));
    }
}

/* ---------- DTO ---------- */
public record CreateAccountRequest(Guid UserId);
public record DepositRequest(Guid UserId, decimal Amount);
public record BalanceRequest(Guid UserId);
public record BalanceResponse(decimal Balance);