using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace BankMore.ContaCorrente.Infrastructure.Repositories;

public class ContaCorrenteRepository : IContaCorrenteRepository
{
    private readonly string _connectionString;

    public ContaCorrenteRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private IDbConnection Connection => new SqliteConnection(_connectionString);

    public async Task AdicionarAsync(Conta conta)
    {
        const string sql = @"INSERT INTO ContaCorrente 
            (IdContaCorrente, Numero, Nome, Cpf, Ativo, SenhaHash, Salt, Saldo) 
            VALUES (@IdContaCorrente, @Numero, @Nome, @Cpf, @Ativo, @SenhaHash, @Salt, @Saldo)";

        using var conn = Connection;
        await conn.ExecuteAsync(sql, conta);
    }

    public async Task<Conta?> ObterPorIdAsync(Guid id)
    {
        const string sql = "SELECT * FROM ContaCorrente WHERE IdContaCorrente = @id";
        using var conn = Connection;
        return await conn.QuerySingleOrDefaultAsync<Conta>(sql, new { id });
    }

    public async Task<Conta?> ObterPorNumeroAsync(int numero)
    {
        const string sql = "SELECT * FROM ContaCorrente WHERE Numero = @numero";
        using var conn = Connection;
        return await conn.QuerySingleOrDefaultAsync<Conta>(sql, new { numero });
    }

    public async Task<Conta?> ObterPorCpfAsync(string cpf)
    {
        const string sql = "SELECT * FROM ContaCorrente WHERE Cpf = @cpf";
        using var conn = Connection;
        return await conn.QuerySingleOrDefaultAsync<Conta>(sql, new { cpf });
    }

    public async Task AtualizarAsync(Conta conta)
    {
        const string sql = @"UPDATE ContaCorrente 
            SET Nome = @Nome, 
                Ativo = @Ativo, 
                SenhaHash = @SenhaHash, 
                Salt = @Salt, 
                Saldo = @Saldo
            WHERE IdContaCorrente = @IdContaCorrente";

        using var conn = Connection;
        await conn.ExecuteAsync(sql, conta);
    }
}
