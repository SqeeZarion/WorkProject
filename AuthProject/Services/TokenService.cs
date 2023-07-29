using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Interface;
using WebApplication3.Models;
using WebAuthCommon;

namespace WebApplication1.Services;

public class TokenService : ITokenService
{
    
    private IOptions<AuthToken> authOptions;

    public TokenService(IOptions<AuthToken> authOptions)
    {
        this.authOptions = authOptions;
    }
    
    public SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        var authParams = authOptions.Value;
        
        using (var sha256 = SHA256.Create())
        {
            byte[] hashedSecret = sha256.ComputeHash(Encoding.ASCII.GetBytes(authParams.Secret));
            return new SymmetricSecurityKey(hashedSecret);
        }
    }
        
    public  string GenerateJWT(UserAccount user)
    {
        var authParams = authOptions.Value;

        var securityKey = GetSymmetricSecurityKey();

        //облікові дані
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


        //список об'єктів для генерації токена

        var claims = new List<Claim>()
        {
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
        };

        foreach (var role in user.UserTypeRole)
            claims.Add(new Claim("UserTypeRole", role.ToString()));

        //используется для передачи информации между двумя сторонами в виде JSON-объекта.
        //JWT - это безопасный стандартный формат, который используется для передачи утверждений между сторонами,
        //такими как серверы и приложения, и может быть использован для аутентификации и авторизации.

        //Конструктор JwtSecurityToken принимает несколько параметров,
        //включая Issuer (выдавший токен), Audience (адресат токена), claims (утверждения токена),
        //expires (время истечения срока действия токена) и signingCredentials
        //(информация о подписи, используемая для подписи токена)
        var token = new JwtSecurityToken(authParams.Issuer, authParams.Audience,
            claims, expires: DateTime.Now.AddSeconds(authParams.TokenLifetime),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}