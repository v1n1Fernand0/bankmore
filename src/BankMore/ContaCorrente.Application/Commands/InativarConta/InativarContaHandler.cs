using MediatR;
using BankMore.Shared;
using BankMore.ContaCorrente.Domain.Interfaces;

namespace BankMore.ContaCorrente.Application.Commands.InativarConta;

public sealed class InativarContaHandler
    : IRequestHandler<InativarContaCommand, Result<Unit>>
{
    private readonly IContaCorrenteRepository _repository;

    public InativarContaHandler(IContaCorrenteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Unit>> Handle(InativarContaCommand request, CancellationToken ct)
    {
        var conta = await _repository.ObterPorIdAsync(request.IdContaCorrente);
        if (conta is null)
            return Result<Unit>.Fail("INVALID_ACCOUNT: Conta não encontrada");

        bool senhaOk = BCrypt.Net.BCrypt.Verify(request.Senha + conta.Salt, conta.SenhaHash);
        if (!senhaOk)
            return Result<Unit>.Fail("USER_UNAUTHORIZED: Senha inválida");

        conta.Inativar();
        await _repository.AtualizarAsync(conta);

        return Result<Unit>.Ok(Unit.Value);
    }
}
