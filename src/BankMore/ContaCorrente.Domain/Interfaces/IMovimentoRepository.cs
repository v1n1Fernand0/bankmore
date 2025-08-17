using BankMore.ContaCorrente.Domain.Entities;

namespace BankMore.ContaCorrente.Domain.Interfaces;

public interface IMovimentoRepository
{
    Task<IEnumerable<Movimento>> ObterPorContaAsync(Guid idContaCorrente);
    Task AdicionarAsync(Movimento movimento);
}
