FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY BankMore.sln ./
COPY Transferencia.Api/Transferencia.Api.csproj Transferencia.Api/
COPY Transferencia.Application/Transferencia.Application.csproj Transferencia.Application/
COPY Transferencia.Domain/Transferencia.Domain.csproj Transferencia.Domain/
COPY Transferencia.Infrastructure/Transferencia.Infrastructure.csproj Transferencia.Infrastructure/
COPY BankMore.Shared/BankMore.Shared.csproj BankMore.Shared/
COPY ContaCorrente.Api/ContaCorrente.Api.csproj ContaCorrente.Api/
COPY ContaCorrente.Application/ContaCorrente.Application.csproj ContaCorrente.Application/
COPY ContaCorrente.Domain/ContaCorrente.Domain.csproj ContaCorrente.Domain/
COPY ContaCorrente.Infrastructure/ContaCorrente.Infrastructure.csproj ContaCorrente.Infrastructure/

RUN dotnet restore BankMore.sln

COPY . .

WORKDIR "/src/Transferencia.Api"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "Transferencia.Api.dll"]
