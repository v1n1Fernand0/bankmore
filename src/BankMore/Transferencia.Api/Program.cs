using BankMore.Transferencia.Application.Commands.EfetuarTransferencia;
using BankMore.Transferencia.Domain.Interfaces;
using BankMore.Transferencia.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.IdentityModel.Tokens;
using SQLitePCL;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
Batteries.Init();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<EfetuarTransferenciaCommand>());

builder.Services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();

builder.Services.AddHttpClient("ContaCorrenteApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ContaCorrenteApi:BaseUrl"]
        ?? "http://localhost:5001/");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var jwtSecret = builder.Configuration["Jwt:Secret"]!;
var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    using var connection = new SqliteConnection(connectionString);
    connection.Open();

    var cmd = connection.CreateCommand();
    cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS TransferenciaRegistro (
            Id TEXT PRIMARY KEY,
            ContaOrigemId TEXT NOT NULL,
            NumeroContaDestino INTEGER NOT NULL,
            Valor DECIMAL(18,2) NOT NULL,
            DataCriacao TEXT NOT NULL
        );
    ";
    cmd.ExecuteNonQuery();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
