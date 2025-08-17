using BankMore.Transferencia.Application.Commands.EfetuarTransferencia;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.Transferencia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransferenciasController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransferenciasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Efetua uma transferência entre contas
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> EfetuarTransferencia([FromBody] EfetuarTransferenciaRequest request)
    {
        var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        var command = new EfetuarTransferenciaCommand
        {
            ContaOrigemId = request.ContaOrigemId,
            NumeroContaDestino = request.NumeroContaDestino,
            Valor = request.Valor,
            Idempotencia = request.Idempotencia,
            JwtToken = jwtToken
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(new { message = "Transferência realizada com sucesso" });
    }
}
