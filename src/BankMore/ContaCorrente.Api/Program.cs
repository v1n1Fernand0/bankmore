using BankMore.ContaCorrente.Application.Commands.CadastrarConta;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CadastrarContaCommand>());
builder.Services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();

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
        CREATE TABLE IF NOT EXISTS ContaCorrente (
            Id TEXT PRIMARY KEY,
            Numero INTEGER NOT NULL,
            Cpf TEXT NOT NULL,
            Nome TEXT NOT NULL,
            Saldo REAL NOT NULL,
            Ativo INTEGER NOT NULL
        );
    ";
    cmd.ExecuteNonQuery();

    cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS Movimento (
            Id TEXT PRIMARY KEY,
            ContaId TEXT NOT NULL,
            Valor REAL NOT NULL,
            Tipo TEXT NOT NULL, -- 'D' ou 'C'
            DataRegistro TEXT NOT NULL,
            Idempotencia TEXT NOT NULL,
            FOREIGN KEY (ContaId) REFERENCES ContaCorrente(Id)
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
