using BankMore.Shared;
using MediatR;

namespace BankMore.Transferencia.Application.Commands.EfetuarTransferencia;

public sealed record EfetuarTransferenciaCommand : IRequest<Result<Unit>>
{
    public Guid ContaOrigemId { get; init; }
    public int NumeroContaDestino { get; init; }
    public decimal Valor { get; init; }
    public Guid Idempotencia { get; init; }
    public string JwtToken { get; init; } = string.Empty;
}


