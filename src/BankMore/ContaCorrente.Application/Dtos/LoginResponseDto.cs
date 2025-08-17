namespace BankMore.ContaCorrente.Application.Dtos;

public sealed class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
}
