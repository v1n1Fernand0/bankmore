namespace BankMore.Transferencia.Application.Dtos;

public sealed class TransferenciaDto
{
    public Guid IdTransferencia { get; set; }
    public Guid IdContaOrigem { get; set; }
    public Guid IdContaDestino { get; set; }
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
}
