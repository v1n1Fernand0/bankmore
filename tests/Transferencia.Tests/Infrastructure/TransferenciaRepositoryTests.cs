using BankMore.Transferencia.Domain.Entities;
using BankMore.Transferencia.Infrastructure.Repositories;
using BankMore.Shared.Dapper;            
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using SQLitePCL;
using Xunit;

public class TransferenciaRepositoryTests : IDisposable
{
    private const string Cs = "Data Source=file:bankmore-tests-transf?mode=memory&cache=shared";

    private readonly SqliteConnection _rootConn;
    private readonly TransferenciaRepository _repository;

    public TransferenciaRepositoryTests()
    {
        DapperGuidHandlersBootstrap.EnsureRegistered();

        Batteries.Init();

        _rootConn = new SqliteConnection(Cs);
        _rootConn.Open();

        CriarTabelas(_rootConn);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = Cs
            })
            .Build();

        _repository = new TransferenciaRepository(configuration);
    }

    private static void CriarTabelas(SqliteConnection conn)
    {
        conn.Execute("PRAGMA foreign_keys = ON;");
        conn.Execute(@"
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
            200m
        );

        await _repository.AdicionarAsync(transferencia);

        var encontrada = await _rootConn.QueryFirstOrDefaultAsync<TransferenciaRegistro>(
            "SELECT * FROM Transferencias WHERE Id = @Id",
            new { transferencia.Id });

        encontrada.Should().NotBeNull();
        encontrada!.Valor.Should().Be(200m);
        encontrada.NumeroContaDestino.Should().Be(123456);
    }

    public void Dispose() => _rootConn.Dispose();
}
