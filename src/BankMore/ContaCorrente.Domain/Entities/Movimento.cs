namespace BankMore.ContaCorrente.Domain.Entities;

public class Movimento
{
    public Guid IdMovimento { get; private set; }
    public Guid IdContaCorrente { get; private set; }
    public DateTime DataMovimento { get; private set; }
    public char TipoMovimento { get; private set; } 
    public decimal Valor { get; private set; }

    private Movimento() { }

    public Movimento(Guid idContaCorrente, char tipo, decimal valor)
    {
        IdMovimento = Guid.NewGuid();
        IdContaCorrente = idContaCorrente;
        DataMovimento = DateTime.UtcNow;
        TipoMovimento = tipo;
        Valor = valor;
    }
}
