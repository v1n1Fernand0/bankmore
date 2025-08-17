namespace BankMore.ContaCorrente.Domain.Entities;

public class Idempotencia
{
    public Guid ChaveIdempotencia { get; private set; }
    public string Requisicao { get; private set; }
    public string Resultado { get; private set; }

    private Idempotencia() { }

    public Idempotencia(Guid chave, string requisicao, string resultado)
    {
        ChaveIdempotencia = chave;
        Requisicao = requisicao;
        Resultado = resultado;
    }
}
