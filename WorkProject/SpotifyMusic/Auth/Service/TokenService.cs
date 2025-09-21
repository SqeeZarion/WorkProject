using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SpotifyWebApi.Database;
using WorkProject.Auth.Interface;
using WorkProject.Auth.Models;

namespace WorkProject.Auth.Service;

public class TokenService : ITokenService
{
    private readonly HttpClient _httpClient; //для здійснення HTTP-запитів до зовнішніх веб-сервісів, таких як Spotify API.
    private readonly IConfiguration _configuration; // для отримання конфіденційних даних з appsettings.json
    private string _currentAccessToken;
    private DateTime _tokenExpiryTime;
    private readonly DbConnection _db;

    public TokenService(HttpClient httpClient, IConfiguration configuration, DbConnection db)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _db = db;
    }

    public async Task<string> GetAccessTokenAsync(int userId = 0)
    {
        if (DateTime.UtcNow >= _tokenExpiryTime || string.IsNullOrEmpty(_currentAccessToken))
        {
            var clientId = _configuration["SpotifyAuth:ClientId"];
            var clientSecret = _configuration["SpotifyAuth:ClientSecret"];

            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")));

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            });

            Console.WriteLine($"Sending token request: client_id={clientId}, client_secret={clientSecret}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<TokenResponse>(responseContent);

            _currentAccessToken = tokenData.AccessToken;
            int expiresInSeconds = tokenData.ExpiresIn;
            _tokenExpiryTime = DateTime.UtcNow.AddSeconds(expiresInSeconds - 30);
        }
        return _currentAccessToken;
    }
}