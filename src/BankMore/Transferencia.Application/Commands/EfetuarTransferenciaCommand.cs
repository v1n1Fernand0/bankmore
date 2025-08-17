using MediatR;
using BankMore.Shared;

namespace BankMore.Transferencia.Application.Commands.EfetuarTransferencia;

public sealed record EfetuarTransferenciaCommand(
    Guid Idempotencia,
    int NumeroContaDestino,
    decimal Valor,
    Guid ContaOrigemId,
    string JwtToken
) : IRequest<Result<Unit>>;
