namespace BankMore.Transferencia.Domain.Entities;

public sealed class TransferenciaRegistro
{
    public Guid Id { get; private set; }
    public Guid ContaOrigemId { get; private set; }  
    public int NumeroContaDestino { get; private set; } 
    public decimal Valor { get; private set; }
    public DateTime Data { get; private set; }

    private TransferenciaRegistro() { } 

    public TransferenciaRegistro(Guid contaOrigemId, int numeroContaDestino, decimal valor)
    {
        Id = Guid.NewGuid();
        ContaOrigemId = contaOrigemId;
        NumeroContaDestino = numeroContaDestino;
        Valor = valor;
        Data = DateTime.UtcNow;
    }
}

