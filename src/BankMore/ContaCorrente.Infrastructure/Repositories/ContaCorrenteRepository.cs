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
        const string sql = @"INSERT INTO contacorrente 
            (idcontacorrente, numero, nome, ativo, senha, salt) 
            VALUES (@IdContaCorrente, @Numero, @Nome, @Ativo, @SenhaHash, @Salt)";

        using var conn = Connection;
        await conn.ExecuteAsync(sql, conta);
    }

    public async Task<Conta?> ObterPorIdAsync(Guid id)
    {
        const string sql = "SELECT * FROM contacorrente WHERE idcontacorrente = @id";
        using var conn = Connection;
        return await conn.QuerySingleOrDefaultAsync<Conta>(sql, new { id });
    }

    public async Task<Conta?> ObterPorNumeroAsync(int numero)
    {
        const string sql = "SELECT * FROM contacorrente WHERE numero = @numero";
        using var conn = Connection;
        return await conn.QuerySingleOrDefaultAsync<Conta>(sql, new { numero });
    }

    public async Task<Conta?> ObterPorCpfAsync(string cpf)
    {
        const string sql = "SELECT * FROM contacorrente WHERE cpf = @cpf";
        using var conn = Connection;
        return await conn.QuerySingleOrDefaultAsync<Conta>(sql, new { cpf });
    }

    public async Task AtualizarAsync(Conta conta)
    {
        const string sql = @"UPDATE contacorrente 
            SET nome = @Nome, ativo = @Ativo, senha = @SenhaHash, salt = @Salt 
            WHERE idcontacorrente = @IdContaCorrente";

        using var conn = Connection;
        await conn.ExecuteAsync(sql, conta);
    }
}
