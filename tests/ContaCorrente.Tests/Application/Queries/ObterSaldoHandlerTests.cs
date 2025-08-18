using BankMore.ContaCorrente.Application.Queries.ObterSaldo;
using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BankMore.Tests.ContaCorrente.Application.Queries;

public class ObterSaldoHandlerTests
{
    private readonly Mock<IContaCorrenteRepository> _contaRepoMock;
    private readonly Mock<IMovimentoRepository> _movimentoRepoMock;
    private readonly ObterSaldoHandler _handler;

    public ObterSaldoHandlerTests()
    {
        _contaRepoMock = new Mock<IContaCorrenteRepository>();
        _movimentoRepoMock = new Mock<IMovimentoRepository>();
        _handler = new ObterSaldoHandler(_contaRepoMock.Object, _movimentoRepoMock.Object);
    }

    [Fact]
    public async Task Deve_Falhar_Se_Conta_Nao_Existir()
    {
        var contaId = Guid.NewGuid();
        _contaRepoMock.Setup(r => r.ObterPorIdAsync(contaId)).ReturnsAsync((Conta?)null);

        var query = new ObterSaldoQuery(contaId);
        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Conta não encontrada");
    }

    [Fact]
    public async Task Deve_Falhar_Se_Conta_Inativa()
    {
        var conta = new Conta("Ana", "11111111111", "hash", "salt");
        conta.Inativar();

        _contaRepoMock.Setup(r => r.ObterPorIdAsync(conta.IdContaCorrente)).ReturnsAsync(conta);

        var query = new ObterSaldoQuery(conta.IdContaCorrente);
        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("inativa");
    }

    [Fact]
    public async Task Deve_Calcular_Saldo_Com_Depositos_E_Saques()
    {
        var conta = new Conta("Ana", "11111111111", "hash", "salt");

        _contaRepoMock.Setup(r => r.ObterPorIdAsync(conta.IdContaCorrente)).ReturnsAsync(conta);

        var movimentos = new List<Movimento>
        {
            new Movimento(conta.IdContaCorrente, Guid.NewGuid(), 100, "C"),
            new Movimento(conta.IdContaCorrente, Guid.NewGuid(), 40, "D")
        };

        _movimentoRepoMock.Setup(r => r.ObterPorContaAsync(conta.IdContaCorrente)).ReturnsAsync(movimentos);

        var query = new ObterSaldoQuery(conta.IdContaCorrente);
        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Valor.Should().Be(60);
        result.Value.Numero.Should().Be(conta.Numero);
        result.Value.Nome.Should().Be("Ana");
    }
}
