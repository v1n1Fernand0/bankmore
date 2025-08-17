using MediatR;
using BankMore.Shared;
using BankMore.Transferencia.Domain.Interfaces;
using BankMore.Transferencia.Domain.Entities;
using System.Net.Http.Json;
using System.Net.Http;

namespace BankMore.Transferencia.Application.Commands.EfetuarTransferencia;

public sealed class EfetuarTransferenciaHandler
    : IRequestHandler<EfetuarTransferenciaCommand, Result<Unit>>
{
    private readonly ITransferenciaRepository _repository;
    private readonly HttpClient _http;

    public EfetuarTransferenciaHandler(ITransferenciaRepository repository, IHttpClientFactory httpClientFactory)
    {
        _repository = repository;
        _http = httpClientFactory.CreateClient("ContaCorrenteApi");
    }

    public async Task<Result<Unit>> Handle(EfetuarTransferenciaCommand request, CancellationToken ct)
    {
        if (request.Valor <= 0)
            return Result<Unit>.Fail("INVALID_VALUE: Valor deve ser positivo");

        var debito = new
        {
            Idempotencia = request.Idempotencia,
            NumeroConta = (int?)null,
            Valor = request.Valor,
            Tipo = 'D'
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/contas/movimentar")
        {
            Content = JsonContent.Create(debito)
        };
        httpRequest.Headers.Add("Authorization", $"Bearer {request.JwtToken}");

        var debitoResponse = await _http.SendAsync(httpRequest, ct);
        if (!debitoResponse.IsSuccessStatusCode)
            return Result<Unit>.Fail("TRANSFER_ERROR: Falha ao debitar conta origem");

        var credito = new
        {
            Idempotencia = Guid.NewGuid(),
            NumeroConta = request.NumeroContaDestino,
            Valor = request.Valor,
            Tipo = 'C'
        };

        httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/contas/movimentar")
        {
            Content = JsonContent.Create(credito)
        };
        httpRequest.Headers.Add("Authorization", $"Bearer {request.JwtToken}");

        var creditoResponse = await _http.SendAsync(httpRequest, ct);

        if (!creditoResponse.IsSuccessStatusCode)
        {
            var estorno = new
            {
                Idempotencia = Guid.NewGuid(),
                NumeroConta = (int?)null,
                Valor = request.Valor,
                Tipo = 'C'
            };

            var estornoReq = new HttpRequestMessage(HttpMethod.Post, "api/contas/movimentar")
            {
                Content = JsonContent.Create(estorno)
            };
            estornoReq.Headers.Add("Authorization", $"Bearer {request.JwtToken}");
            await _http.SendAsync(estornoReq, ct);

            return Result<Unit>.Fail("TRANSFER_ERROR: Falha ao creditar conta destino");
        }

        var transferencia = new TransferenciaRegistro(request.ContaOrigemId, Guid.NewGuid(), request.Valor);
        await _repository.AdicionarAsync(transferencia);

        return Result<Unit>.Ok(Unit.Value);
    }
}
