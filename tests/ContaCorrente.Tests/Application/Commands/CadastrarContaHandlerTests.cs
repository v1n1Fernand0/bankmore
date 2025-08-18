using BankMore.ContaCorrente.Application.Commands.CadastrarConta;
using BankMore.ContaCorrente.Application.Dtos;
using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BankMore.Tests.ContaCorrente.Application.Commands;

public class CadastrarContaHandlerTests
{
    private readonly Mock<IContaCorrenteRepository> _repoMock;
    private readonly CadastrarContaHandler _handler;

    public CadastrarContaHandlerTests()
    {
        _repoMock = new Mock<IContaCorrenteRepository>();
        _handler = new CadastrarContaHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Deve_Falhar_Se_Cpf_Invalido()
    {
        var command = new CadastrarContaCommand("Ana", "123", "senha");
        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("CPF inválido");
    }

    [Fact]
    public async Task Deve_Cadastrar_Conta_Com_Sucesso()
    {
        var command = new CadastrarContaCommand("Ana", "12345678901", "senha");
        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<ContaCorrenteDto>();

        _repoMock.Verify(r => r.AdicionarAsync(It.IsAny<Conta>()), Times.Once);
    }
}
