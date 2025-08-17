using BankMore.Transferencia.Domain.Entities;

namespace Transferencia.Domain.Interfaces;

public interface IIdempotenciaRepository
{
    Task<Idempotencia?> ObterAsync(Guid chave);
    Task SalvarAsync(Idempotencia idempotencia);
}
