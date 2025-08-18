using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using Dapper;
using System.Data;

namespace BankMore.ContaCorrente.Infrastructure.Repositories;

public class ContaCorrenteRepository : IContaCorrenteRepository
{
    private readonly IDbConnection _connection;

    public ContaCorrenteRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task AdicionarAsync(Conta conta)
    {
        const string sql = @"INSERT INTO ContaCorrente 
            (IdContaCorrente, Numero, Nome, Cpf, Ativo, SenhaHash, Salt, Saldo) 
            VALUES (@IdContaCorrente, @Numero, @Nome, @Cpf, @Ativo, @SenhaHash, @Salt, @Saldo)";

        await _connection.ExecuteAsync(sql, conta);
    }

    public async Task<Conta?> ObterPorIdAsync(Guid id)
    {
        const string sql = "SELECT * FROM ContaCorrente WHERE IdContaCorrente = @id";
        return await _connection.QuerySingleOrDefaultAsync<Conta>(sql, new { id });
    }

    public async Task<Conta?> ObterPorNumeroAsync(int numero)
    {
        const string sql = "SELECT * FROM ContaCorrente WHERE Numero = @numero";
        return await _connection.QuerySingleOrDefaultAsync<Conta>(sql, new { numero });
    }

    public async Task<Conta?> ObterPorCpfAsync(string cpf)
    {
        const string sql = "SELECT * FROM ContaCorrente WHERE Cpf = @cpf";
        return await _connection.QuerySingleOrDefaultAsync<Conta>(sql, new { cpf });
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

        await _connection.ExecuteAsync(sql, conta);
    }
}
