using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Job_Tracker.Models;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLogin userLogin)
    {
        var user = Authenticate(userLogin);

        if (user != null)
        {
            var token = GenerateToken(user);
            return Ok(new { token });
        }

        return NotFound("User not found");
    }

    private UserModel Authenticate(UserLogin userLogin)
    {
        // Validate the user credentials
        // In a real application, you would check the credentials against a database
        if (userLogin.Username == "test" && userLogin.Password == "password")
        {
            return new UserModel { Name = "Test User", Email = "test@example.com" };
        }

        return null;
    }

    private string GenerateToken(UserModel user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Name),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
          _config["Jwt:Audience"],
          claims,
          expires: DateTime.Now.AddMinutes(30),
          signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
