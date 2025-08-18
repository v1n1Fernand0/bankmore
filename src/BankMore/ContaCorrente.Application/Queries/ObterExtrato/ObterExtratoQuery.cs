using BankMore.ContaCorrente.Application.Dtos;
using BankMore.Shared;
using MediatR;

namespace ContaCorrente.Application.Queries.ObterExtrato
{
    public sealed record ObterExtratoQuery(Guid ContaId) : IRequest<Result<IReadOnlyList<MovimentoDto>>>;

}
