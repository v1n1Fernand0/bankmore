using BankMore.ContaCorrente.Domain.Entities;
using FluentAssertions;

namespace BankMore.Tests.ContaCorrente.Domain.Entities;

public class ContaTests
{
    [Fact]
    public void Deve_Criar_Conta_Ativa_Com_Saldo_Zero()
    {
        var conta = new Conta("Ana Souza", "11111111111", "hash123", "salt123");

        conta.Nome.Should().Be("Ana Souza");
        conta.Cpf.Should().Be("11111111111");
        conta.Ativo.Should().BeTrue();
        conta.Saldo.Should().Be(0);
    }

    [Fact]
    public void Depositar_Deve_Aumentar_Saldo()
    {
        var conta = new Conta("Ana Souza", "11111111111", "hash123", "salt123");
        conta.Depositar(100);

        conta.Saldo.Should().Be(100);
    }

    [Fact]
    public void Depositar_Com_Valor_Negativo_Deve_Lancar_Excecao()
    {
        var conta = new Conta("Ana Souza", "11111111111", "hash123", "salt123");

        Action act = () => conta.Depositar(-50);

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Valor do depósito deve ser positivo.");
    }

    [Fact]
    public void Sacar_Deve_Diminuir_Saldo()
    {
        var conta = new Conta("Ana Souza", "11111111111", "hash123", "salt123");
        conta.Depositar(200);
        conta.Sacar(50);

        conta.Saldo.Should().Be(150);
    }

    [Fact]
    public void Sacar_Com_Saldo_Insuficiente_Deve_Lancar_Excecao()
    {
        var conta = new Conta("Ana Souza", "11111111111", "hash123", "salt123");

        Action act = () => conta.Sacar(100);

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Saldo insuficiente.");
    }

    [Fact]
    public void Inativar_Deve_Tornar_Conta_Inativa()
    {
        var conta = new Conta("Ana Souza", "11111111111", "hash123", "salt123");
        conta.Inativar();

        conta.Ativo.Should().BeFalse();
    }
}
