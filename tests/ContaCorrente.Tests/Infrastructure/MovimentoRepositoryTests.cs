using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Infrastructure.Repositories;
using BankMore.Shared.Dapper;
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using Xunit;

public class MovimentoRepositoryTests : IDisposable
{
    private readonly MovimentoRepository _repository;
    private readonly SqliteConnection _connection;
    private readonly string _connectionString = "DataSource=:memory:;Cache=Shared";

    public MovimentoRepositoryTests()
    {
        Batteries.Init();
        SqlMapper.AddTypeHandler(new GuidTypeHandler());

        _connection = new SqliteConnection(_connectionString);
        _connection.Open();

        CriarTabelas();

        _repository = new MovimentoRepository(_connection);
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
                Id TEXT PRIMARY KEY,
                ContaId TEXT NOT NULL,
                Idempotencia TEXT NOT NULL UNIQUE,
                Valor DECIMAL(18,2) NOT NULL,
                Tipo TEXT NOT NULL,
                DataCriacao TEXT NOT NULL,
                FOREIGN KEY (ContaId) REFERENCES ContaCorrente(IdContaCorrente)
            );
        ");
    }

    [Fact]
    public async Task Deve_Adicionar_E_Obter_Movimento_Por_Idempotencia()
    {
        var contaId = Guid.NewGuid();
        await CriarContaCorrenteFake(contaId);

        var movimento = new Movimento(contaId, Guid.NewGuid(), 150, "C");

        await _repository.AdicionarAsync(movimento);
        var encontrado = await _repository.ObterPorIdempotenciaAsync(movimento.Idempotencia);

        encontrado.Should().NotBeNull("O movimento deveria ter sido encontrado.");
        encontrado!.Valor.Should().Be(150);
    }

    [Fact]
    public async Task Deve_Listar_Movimentos_De_Uma_Conta()
    {
        var contaId = Guid.NewGuid();
        await CriarContaCorrenteFake(contaId);

        var mov1 = new Movimento(contaId, Guid.NewGuid(), 100, "C");
        var mov2 = new Movimento(contaId, Guid.NewGuid(), 50, "D");

        await _repository.AdicionarAsync(mov1);
        await _repository.AdicionarAsync(mov2);

        var movimentos = await _repository.ObterPorContaAsync(contaId);

        movimentos.Should().HaveCount(2, "Dois movimentos foram inseridos para essa conta.");
    }

    private async Task CriarContaCorrenteFake(Guid contaId)
    {
        await _connection.ExecuteAsync(@"
            INSERT INTO ContaCorrente (IdContaCorrente, Numero, Nome, Cpf, Ativo, SenhaHash, Salt, Saldo)
            VALUES (@Id, @Numero, @Nome, @Cpf, @Ativo, @SenhaHash, @Salt, @Saldo);",
            new
            {
                Id = contaId.ToString(),
                Numero = 123456,
                Nome = "Teste",
                Cpf = "00000000000",
                Ativo = 1,
                SenhaHash = "hash",
                Salt = "salt",
                Saldo = 0m
            });
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
