using BankMore.ContaCorrente.Domain.Entities;

namespace BankMore.ContaCorrente.Domain.Interfaces;

public interface IMovimentoRepository
{
    Task<Movimento?> ObterPorIdempotenciaAsync(Guid idempotencia);

    Task<IEnumerable<Movimento>> ObterPorContaAsync(Guid contaId);

    Task AdicionarAsync(Movimento movimento);
}
