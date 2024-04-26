using System.Net.Http.Headers;
using WorkProject.Auth.Interface;

namespace WorkProject.Auth.Handler;

public class AuthorizationHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;

    public AuthorizationHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await _tokenService.GetAccessTokenAsync();
        
        //об'єкт заголовка авторизації з вказаним типом і значенням.
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return await base.SendAsync(request, cancellationToken);
    }
}