    namespace WorkProject.Auth.Controllers;

    using Microsoft.AspNetCore.Mvc;
    using WorkProject.Auth.Service;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly JwtService _jwtService;

        public AuthController(AuthService authService, UserService userService, JwtService jwtService)
        {
            _authService = authService;
            _userService = userService;
            _jwtService = jwtService;
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
            
            var accessToken = await _authService.GetAccessTokenUsingCodeAsync(code);
            
            // Отримуємо профіль користувача через сервіс
            var userProfile = await _authService.GetSpotifyUserProfileAsync(accessToken);
            
            if (userProfile == null)
                return BadRequest("Failed to fetch Spotify user profile.");
            
            // Зберігаємо користувача в БД
            var user = await _userService.UpsertFromSpotifyAsync(userProfile, refreshToken: null, expiresIn: 3600);

            // Генеруємо JWT
            var jwt = _jwtService.GenerateJwtToken(user);

            return Ok(new { Jwt = jwt });
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