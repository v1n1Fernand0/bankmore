using BankMore.ContaCorrente.Application.Dtos;
using BankMore.Shared;
using MediatR;

namespace BankMore.ContaCorrente.Application.Commands.Login;

public sealed record LoginCommand(string Identificador, string Senha)
    : IRequest<Result<LoginResponseDto>>;
