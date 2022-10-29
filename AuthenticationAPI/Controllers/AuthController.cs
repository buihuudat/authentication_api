using AuthenticationAPI.Data;
using AuthenticationAPI.Models.Auth;
using AuthenticationAPI.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AuthenticationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly AuthenticationApiDbContext authContext;
        private IConfiguration _configuration;

        public AuthController(IConfiguration configuration, AuthenticationApiDbContext authenticationApiDbContext)
        {
            _configuration = configuration;
            authContext = authenticationApiDbContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Signup signup)
        {
            var checkExitsUsername = authContext.Users.Where(u => u.Username == signup.Username).FirstOrDefault();
            if (checkExitsUsername != null)
            {
                return BadRequest("Username has been areadly");
            }

            CreatePasswordHash(signup.Password, out byte[] hash, out byte[] salt);

            var user = new User()
            {
                Id = Guid.NewGuid(),
                Username = signup.Username,
                Email = signup.Email,
                MSV = signup.MSV,
                FullName = signup.FullName,
                Password = hash,
                Salt = salt,
            };

            await authContext.Users.AddAsync(user);
            await authContext.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Signin signin)
        {
            var user = authContext.Users.Where(u => u.Username == signin.Username).FirstOrDefault();

            if (user.Username == null)
            {
                return BadRequest("Username not found");
            }

            if (user.Username != signin.Username)
            {
                return BadRequest("Invalid username");
            }

            if (!VerifyPassword(signin.Password, user.Password, user.Salt))
            {
                return BadRequest("Wrong password");
            }

            var token = CreateToken(user);
            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPassword(string password, byte[] hash, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return passwordHash.SequenceEqual(hash);
            }
        }
    }

}
