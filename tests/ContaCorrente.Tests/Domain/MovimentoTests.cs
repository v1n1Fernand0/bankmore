using BankMore.ContaCorrente.Domain.Entities;
using FluentAssertions;

namespace BankMore.Tests.ContaCorrente.Domain.Entities;

public class MovimentoTests
{
    [Fact]
    public void Deve_Criar_Movimento_Com_Dados_Corretos()
    {
        var contaId = Guid.NewGuid();
        var idem = Guid.NewGuid();

        var movimento = new Movimento(contaId, idem, 100, "C");

        movimento.ContaId.Should().Be(contaId);
        movimento.Idempotencia.Should().Be(idem);
        movimento.Valor.Should().Be(100);
        movimento.Tipo.Should().Be("C");
        movimento.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
