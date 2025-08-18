using BankMore.Transferencia.Domain.Entities;
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using SQLitePCL;
using Transferencia.Infrastructure.Repositories;
using Xunit;

public class IdempotenciaRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly IdempotenciaRepository _repository;
    private readonly string _connectionString = "DataSource=:memory:";

    public IdempotenciaRepositoryTests()
    {
        Batteries.Init();

        _connection = new SqliteConnection(_connectionString);
        _connection.Open();

        CriarTabelas();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", _connectionString }
            })
            .Build();

        _repository = new IdempotenciaRepository(configuration);
    }

    private void CriarTabelas()
    {
        _connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Idempotencias (
                Chave TEXT PRIMARY KEY,
                CriadoEm TEXT NOT NULL
            );
        ");
    }

    [Fact]
    public async Task Deve_Salvar_E_Obter_Idempotencia()
    {
        var chave = Guid.NewGuid();
        var idempotencia = new Idempotencia(chave);

        await _repository.SalvarAsync(idempotencia);
        var encontrado = await _repository.ObterAsync(chave);

        encontrado.Should().NotBeNull();
        encontrado!.Chave.Should().Be(chave);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
