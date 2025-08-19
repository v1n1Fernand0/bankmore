using BankMore.Transferencia.Application.Commands.EfetuarTransferencia;
using BankMore.Transferencia.Domain.Entities;
using BankMore.Transferencia.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Moq.Protected;
using System.Net;

public class EfetuarTransferenciaHandlerTests
{
    private readonly Mock<ITransferenciaRepository> _repoMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly EfetuarTransferenciaHandler _handler;

    public EfetuarTransferenciaHandlerTests()
    {
        _repoMock = new Mock<ITransferenciaRepository>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var httpClient = new HttpClient(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost:5001/")
        };

        _httpClientFactoryMock.Setup(f => f.CreateClient("ContaCorrenteApi"))
            .Returns(httpClient);

        _handler = new EfetuarTransferenciaHandler(_repoMock.Object, _httpClientFactoryMock.Object);
    }

    [Fact]
    public async Task Deve_Falhar_Se_Valor_Invalido()
    {
        var cmd = new EfetuarTransferenciaCommand
        {
            ContaOrigemId = Guid.NewGuid(),
            NumeroContaDestino = 123,
            Valor = 0,
            Idempotencia = Guid.NewGuid(),
            JwtToken = "fake"
        };

        var result = await _handler.Handle(cmd, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("INVALID_VALUE");
    }

    [Fact]
    public async Task Deve_Sucesso_Se_Debito_Credito_OK()
    {
        var cmd = new EfetuarTransferenciaCommand
        {
            ContaOrigemId = Guid.NewGuid(),
            NumeroContaDestino = 123,
            Valor = 100,
            Idempotencia = Guid.NewGuid(),
            JwtToken = "fake"
        };

        var result = await _handler.Handle(cmd, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _repoMock.Verify(r => r.AdicionarAsync(It.IsAny<TransferenciaRegistro>()), Times.Once);
    }
}
