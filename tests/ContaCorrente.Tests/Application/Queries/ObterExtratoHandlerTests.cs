using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using ContaCorrente.Application.Queries.ObterExtrato;
using FluentAssertions;
using Moq;

namespace BankMore.Tests.ContaCorrente.Application.Queries;

public class ObterExtratoHandlerTests
{
    private readonly Mock<IMovimentoRepository> _movimentoRepoMock;
    private readonly ObterExtratoHandler _handler;

    public ObterExtratoHandlerTests()
    {
        _movimentoRepoMock = new Mock<IMovimentoRepository>();
        _handler = new ObterExtratoHandler(_movimentoRepoMock.Object);
    }

    [Fact]
    public async Task Deve_Retornar_Extrato_Ordenado_Por_Data()
    {
        var contaId = Guid.NewGuid();

        var movimentos = new List<Movimento>
        {
            new Movimento(contaId, Guid.NewGuid(), 100, "C"),
            new Movimento(contaId, Guid.NewGuid(), 50, "D"),
            new Movimento(contaId, Guid.NewGuid(), 200, "C")
        };

        _movimentoRepoMock
            .Setup(m => m.ObterPorContaAsync(contaId))
            .ReturnsAsync(movimentos);

        var query = new ObterExtratoQuery(contaId);
        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);

        var ordered = result.Value.ToList();
        ordered.Should().BeInDescendingOrder(m => m.DataCriacao);
    }
}
