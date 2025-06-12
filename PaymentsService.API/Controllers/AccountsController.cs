using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentsService.Application.Queries.GetBalance;
using PaymentsService.Application.Commands.CreateAccount;
using PaymentsService.Application.Commands.Deposit;

namespace PaymentsService.API.Controllers;

/// <summary>
/// Управление счётом пользователя.
/// </summary>
[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Создать счёт пользователя.
    /// </summary>
    [HttpPost("{userId:guid}")]
    public async Task<IActionResult> Create(Guid userId)
    {
        await _mediator.Send(new CreateAccountCommand(userId));
        return Ok();
    }

    /// <summary>
    /// Пополнить счёт.
    /// </summary>
    [HttpPost("{userId:guid}/deposit")]
    public async Task<IActionResult> Deposit(Guid userId, [FromBody] DepositRequest req)
    {
        await _mediator.Send(new DepositCommand(userId, req.Amount));
        return Ok();
    }

    /// <summary>
    /// Получить баланс счёта.
    /// </summary>
    [HttpGet("{userId:guid}/balance")]
    public async Task<IActionResult> Balance(Guid userId)
    {
        var bal = await _mediator.Send(new GetBalanceQuery(userId));
        return Ok(bal);
    }

    public record DepositRequest(decimal Amount);
}