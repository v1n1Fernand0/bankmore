using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.Shared;
using MediatR;

namespace BankMore.ContaCorrente.Application.Commands.MovimentarConta;

public sealed class MovimentarContaHandler
    : IRequestHandler<MovimentarContaCommand, Result<Unit>>
{
    private readonly IContaCorrenteRepository _contas;
    private readonly IMovimentoRepository _movimentos;

    public MovimentarContaHandler(
        IContaCorrenteRepository contas,
        IMovimentoRepository movimentos)
    {
        _contas = contas;
        _movimentos = movimentos;
    }

    public async Task<Result<Unit>> Handle(MovimentarContaCommand request, CancellationToken ct)
    {
        var movimentoExistente = await _movimentos.ObterPorIdempotenciaAsync(request.Idempotencia);
        if (movimentoExistente is not null)
            return Result<Unit>.Ok(Unit.Value);

        var conta = await _contas.ObterPorIdAsync(request.ContaId);
        if (conta is null || !conta.Ativo)
            return Result<Unit>.Fail("ACCOUNT_NOT_FOUND_OR_INACTIVE");

        if (request.Tipo == "D")
        {
            var movimentosConta = await _movimentos.ObterPorContaAsync(conta.IdContaCorrente);

            decimal saldo = 0;
            foreach (var mov in movimentosConta)
            {
                if (mov.Tipo == "C")
                    saldo += mov.Valor;
                else if (mov.Tipo == "D")
                    saldo -= mov.Valor;
            }

            if (saldo < request.Valor)
                return Result<Unit>.Fail("INSUFFICIENT_FUNDS");
        }
        else if (request.Tipo != "C")
        {
            return Result<Unit>.Fail("INVALID_OPERATION");
        }

        var movimento = new Movimento(
            request.ContaId,
            request.Idempotencia,
            request.Valor,
            request.Tipo
        );

        await _movimentos.AdicionarAsync(movimento);

        return Result<Unit>.Ok(Unit.Value);
    }
}
