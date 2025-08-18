namespace BankMore.ContaCorrente.Domain.Entities;

public class Movimento
{
    public Guid Id { get; private set; }
    public Guid ContaId { get; private set; }
    public Guid Idempotencia { get; private set; }
    public decimal Valor { get; private set; }
    public string Tipo { get; private set; }  
    public DateTime DataCriacao { get; private set; }

    private Movimento() { }

    public Movimento(Guid contaId, Guid idempotencia, decimal valor, string tipo)
    {
        Id = Guid.NewGuid();
        ContaId = contaId;
        Idempotencia = idempotencia;
        Valor = valor;
        Tipo = tipo;
        DataCriacao = DateTime.UtcNow;
    }
}
