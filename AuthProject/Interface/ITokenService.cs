using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApplication3.Models;
using WebAuthCommon;

namespace WebApplication1.Interface;

public interface ITokenService
{
    public SymmetricSecurityKey GetSymmetricSecurityKey();
    public string GenerateJWT(UserAccount user);
}