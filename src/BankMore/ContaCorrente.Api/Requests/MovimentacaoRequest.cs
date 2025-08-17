namespace BankMore.ContaCorrente.Api.Requests;

public sealed class MovimentacaoRequest
{
    public Guid Idempotencia { get; set; }
    public int? NumeroConta { get; set; }
    public decimal Valor { get; set; }
    public char Tipo { get; set; } 
}
