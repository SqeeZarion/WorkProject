using System.Text.Json;
using WorkProject.Auth.Interface;
using WorkProject.Auth.Models;

namespace WorkProject.Auth.Service;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;

    public AuthService(HttpClient httpClient, IConfiguration configuration, ITokenService tokenService)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _tokenService = tokenService;
    }
    
    public string GetAuthorizationUrl()
    {
        //індифікатор додатку
        var clientId = _configuration["Spotify:ClientID"];
        
        //посилання на Spotify перенаправить користувача після авторизації.
        //З унікальним авторизаційним кодом, якщо авторизація пройшла успішно
        var redirectUri = _configuration["Spotify:RedirectUri"]; 
        
        //Строка дозволів, які додаток запитує у користувача.
        //Ці дозволи визначають рівень доступу до інформації користувача Spotify
        var scopes = _configuration["Spotify:Scopes"]; 
        
        //Цей параметр керує тим, чи повинне Spotify показувати діалог підтвердження дозволів кожного разу, коли користувач намагається увійти
        var showDialog = _configuration["Spotify:ShowDialog"];

        var authorizationUrl = 
            $"{_configuration["Spotify:AuthUrl"]}?client_id={clientId}&response_type=code&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope={Uri.EscapeDataString(scopes)}&show_dialog={showDialog}";
        return authorizationUrl;
    }

    //3. Обмін коду на Access і Refresh Tokens

    public async Task<string> GetAccessTokenUsingCodeAsync(string code)
    {
        var clientId = _configuration["Spotify:ClientId"];
        var clientSecret = _configuration["Spotify:ClientSecret"];
        var redirectUri = _configuration["Spotify:RedirectUri"];

        var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", redirectUri },
                { "client_id", clientId },          
                { "client_secret", clientSecret }
            })
        };
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var tokenData = JsonSerializer.Deserialize<TokenResponse>(responseContent);

        await _tokenService.SaveAccessTokenAsync(tokenData.AccessToken, tokenData.ExpiresIn);
        return tokenData.AccessToken;
        
    }
}