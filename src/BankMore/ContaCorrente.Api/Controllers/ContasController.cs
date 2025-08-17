using BankMore.ContaCorrente.Application.Commands.CadastrarConta;
using BankMore.ContaCorrente.Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
    /// <param name="command">Dados da conta</param>
    /// <returns>ContaCorrenteDto</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ContaCorrenteDto), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarContaCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error, type = "INVALID_DOCUMENT" });

        return Ok(result.Value);
    }
}
