using BankMore.ContaCorrente.Application.Dtos;
using BankMore.Shared;
using MediatR;

namespace BankMore.ContaCorrente.Application.Queries.ObterSaldo;

public sealed record ObterSaldoQuery(Guid IdConta)
    : IRequest<Result<SaldoDto>>;
