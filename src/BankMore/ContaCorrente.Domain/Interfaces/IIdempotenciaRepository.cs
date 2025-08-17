using BankMore.ContaCorrente.Domain.Entities;

namespace BankMore.ContaCorrente.Domain.Interfaces;

public interface IIdempotenciaRepository
{
    Task<Idempotencia?> ObterAsync(Guid chave);
    Task SalvarAsync(Idempotencia item);
}
