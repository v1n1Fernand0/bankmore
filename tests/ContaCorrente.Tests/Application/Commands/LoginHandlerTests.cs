using BankMore.ContaCorrente.Application.Commands.Login;
using BankMore.ContaCorrente.Application.Dtos;
using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BankMore.Tests.ContaCorrente.Application.Commands;

public class LoginHandlerTests
{
    private readonly Mock<IContaCorrenteRepository> _repoMock;
    private readonly IConfiguration _config;
    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        _repoMock = new Mock<IContaCorrenteRepository>();
        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"Jwt:Secret", "supersecretkey123"},
                {"Jwt:ExpireMinutes", "60"}
            })
            .Build();

        _handler = new LoginHandler(_repoMock.Object, _config);
    }

    [Fact]
    public async Task Deve_Falhar_Se_Conta_Nao_Existir()
    {
        var command = new LoginCommand("12345678901", "senha");
        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Conta não encontrada");
    }

    [Fact]
    public async Task Deve_Falhar_Se_Senha_Invalida()
    {
        var conta = new Conta("Ana", "12345678901", BCrypt.Net.BCrypt.HashPassword("erradoSalt"), "salt");
        _repoMock.Setup(r => r.ObterPorCpfAsync("12345678901")).ReturnsAsync(conta);

        var command = new LoginCommand("12345678901", "senhaErrada");
        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Senha inválida");
    }

    [Fact]
    public async Task Deve_Logar_E_Retornar_Token()
    {
        var salt = Guid.NewGuid().ToString("N");
        var senhaHash = BCrypt.Net.BCrypt.HashPassword("senha" + salt);

        var conta = new Conta("Ana", "12345678901", senhaHash, salt);

        _repoMock.Setup(r => r.ObterPorCpfAsync("12345678901"))
            .ReturnsAsync(conta);

        var inMemorySettings = new Dictionary<string, string>
    {
        {"Jwt:Secret", "12345678901234567890123456789012"}, 
        {"Jwt:ExpireMinutes", "60"}
    };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var handler = new LoginHandler(_repoMock.Object, configuration);

        var command = new LoginCommand("12345678901", "senha");

        var result = await handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Token.Should().NotBeNullOrWhiteSpace();
        result.Value.Should().BeOfType<LoginResponseDto>();
    }

}
