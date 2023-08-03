using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApplication1.Database;
using WebApplication1.Interface;
using WebApplication3.Models;
using WebAuthCommon;


namespace WebApplication3.Controllers;

//відповідає за генерацію токенів при успішній аутифікації

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
   
    private readonly DBConnection connection;
    private ITokenService authOption;
    private IPasswordService passwordService;
    
    public AuthController(ITokenService authOption, DBConnection connection, IPasswordService passwordService)
    {
        this.connection = connection;
        this.authOption = authOption;
        this.passwordService = passwordService;

    }

    [Route("login")] // вказує на шлях URL-адреси, за яким клієнт може зробити HTTP-запит до сервера.
    [HttpPost]
    //[FromBody] - це атрибут дотнета, який вказує на те, що дані для
    //параметра методу контролера повинні братися з тіла запиту HTTP
    public async Task<IActionResult> Login([FromBody] Login? request)
    {
        // UserAccount user = UserRepository.AuthenticateUser(request.UserName, request.Password);

        if (request != null)
        {
            var user = await connection.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName
                                                                 && u.Password == request.Password);
            if (user == null)
                return NotFound(new { Message = "User not Found" });
            //Generate token
            var token = authOption.GenerateJWT(user);

            return Ok(new
            {
                acess_token = token
            });
        }

        return Unauthorized(); //помилка 401
    }

    //токен для авторизованого користувача

    [Route("register")] // вказує на шлях URL-адреси, за яким клієнт може зробити HTTP-запит до сервера.
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] UserAccount user)
    {
        if (user != null)
        {
            //Generate token
            // UserRepository.RegisterUser(user);
            user.UserId = Guid.NewGuid();
            user.Password = passwordService.Encode(user.Password);
            user.UserTypeRole = "User";
            // user.Token
            
            await connection.Users.AddAsync(user);
            await connection.SaveChangesAsync();
            return Ok(new
            {
                Message = "Registration sucessful"
            });
        }

        return BadRequest(); //помилка 401
    }
}