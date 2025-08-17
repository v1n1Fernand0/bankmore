using MediatR;
using BankMore.Shared;
using BankMore.ContaCorrente.Application.Dtos;
using BankMore.ContaCorrente.Domain.Interfaces;

namespace BankMore.ContaCorrente.Application.Queries.ObterSaldo;

public sealed class ObterSaldoHandler
    : IRequestHandler<ObterSaldoQuery, Result<SaldoDto>>
{
    private readonly IContaCorrenteRepository _contas;
    private readonly IMovimentoRepository _movimentos;

    public ObterSaldoHandler(IContaCorrenteRepository contas, IMovimentoRepository movimentos)
    {
        _contas = contas;
        _movimentos = movimentos;
    }

    public async Task<Result<SaldoDto>> Handle(ObterSaldoQuery request, CancellationToken ct)
    {
        var conta = await _contas.ObterPorIdAsync(request.IdConta);
        if (conta is null)
            return Result<SaldoDto>.Fail("INVALID_ACCOUNT: Conta não encontrada");

        if (!conta.Ativo)
            return Result<SaldoDto>.Fail("INACTIVE_ACCOUNT: Conta inativa");

        var movimentos = await _movimentos.ObterPorContaAsync(conta.IdContaCorrente);

        decimal saldo = 0;
        foreach (var mov in movimentos)
        {
            if (mov.TipoMovimento == 'C')
                saldo += mov.Valor;
            else if (mov.TipoMovimento == 'D')
                saldo -= mov.Valor;
        }

        return Result<SaldoDto>.Ok(new SaldoDto
        {
            Numero = conta.Numero,
            Nome = conta.Nome,
            DataConsulta = DateTime.UtcNow,
            Valor = saldo
        });
    }
}
