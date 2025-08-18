using BankMore.ContaCorrente.Application.Commands.CadastrarConta;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Infrastructure.Repositories;
using BankMore.Shared.Dapper;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; 
using SQLitePCL;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
Batteries.Init();
SqlMapper.AddTypeHandler(new GuidTypeHandler());

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CadastrarContaCommand>());
builder.Services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();
builder.Services.AddScoped<IMovimentoRepository, MovimentoRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Conta Corrente API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite ** {seu_token}**"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    };

    c.AddSecurityRequirement(securityRequirement);
});
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

    if (app.Environment.IsDevelopment())
    {
        cmd.CommandText = @"DROP TABLE IF EXISTS Movimento;";
        cmd.ExecuteNonQuery();
        cmd.CommandText = @"DROP TABLE IF EXISTS ContaCorrente;";
        cmd.ExecuteNonQuery();
    }

    cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS ContaCorrente (
            IdContaCorrente TEXT PRIMARY KEY,
            Numero INTEGER NOT NULL,
            Nome TEXT NOT NULL,
            Cpf TEXT NOT NULL UNIQUE,
            Ativo INTEGER NOT NULL,
            SenhaHash TEXT NOT NULL,
            Salt TEXT NOT NULL,
            Saldo REAL NOT NULL
        );
    ";
    cmd.ExecuteNonQuery();

    cmd.CommandText = "CREATE INDEX IF NOT EXISTS IX_ContaCorrente_Numero ON ContaCorrente (Numero);";
    cmd.ExecuteNonQuery();
    cmd.CommandText = "CREATE INDEX IF NOT EXISTS IX_ContaCorrente_Cpf ON ContaCorrente (Cpf);";
    cmd.ExecuteNonQuery();

    cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS Movimento (
            Id TEXT PRIMARY KEY,
            ContaId TEXT NOT NULL,
            Idempotencia TEXT NOT NULL UNIQUE,
            Valor REAL NOT NULL,
            Tipo TEXT NOT NULL,
            DataCriacao TEXT NOT NULL,
            FOREIGN KEY (ContaId) REFERENCES ContaCorrente(IdContaCorrente)
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
