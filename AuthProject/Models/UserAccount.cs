using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace WebApplication3.Models;

public class UserAccount
{
    [Key]
    public Guid UserId { get; set; }
    
    public string Email { get; set; }
    public string Password { get; set; }
    public string UserName { get; set; }
    
    public string? UserTypeRole { get; set; }

    public UserAccount()
    {
        // Код для ініціалізації властивостей за замовчуванням
    }
    
    [JsonConstructor]
    public UserAccount(string email, string password, string userName)
    {
        Email = email;
        Password = password;
        UserName = userName;
    }
}
