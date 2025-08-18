using BankMore.Transferencia.Domain.Entities;
using BankMore.Transferencia.Infrastructure.Repositories;
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using SQLitePCL;
using Xunit;

public class TransferenciaRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly TransferenciaRepository _repository;
    private readonly string _connectionString = "DataSource=:memory:";

    public TransferenciaRepositoryTests()
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

        _repository = new TransferenciaRepository(configuration);
    }

    private void CriarTabelas()
    {
        _connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Transferencias (
                Id TEXT PRIMARY KEY,
                ContaOrigemId TEXT NOT NULL,
                NumeroContaDestino INTEGER NOT NULL,
                Valor DECIMAL(18,2) NOT NULL,
                DataCriacao TEXT NOT NULL
            );
        ");
    }

    [Fact]
    public async Task Deve_Salvar_Transferencia()
    {
        var transferencia = new TransferenciaRegistro(
            Guid.NewGuid(),
            123456,
            200
        );

        await _repository.AdicionarAsync(transferencia);

        var encontrada = await _connection.QueryFirstOrDefaultAsync<TransferenciaRegistro>(
            "SELECT * FROM Transferencias WHERE Id = @Id",
            new { transferencia.Id });

        encontrada.Should().NotBeNull();
        encontrada!.Valor.Should().Be(200);
        encontrada.NumeroContaDestino.Should().Be(123456);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
