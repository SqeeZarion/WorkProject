using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SpotifyWebApi.Database;
using WorkProject.Auth.Interface;
using WorkProject.Auth.Models;

namespace WorkProject.Auth.Service;

public class TokenService : ITokenService
{
    private readonly HttpClient
        _httpClient; //для здійснення HTTP-запитів до зовнішніх веб-сервісів, таких як Spotify API.

    private readonly IConfiguration _configuration; 
    private string _currentAccessToken;
    private DateTime _tokenExpiryTime;
    private readonly DbConnection _db;

    public TokenService(HttpClient httpClient, IConfiguration configuration, DbConnection db)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _db = db;
    }

    public async Task<string> GetAccessTokenAsync(int userId = 0, CancellationToken cancellationToken = default)
    {
        // Якщо токен ще живий — повертаємо кешований
        if (DateTime.UtcNow < _tokenExpiryTime && !string.IsNullOrEmpty(_currentAccessToken))
            return _currentAccessToken;
        
        // Шукаємо користувача
        var user = await _db.Users.SingleOrDefaultAsync(u => u.UserId == userId, cancellationToken);
        if (user == null || string.IsNullOrEmpty(user.RefreshToken))
            throw new InvalidOperationException("User not found or has no refresh token.");

        var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
        {
            //convert format for dictionary
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", user.RefreshToken },
                { "client_id", _configuration["Spotify:ClientId"] ?? throw new InvalidOperationException("Missing Spotify:ClientId") },
                { "client_secret", _configuration["Spotify:ClientSecret"] ?? throw new InvalidOperationException("Missing Spotify:ClientSecret") }
            })
        };

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        var tokenData = JsonSerializer.Deserialize<TokenResponse>(responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (tokenData == null || string.IsNullOrEmpty(tokenData.AccessToken))
            throw new Exception("Failed to refresh access token from Spotify.");

        //update cash
        _currentAccessToken = tokenData.AccessToken;
        _tokenExpiryTime = DateTime.UtcNow.AddSeconds(tokenData.ExpiresIn - 30);

        return _currentAccessToken;
    }
}