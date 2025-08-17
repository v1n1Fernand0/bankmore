namespace BankMore.ContaCorrente.Application.Dtos;

public sealed class ContaCorrenteDto
{
    public Guid IdContaCorrente { get; set; }
    public int Numero { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public bool Ativo { get; set; }
}
