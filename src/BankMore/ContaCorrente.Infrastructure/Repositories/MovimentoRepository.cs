using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using Dapper;
using System.Data;

namespace BankMore.ContaCorrente.Infrastructure.Repositories;

public class MovimentoRepository : IMovimentoRepository
{
    private readonly IDbConnection _connection;

    public MovimentoRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Movimento?> ObterPorIdempotenciaAsync(Guid idempotencia)
    {
        return await _connection.QueryFirstOrDefaultAsync<Movimento>(
            "SELECT * FROM Movimento WHERE Idempotencia = @Idempotencia",
            new { Idempotencia = idempotencia.ToString() } 
        );
    }

    public async Task<IEnumerable<Movimento>> ObterPorContaAsync(Guid contaId)
    {
        return await _connection.QueryAsync<Movimento>(
            "SELECT * FROM Movimento WHERE ContaId = @ContaId ORDER BY DataCriacao DESC",
            new { ContaId = contaId.ToString() }  
        );
    }

    public async Task AdicionarAsync(Movimento movimento)
    {
        await _connection.ExecuteAsync(@"
            INSERT INTO Movimento (Id, ContaId, Idempotencia, Valor, Tipo, DataCriacao)
            VALUES (@Id, @ContaId, @Idempotencia, @Valor, @Tipo, @DataCriacao)",
            new
            {
                Id = movimento.Id.ToString(),
                ContaId = movimento.ContaId.ToString(),
                Idempotencia = movimento.Idempotencia.ToString(),
                Valor = movimento.Valor,
                Tipo = movimento.Tipo,
                DataCriacao = movimento.DataCriacao.ToString("s")
            });
    }
}
