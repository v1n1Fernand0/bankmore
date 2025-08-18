using BankMore.ContaCorrente.Application.Commands.InativarConta;
using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BankMore.Tests.ContaCorrente.Application.Commands;

public class InativarContaHandlerTests
{
    private readonly Mock<IContaCorrenteRepository> _repoMock;
    private readonly InativarContaHandler _handler;

    public InativarContaHandlerTests()
    {
        _repoMock = new Mock<IContaCorrenteRepository>();
        _handler = new InativarContaHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Deve_Falhar_Se_Conta_Nao_Encontrada()
    {
        var command = new InativarContaCommand(Guid.NewGuid(), "123");
        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Conta não encontrada");
    }

    [Fact]
    public async Task Deve_Falhar_Se_Senha_Invalida()
    {
        var salt = "salt";
        var senhaHash = BCrypt.Net.BCrypt.HashPassword("senhaCorreta" + salt);
        var conta = new Conta("Ana", "12345678901", senhaHash, salt);

        _repoMock.Setup(r => r.ObterPorIdAsync(conta.IdContaCorrente)).ReturnsAsync(conta);

        var command = new InativarContaCommand(conta.IdContaCorrente, "senhaErrada");
        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Senha inválida");
    }

    [Fact]
    public async Task Deve_Inativar_Conta_Com_Sucesso()
    {
        var conta = new Conta("Ana", "12345678901", "", "salt");
        conta = new Conta("Ana", "12345678901", BCrypt.Net.BCrypt.HashPassword("senha" + conta.Salt), conta.Salt);

        _repoMock.Setup(r => r.ObterPorIdAsync(conta.IdContaCorrente)).ReturnsAsync(conta);

        var command = new InativarContaCommand(conta.IdContaCorrente, "senha");
        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        conta.Ativo.Should().BeFalse();
        _repoMock.Verify(r => r.AtualizarAsync(conta), Times.Once);
    }
}
