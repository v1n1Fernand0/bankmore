using MediatR;
using BankMore.Shared;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Domain.Entities;

namespace BankMore.ContaCorrente.Application.Commands.MovimentarConta;

public sealed class MovimentarContaHandler
    : IRequestHandler<MovimentarContaCommand, Result<Unit>>
{
    private readonly IContaCorrenteRepository _contas;
    private readonly IMovimentoRepository _movimentos;
    private readonly IIdempotenciaRepository _idempotencia;

    public MovimentarContaHandler(
        IContaCorrenteRepository contas,
        IMovimentoRepository movimentos,
        IIdempotenciaRepository idempotencia)
    {
        _contas = contas;
        _movimentos = movimentos;
        _idempotencia = idempotencia;
    }

    public async Task<Result<Unit>> Handle(MovimentarContaCommand request, CancellationToken ct)
    {
        var idem = await _idempotencia.ObterAsync(request.Idempotencia);
        if (idem is not null)
            return Result<Unit>.Ok(Unit.Value);

        var conta = request.NumeroConta.HasValue
            ? await _contas.ObterPorNumeroAsync(request.NumeroConta.Value)
            : await _contas.ObterPorIdAsync(request.IdContaLogada);

        if (conta is null)
            return Result<Unit>.Fail("INVALID_ACCOUNT: Conta não encontrada");

        if (!conta.Ativo)
            return Result<Unit>.Fail("INACTIVE_ACCOUNT: Conta está inativa");

        if (request.Valor <= 0)
            return Result<Unit>.Fail("INVALID_VALUE: Valor deve ser positivo");

        if (request.TipoMovimento != 'C' && request.TipoMovimento != 'D')
            return Result<Unit>.Fail("INVALID_TYPE: Tipo inválido");

        if (request.NumeroConta.HasValue && request.NumeroConta.Value != conta.Numero && request.TipoMovimento != 'C')
            return Result<Unit>.Fail("INVALID_TYPE: Apenas crédito permitido em conta destino");

        var movimento = new Movimento(conta.IdContaCorrente, request.TipoMovimento, request.Valor);
        await _movimentos.AdicionarAsync(movimento);

        var novoIdem = new Idempotencia(request.Idempotencia, "req", "ok");
        await _idempotencia.SalvarAsync(novoIdem);

        return Result<Unit>.Ok(Unit.Value);
    }
}
