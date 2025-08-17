using BankMore.Transferencia.Domain.Entities;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Transferencia.Domain.Interfaces;

namespace Transferencia.Infrastructure.Repositories;

public class IdempotenciaRepository : IIdempotenciaRepository
{
    private readonly string _connectionString;

    public IdempotenciaRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<Idempotencia?> ObterAsync(Guid chave)
    {
        using var connection = new SqliteConnection(_connectionString);

        var sql = "SELECT Chave, CriadoEm FROM Idempotencias WHERE Chave = @Chave LIMIT 1";
        return await connection.QueryFirstOrDefaultAsync<Idempotencia>(sql, new { Chave = chave });
    }

    public async Task SalvarAsync(Idempotencia idempotencia)
    {
        using var connection = new SqliteConnection(_connectionString);

        var sql = "INSERT INTO Idempotencias (Chave, CriadoEm) VALUES (@Chave, @CriadoEm)";
        await connection.ExecuteAsync(sql, idempotencia);
    }
}
