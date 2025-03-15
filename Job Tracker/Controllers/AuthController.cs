using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Job_Tracker.Models;
using Job_Tracker.Data;
using System.Security.Cryptography;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ApplicationDbContext _context;

    public AuthController(IConfiguration config, ApplicationDbContext context)
    {
        _config = config;
        _context = context;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] UserRegister userRegister)
    {
        var user = new UserModel
        {
            Username = userRegister.Username,
            Email = userRegister.Email,
            PasswordHash = HashPassword(userRegister.Password)
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok("User registered successfully");
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
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
        var user = _context.Users.SingleOrDefault(u => u.Username == userLogin.Username);

        if (user != null && VerifyPassword(userLogin.Password, user.PasswordHash))
        {
            return user;
        }

        return null;
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            return hashedPassword == storedHash;
        }
    }

    private string GenerateToken(UserModel user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Username),
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

public class UserRegister
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}
public class UserLogin
{
    public string Username { get; set; }
    public string Password { get; set; }
}