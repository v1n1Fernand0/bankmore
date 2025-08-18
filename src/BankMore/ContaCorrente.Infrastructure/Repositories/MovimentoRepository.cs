using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace BankMore.ContaCorrente.Infrastructure.Repositories;

public class MovimentoRepository : IMovimentoRepository
{
    private readonly string _connectionString;

    public MovimentoRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")!;
        EnsureTable();
    }

    private void EnsureTable()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Movimento (
                Id TEXT PRIMARY KEY,
                ContaId TEXT NOT NULL,
                Idempotencia TEXT NOT NULL UNIQUE,
                Valor DECIMAL(18,2) NOT NULL,
                Tipo TEXT NOT NULL,
                DataCriacao TEXT NOT NULL,
                FOREIGN KEY (ContaId) REFERENCES Conta(IdContaCorrente)
            );");
    }

    public async Task<Movimento?> ObterPorIdempotenciaAsync(Guid idempotencia)
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Movimento>(
            "SELECT * FROM Movimento WHERE Idempotencia = @Idempotencia",
            new { Idempotencia = idempotencia });
    }

    public async Task<IEnumerable<Movimento>> ObterPorContaAsync(Guid contaId)
    {
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<Movimento>(
            "SELECT * FROM Movimento WHERE ContaId = @ContaId ORDER BY DataCriacao DESC",
            new { ContaId = contaId });
    }

    public async Task AdicionarAsync(Movimento movimento)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(@"
            INSERT INTO Movimento (Id, ContaId, Idempotencia, Valor, Tipo, DataCriacao)
            VALUES (@Id, @ContaId, @Idempotencia, @Valor, @Tipo, @DataCriacao)",
            movimento);
    }
}
