using BankMore.ContaCorrente.Application.Commands.MovimentarConta;
using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BankMore.Tests.ContaCorrente.Application.Commands;

public class MovimentarContaHandlerTests
{
    private readonly Mock<IContaCorrenteRepository> _contaRepoMock;
    private readonly Mock<IMovimentoRepository> _movRepoMock;
    private readonly MovimentarContaHandler _handler;

    public MovimentarContaHandlerTests()
    {
        _contaRepoMock = new Mock<IContaCorrenteRepository>();
        _movRepoMock = new Mock<IMovimentoRepository>();
        _handler = new MovimentarContaHandler(_contaRepoMock.Object, _movRepoMock.Object);
    }

    [Fact]
    public async Task Deve_Falhar_Se_Conta_Nao_Encontrada()
    {
        var command = new MovimentarContaCommand(Guid.NewGuid(), 12345, 100, "C", Guid.NewGuid());
        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("ACCOUNT_NOT_FOUND");
    }

    [Fact]
    public async Task Deve_Falhar_Se_Fundos_Insuficientes()
    {
        var conta = new Conta("Ana", "12345678901", "", "salt");
        _contaRepoMock.Setup(r => r.ObterPorIdAsync(conta.IdContaCorrente)).ReturnsAsync(conta);
        _movRepoMock.Setup(r => r.ObterPorContaAsync(conta.IdContaCorrente)).ReturnsAsync(new List<Movimento>());

        var command = new MovimentarContaCommand(Guid.NewGuid(), 12345, 100, "D", conta.IdContaCorrente);
        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("INSUFFICIENT_FUNDS");
    }

    [Fact]
    public async Task Deve_Creditar_Com_Sucesso()
    {
        var conta = new Conta("Ana", "12345678901", "", "salt");
        _contaRepoMock.Setup(r => r.ObterPorIdAsync(conta.IdContaCorrente)).ReturnsAsync(conta);

        var command = new MovimentarContaCommand(Guid.NewGuid(), 12345, 200, "C", conta.IdContaCorrente);
        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        _movRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Movimento>()), Times.Once);
    }
}
