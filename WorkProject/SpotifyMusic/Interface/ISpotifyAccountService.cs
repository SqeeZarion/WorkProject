namespace SpotifyWebApi.Interface;

public interface ISpotifyAccountService
{
    Task<string> GetToken (string? clientId, string? clientSecret);
}