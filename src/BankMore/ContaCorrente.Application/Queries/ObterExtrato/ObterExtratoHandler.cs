using BankMore.ContaCorrente.Application.Dtos;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.Shared;
using MediatR;

namespace ContaCorrente.Application.Queries.ObterExtrato
{

    public sealed class ObterExtratoHandler
        : IRequestHandler<ObterExtratoQuery, Result<IReadOnlyList<MovimentoDto>>>
    {
        private readonly IMovimentoRepository _movimentos;

        public ObterExtratoHandler(IMovimentoRepository movimentos) => _movimentos = movimentos;

        public async Task<Result<IReadOnlyList<MovimentoDto>>> Handle(ObterExtratoQuery request, CancellationToken ct)
        {
            var movimentos = await _movimentos.ObterPorContaAsync(request.ContaId);

            var lista = movimentos
                .OrderByDescending(m => m.DataCriacao)
                .Select(m => new MovimentoDto
                {
                    Id = m.Id,
                    Valor = m.Valor,
                    Tipo = m.Tipo,
                    DataCriacao = m.DataCriacao
                })
                .ToList();

            return Result<IReadOnlyList<MovimentoDto>>.Ok(lista);
        }
    }
}
