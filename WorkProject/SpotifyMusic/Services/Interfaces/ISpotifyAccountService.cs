namespace WorkProject.Services.Interfaces;

public interface ISpotifyAccountService
{
    Task<string> GetAccessTokenAsync();
    Task<bool> ValidateTokenAsync(string token);
    Task<string> RefreshTokenAsync(string refreshToken);
} 