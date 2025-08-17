using BankMore.ContaCorrente.Domain.Entities;

namespace BankMore.ContaCorrente.Domain.Interfaces;

public interface IContaCorrenteRepository
{
    Task<Conta?> ObterPorIdAsync(Guid id);
    Task<Conta?> ObterPorNumeroAsync(int numero);
    Task<Conta?> ObterPorCpfAsync(string cpf);
    Task AdicionarAsync(Conta conta);
    Task AtualizarAsync(Conta conta);
}
