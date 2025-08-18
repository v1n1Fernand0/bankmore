using BankMore.Transferencia.Domain.Entities;
using BankMore.Transferencia.Domain.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BankMore.Transferencia.Infrastructure.Repositories;

public class TransferenciaRepository : ITransferenciaRepository
{
    private readonly string _connectionString;

    public TransferenciaRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private IDbConnection Connection => new SqliteConnection(_connectionString);

    public async Task AdicionarAsync(TransferenciaRegistro transferencia)
    {
        const string sql = @"
            INSERT INTO Transferencias 
                (Id, ContaOrigemId, NumeroContaDestino, Valor, DataCriacao) 
            VALUES 
                (@Id, @ContaOrigemId, @NumeroContaDestino, @Valor, @DataCriacao);";

        using var conn = Connection;
        await conn.ExecuteAsync(sql, new
        {
            transferencia.Id,
            transferencia.ContaOrigemId,
            transferencia.NumeroContaDestino,
            transferencia.Valor,
            DataCriacao = transferencia.DataCriacao
        });
    }
}
