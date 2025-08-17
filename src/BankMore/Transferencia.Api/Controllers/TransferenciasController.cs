using BankMore.Transferencia.Api.Requests;
using BankMore.Transferencia.Application.Commands.EfetuarTransferencia;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankMore.Transferencia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransferenciasController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransferenciasController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [Authorize]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Transferir([FromBody] EfetuarTransferenciaRequest request)
    {
        var contaId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (contaId is null)
            return Forbid();

        var command = new EfetuarTransferenciaCommand(
            request.Idempotencia,
            request.NumeroContaDestino,
            request.Valor,
            Guid.Parse(contaId),
            Request.Headers["Authorization"]!
        );

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error, type = "TRANSFER_ERROR" });

        return NoContent();
    }
}
