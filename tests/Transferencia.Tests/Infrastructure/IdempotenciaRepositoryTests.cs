using BankMore.Transferencia.Domain.Entities;
using BankMore.Shared.Dapper;               
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using SQLitePCL;
using Transferencia.Infrastructure.Repositories;
using Xunit;

public class IdempotenciaRepositoryTests : IDisposable
{
    private const string Cs = "Data Source=file:bankmore-tests-idem?mode=memory&cache=shared";

    private readonly SqliteConnection _rootConn; 
    private readonly IdempotenciaRepository _repository;

    public IdempotenciaRepositoryTests()
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

        _repository = new IdempotenciaRepository(configuration);
    }

    private static void CriarTabelas(SqliteConnection conn)
    {
        conn.Execute("PRAGMA foreign_keys = ON;");
        conn.Execute(@"
            CREATE TABLE IF NOT EXISTS Idempotencias (
                Chave TEXT PRIMARY KEY,
                DataCriacao TEXT NOT NULL
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

    public void Dispose() => _rootConn.Dispose(); 
}
