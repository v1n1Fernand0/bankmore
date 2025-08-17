using MediatR;
using BankMore.Shared;

namespace BankMore.ContaCorrente.Application.Commands.MovimentarConta;

public sealed record MovimentarContaCommand(
    Guid Idempotencia,
    int? NumeroConta,
    decimal Valor,
    char TipoMovimento,
    Guid IdContaLogada
) : IRequest<Result<Unit>>;
