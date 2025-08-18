using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Infrastructure.Repositories;
using BankMore.Shared.Dapper;
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using Xunit;

public class ContaCorrenteRepositoryTests : IDisposable
{
    private readonly ContaCorrenteRepository _repository;
    private readonly SqliteConnection _connection;
    private readonly string _connectionString = "DataSource=:memory:;Cache=Shared";

    public ContaCorrenteRepositoryTests()
    {
        Batteries.Init();
        SqlMapper.AddTypeHandler(new GuidTypeHandler()); 

        _connection = new SqliteConnection(_connectionString);
        _connection.Open();

        CriarTabelas();

        _repository = new ContaCorrenteRepository(_connection);
    }

    private void CriarTabelas()
    {
        _connection.Execute(@"
            CREATE TABLE IF NOT EXISTS ContaCorrente (
                IdContaCorrente TEXT PRIMARY KEY,
                Numero INTEGER NOT NULL,
                Nome TEXT NOT NULL,
                Cpf TEXT NOT NULL,
                Ativo INTEGER NOT NULL,
                SenhaHash TEXT NOT NULL,
                Salt TEXT NOT NULL,
                Saldo DECIMAL(18,2) NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Movimento (
                IdMovimento TEXT PRIMARY KEY,
                IdContaCorrente TEXT NOT NULL,
                Tipo TEXT NOT NULL,
                Valor DECIMAL(18,2) NOT NULL,
                DataMovimento TEXT NOT NULL
            );
        ");
    }

    [Fact]
    public async Task Deve_Adicionar_E_Obter_Conta_Por_Id()
    {
        var conta = new Conta("Ana Souza", "11111111111", "hash123", "salt123");

        await _repository.AdicionarAsync(conta);
        var encontrada = await _repository.ObterPorIdAsync(conta.IdContaCorrente);

        encontrada.Should().NotBeNull();
        encontrada!.Cpf.Should().Be("11111111111");
    }

    [Fact]
    public async Task Deve_Atualizar_Saldo_E_Persistir()
    {
        var conta = new Conta("Ana Souza", "11111111111", "hash123", "salt123");
        await _repository.AdicionarAsync(conta);

        conta.Depositar(200);
        await _repository.AtualizarAsync(conta);

        var atualizada = await _repository.ObterPorIdAsync(conta.IdContaCorrente);
        atualizada!.Saldo.Should().Be(200);
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
