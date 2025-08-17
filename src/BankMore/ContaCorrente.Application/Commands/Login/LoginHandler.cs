using MediatR;
using BankMore.Shared;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Application.Dtos;
using BankMore.Shared.Auth;
using Microsoft.Extensions.Configuration;

namespace BankMore.ContaCorrente.Application.Commands.Login;

public sealed class LoginHandler
    : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly IContaCorrenteRepository _repository;
    private readonly IConfiguration _configuration;

    public LoginHandler(IContaCorrenteRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        Conta? conta = int.TryParse(request.Identificador, out int numero)
            ? await _repository.ObterPorNumeroAsync(numero)
            : await _repository.ObterPorCpfAsync(request.Identificador);

        if (conta is null)
            return Result<LoginResponseDto>.Fail("USER_UNAUTHORIZED: Conta não encontrada");

        bool senhaOk = BCrypt.Net.BCrypt.Verify(request.Senha + conta.Salt, conta.SenhaHash);
        if (!senhaOk)
            return Result<LoginResponseDto>.Fail("USER_UNAUTHORIZED: Senha inválida");

        string secret = _configuration["Jwt:Secret"] ?? throw new Exception("Jwt:Secret não configurado");
        int expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "60");

        string token = JwtHelper.GenerateToken(conta.IdContaCorrente, secret, expireMinutes);

        return Result<LoginResponseDto>.Ok(new LoginResponseDto
        {
            Token = token,
            ExpiraEm = DateTime.UtcNow.AddMinutes(expireMinutes)
        });
    }
}
