namespace BankMore.ContaCorrente.Application.Dtos;

public sealed class SaldoDto
{
    public int Numero { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataConsulta { get; set; }
    public decimal Valor { get; set; }
}
