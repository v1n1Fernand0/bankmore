namespace BankMore.ContaCorrente.Application.Dtos;

public sealed class MovimentoDto
{
    public Guid Id { get; set; }
    public decimal Valor { get; set; }
    public string Tipo { get; set; } = string.Empty; 
    public DateTime DataCriacao { get; set; }
}
