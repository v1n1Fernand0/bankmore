namespace BankMore.Transferencia.Application.Commands.EfetuarTransferencia;

public sealed class EfetuarTransferenciaRequest
{
    public Guid Idempotencia { get; set; } = Guid.NewGuid();
    public Guid ContaOrigemId { get; set; }
    public int NumeroContaDestino { get; set; } 
    public decimal Valor { get; set; }
}
