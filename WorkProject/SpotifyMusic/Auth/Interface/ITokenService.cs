namespace WorkProject.Auth.Interface;

public interface ITokenService
{
    Task<string> GetAccessTokenAsync(int userId);
}