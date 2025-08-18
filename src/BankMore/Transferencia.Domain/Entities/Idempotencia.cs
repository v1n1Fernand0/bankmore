namespace BankMore.Transferencia.Domain.Entities;

public sealed class Idempotencia
{
    public Guid Chave { get; private set; }
    public DateTime DataCriacao { get; private set; }

    private Idempotencia() { }

    public Idempotencia(Guid chave)
    {
        Chave = chave;
        DataCriacao = DateTime.UtcNow;
    }
}
