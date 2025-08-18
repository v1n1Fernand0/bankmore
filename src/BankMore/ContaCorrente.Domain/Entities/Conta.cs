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
    public decimal Saldo { get; private set; }

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
        Saldo = 0;
    }

    public void Inativar() => Ativo = false;

    public void Depositar(decimal valor)
    {
        if (valor <= 0)
            throw new InvalidOperationException("Valor do depósito deve ser positivo.");

        Saldo += valor;
    }

    public void Sacar(decimal valor)
    {
        if (valor <= 0)
            throw new InvalidOperationException("Valor do saque deve ser positivo.");

        if (Saldo < valor)
            throw new InvalidOperationException("Saldo insuficiente.");

        Saldo -= valor;
    }
}
