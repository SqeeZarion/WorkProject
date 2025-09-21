using System.Net.Http.Headers;
using WorkProject.Auth.Interface;

namespace WorkProject.Auth.Handler;

//Використання Access Token через інтеграцію в AuthorizationHandler
public class AuthorizationHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationHandler(ITokenService tokenService, IHttpContextAccessor httpContextAccessor)
    {
        _tokenService = tokenService;
        _httpContextAccessor = httpContextAccessor;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User ID not found in claims.");

        var userId = int.Parse(userIdClaim);
        
        var accessToken = await _tokenService.GetAccessTokenAsync(userId);
      
        //об'єкт заголовка авторизації з вказаним типом і значенням.
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return await base.SendAsync(request, cancellationToken);
    }
}