using BankMore.ContaCorrente.Application.Commands.CadastrarConta;
using BankMore.ContaCorrente.Application.Commands.InativarConta;
using BankMore.ContaCorrente.Application.Commands.Login;
using BankMore.ContaCorrente.Application.Commands.MovimentarConta;
using BankMore.ContaCorrente.Application.Dtos;
using BankMore.ContaCorrente.Application.Queries.ObterSaldo;
using ContaCorrente.Api.Requests;
using ContaCorrente.Application.Queries.ObterExtrato;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankMore.ContaCorrente.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContasController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContasController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Cadastra uma nova conta corrente
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ContaCorrenteDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarContaCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error, type = "INVALID_DOCUMENT" });

        return CreatedAtAction(nameof(Saldo), new { }, result.Value);
    }

    /// <summary>
    /// Efetua login e retorna token JWT
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return Unauthorized(new { message = result.Error, type = "USER_UNAUTHORIZED" });

        return Ok(result.Value);
    }

    /// <summary>
    /// Inativa a conta corrente autenticada
    /// </summary>
    [HttpPost("inativar")]
    [Authorize]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Inativar([FromBody] InativarContaRequest request)
    {
        var contaId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (contaId is null)
            return Forbid();

        var command = new InativarContaCommand(Guid.Parse(contaId), request.Senha);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error, type = "USER_UNAUTHORIZED" });

        return NoContent();
    }

    /// <summary>
    /// Realiza movimentação na conta (depósito/saque)
    /// </summary>
    [HttpPost("movimentar")]
    [Authorize]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Movimentar([FromBody] MovimentacaoRequest request)
    {
        var contaId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (contaId is null)
            return Forbid();

        var command = new MovimentarContaCommand(
            request.Idempotencia,
            request.NumeroConta,
            request.Valor,
            request.Tipo,
            Guid.Parse(contaId)
        );

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error, type = "INVALID_OPERATION" });

        return NoContent();
    }

    /// <summary>
    /// Consulta saldo da conta corrente autenticada
    /// </summary>
    [HttpGet("saldo")]
    [Authorize]
    [ProducesResponseType(typeof(SaldoDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Saldo()
    {
        var contaId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (contaId is null)
            return Forbid();

        var query = new ObterSaldoQuery(Guid.Parse(contaId));
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error, type = "INVALID_OPERATION" });

        return Ok(result.Value);
    }

    /// <summary>
    /// Lista as movimentações da conta autenticada (extrato)
    /// </summary>
    [HttpGet("extrato")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<MovimentoDto>), 200)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Extrato()
    {
        var contaId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (contaId is null)
            return Forbid();

        var query = new ObterExtratoQuery(Guid.Parse(contaId));
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error, type = "INVALID_OPERATION" });

        return Ok(result.Value);
    }

}
