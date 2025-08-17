using MediatR;
using BankMore.Shared;

namespace BankMore.ContaCorrente.Application.Commands.InativarConta;

public sealed record InativarContaCommand(Guid IdContaCorrente, string Senha)
    : IRequest<Result<Unit>>;
