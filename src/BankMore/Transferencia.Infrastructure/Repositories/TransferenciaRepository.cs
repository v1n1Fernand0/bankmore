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
        const string sql = @"INSERT INTO transferencia 
            (idtransferencia, idcontaorigem, idcontadestino, valor, data) 
            VALUES (@IdTransferencia, @IdContaOrigem, @IdContaDestino, @Valor, @Data)";

        using var conn = Connection;
        await conn.ExecuteAsync(sql, transferencia);
    }
}
