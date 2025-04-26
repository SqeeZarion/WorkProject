namespace WorkProject.Auth.Interface;

public interface ITokenService
{
    Task<string> GetAccessTokenAsync();

    Task SaveAccessTokenAsync(string accessToken, int expiresIn);
}