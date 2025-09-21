namespace WorkProject.Models.Dtos;

public class AuthResultDto
{
    public int UserId { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }

    // Повертаємо access_token для клієнта, але NIКОЛИ refresh_token
    public string AccessToken { get; set; }
    public int ExpiresIn { get; set; }
}