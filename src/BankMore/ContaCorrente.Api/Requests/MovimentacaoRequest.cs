public class MovimentacaoRequest
{
    public Guid Idempotencia { get; set; }
    public int? NumeroConta { get; set; }
    public decimal Valor { get; set; }
    public string Tipo { get; set; } = string.Empty; 
}
