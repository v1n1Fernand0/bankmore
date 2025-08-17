using BankMore.ContaCorrente.Application.Dtos;
using BankMore.Shared;
using MediatR;

namespace BankMore.ContaCorrente.Application.Commands.CadastrarConta;

public sealed record CadastrarContaCommand(string Nome, string Cpf, string Senha)
    : IRequest<Result<ContaCorrenteDto>>;
