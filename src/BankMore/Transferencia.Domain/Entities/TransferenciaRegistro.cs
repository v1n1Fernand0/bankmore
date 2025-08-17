namespace BankMore.Transferencia.Domain.Entities;

public class TransferenciaRegistro
{
    public Guid IdTransferencia { get; private set; }
    public Guid IdContaOrigem { get; private set; }
    public Guid IdContaDestino { get; private set; }
    public decimal Valor { get; private set; }
    public DateTime Data { get; private set; }

    private TransferenciaRegistro() { }

    public TransferenciaRegistro(Guid idOrigem, Guid idDestino, decimal valor)
    {
        IdTransferencia = Guid.NewGuid();
        IdContaOrigem = idOrigem;
        IdContaDestino = idDestino;
        Valor = valor;
        Data = DateTime.UtcNow;
    }
}
