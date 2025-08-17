using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Application.Dtos;

namespace BankMore.ContaCorrente.Application.Mapping;

public static class ContaCorrenteMapping
{
    public static ContaCorrenteDto ToDto(this Conta conta)
    {
        return new ContaCorrenteDto
        {
            IdContaCorrente = conta.IdContaCorrente,
            Numero = conta.Numero,
            Nome = conta.Nome,
            Cpf = conta.Cpf,
            Ativo = conta.Ativo
        };
    }
}
