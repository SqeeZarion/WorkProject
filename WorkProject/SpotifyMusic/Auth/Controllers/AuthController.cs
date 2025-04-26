namespace WorkProject.Auth.Controllers;

using Microsoft.AspNetCore.Mvc;
using WorkProject.Auth.Service;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    //1 крок перенаправлення на сторінку авторизації Spotify
    [HttpGet("authorization-url")]
    public IActionResult GetAuthorizationUrl()
    {
        var authorizationUrl = _authService.GetAuthorizationUrl();
        return Ok(new { Url = authorizationUrl });
    }

    // 2 крок  це обробка редиректу з Spotify,
    // де отримаєм code, який використовуватиметься для отримання токенів
    [HttpGet("redpage")]
    public async Task<IActionResult> HandleSpotifyRedirect([FromQuery] string code)
    {
        if (string.IsNullOrEmpty(code))
            return BadRequest("Authorization code is missing.");

        Console.WriteLine(code);
        var accessToken = await _authService.GetAccessTokenUsingCodeAsync(code);
        
        // Зберігати токен в базі даних добав
        // await SaveAccessTokenToDatabaseAsync(accessToken);
        
        return Ok(new { AccessToken = accessToken });
    }

    // private async Task SaveAccessTokenToDatabaseAsync(string accessToken)
    // {
    //     // Реалізація збереження токену в базі даних
    //     // Наприклад, з використанням Entity Framework
    //     using (var context = new YourDbContext())
    //     {
    //         var tokenEntry = new AccessTokenEntity
    //         {
    //             Token = accessToken,     
    //             ExpiryTime = DateTime.UtcNow.AddHours(1) // Приведений час життя токену, змініть відповідно
    //         };
    //         context.AccessTokens.Add(tokenEntry);
    //         await context.SaveChangesAsync();
    //     }
    // }
}