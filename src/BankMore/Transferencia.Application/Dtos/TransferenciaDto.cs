namespace BankMore.Transferencia.Application.Dtos;

public sealed class TransferenciaDto
{
    public Guid Id { get; set; }
    public Guid ContaOrigemId { get; set; }    
    public int NumeroContaDestino { get; set; }   
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
}
