

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WorkProject.Models.Entities;

namespace WorkProject.Auth.Service;

public class JwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    

    public string GenerateJwtToken(UserEnity user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("userId", user.UserId.ToString()),       // внутрішній ID
            new Claim("spotifyId", user.SpotifyUserId),        // Spotify ID
            new Claim("email", user.Email ?? ""),              // пошта
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // унікальний ID токена
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),  // JWT живе 1 годину
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
