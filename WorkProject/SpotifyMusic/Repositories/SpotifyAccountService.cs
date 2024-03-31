using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SpotifyWebApi.Interface;
using SpotifyWebApi.Models;
using SpotifyApi.NetCore;

namespace SpotifyWebApi.Repositories;

public class SpotifyAccountService : ISpotifyAccountService
{
    
    private readonly HttpClient _httpClient;
    
   

    public SpotifyAccountService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetToken(string? clientId, string? clientSecret)
    {
        
        // url: 'https://accounts.spotify.com/api/token'
        var request = new HttpRequestMessage(HttpMethod.Post, "token");

        //Цей блок коду додає заголовок авторизації типу "Basic" до запиту. clientId та clientSecret об'єднуються з двокрапкою і кодуються в Base64.
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")));

        //кодуються як x-www-form-urlencoded
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"grant_type", "client_credentials"}
        });
    
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseStream = await response.Content.ReadAsStreamAsync();
        var authResult = await JsonSerializer.DeserializeAsync<AuthResult>(responseStream);

        return authResult?.access_token!;
    }
}