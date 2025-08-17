namespace BankMore.ContaCorrente.Domain.Entities;

public class Conta
{
    public Guid IdContaCorrente { get; private set; }
    public int Numero { get; private set; }
    public string Nome { get; private set; }
    public string Cpf { get; private set; }
    public bool Ativo { get; private set; }
    public string SenhaHash { get; private set; }
    public string Salt { get; private set; }

    private Conta() { }

    public Conta(string nome, string cpf, string senhaHash, string salt)
    {
        IdContaCorrente = Guid.NewGuid();
        Numero = new Random().Next(100000, 999999); 
        Nome = nome;
        Cpf = cpf;
        SenhaHash = senhaHash;
        Salt = salt;
        Ativo = true;
    }

    public void Inativar() => Ativo = false;
}
