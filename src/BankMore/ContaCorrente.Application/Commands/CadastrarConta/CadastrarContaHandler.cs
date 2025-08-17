using BankMore.ContaCorrente.Application.Dtos;
using BankMore.ContaCorrente.Application.Mapping;
using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.Shared;
using MediatR;

namespace BankMore.ContaCorrente.Application.Commands.CadastrarConta;

public sealed class CadastrarContaHandler
    : IRequestHandler<CadastrarContaCommand, Result<ContaCorrenteDto>>
{
    private readonly IContaCorrenteRepository _repository;

    public CadastrarContaHandler(IContaCorrenteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ContaCorrenteDto>> Handle(CadastrarContaCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Cpf) || request.Cpf.Length != 11)
            return Result<ContaCorrenteDto>.Fail("INVALID_DOCUMENT: CPF inválido");

        var salt = Guid.NewGuid().ToString("N");
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha + salt);

        var conta = new Conta(
            request.Nome,
            request.Cpf,
            senhaHash,
            salt
        );

        await _repository.AdicionarAsync(conta);

        return Result<ContaCorrenteDto>.Ok(conta.ToDto());
    }
}
