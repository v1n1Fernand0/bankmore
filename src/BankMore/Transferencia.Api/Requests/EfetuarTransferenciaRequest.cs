namespace BankMore.Transferencia.Api.Requests;

public sealed class EfetuarTransferenciaRequest
{
    public Guid Idempotencia { get; set; }
    public int NumeroContaDestino { get; set; }
    public decimal Valor { get; set; }
}
