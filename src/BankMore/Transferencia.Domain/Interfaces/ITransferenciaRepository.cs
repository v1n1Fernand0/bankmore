using BankMore.Transferencia.Domain.Entities;

namespace BankMore.Transferencia.Domain.Interfaces;

public interface ITransferenciaRepository
{
    Task AdicionarAsync(TransferenciaRegistro transferencia);
}
