using BankMore.ContaCorrente.Domain.Entities;
using FluentAssertions;

namespace BankMore.Tests.ContaCorrente.Domain.Entities;

public class IdempotenciaTests
{
    [Fact]
    public void Deve_Criar_Idempotencia_Com_Dados_Corretos()
    {
        var chave = Guid.NewGuid();
        var idem = new Idempotencia(chave, "request-data", "response-data");

        idem.ChaveIdempotencia.Should().Be(chave);
        idem.Requisicao.Should().Be("request-data");
        idem.Resultado.Should().Be("response-data");
    }
}
