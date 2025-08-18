using BankMore.Shared;
using MediatR;

namespace BankMore.ContaCorrente.Application.Commands.MovimentarConta;

public sealed record MovimentarContaCommand(
    Guid Idempotencia,
    int? NumeroConta,  
    decimal Valor,
    string Tipo,      
    Guid ContaId
) : IRequest<Result<Unit>>;
