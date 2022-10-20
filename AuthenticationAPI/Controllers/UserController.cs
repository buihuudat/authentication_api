using AuthenticationAPI.Data;
using AuthenticationAPI.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AuthenticationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly AuthenticationApiDbContext authContext;

        public UserController(AuthenticationApiDbContext authContext)
        {
            this.authContext = authContext;
        }


        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await authContext.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid id)
        {
            var user = await authContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateUser(CreateUser createUser)
        {
            var checkExitsUsername = authContext.Users.Where(u => u.Username == createUser.Username).FirstOrDefault();

            if (checkExitsUsername != null)
            {
                return BadRequest("Username has been already");
            }

            CreatePasswordHash(createUser.Password, out byte[] hash, out byte[] salt);
            var user = new User()
            {
                FullName = createUser.FullName,
                Username = createUser.Username,
                Password = hash,
                Salt = salt,
                Email = createUser.Email,
                MSV = createUser.MSV,
                Phone = createUser.Phone,
                Sex = createUser.Sex
            };

            await authContext.Users.AddAsync(user);
            await authContext.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPut]
        [Route("update/{id:guid}")]
        public async Task<IActionResult> UpdateUser([FromRoute] Guid id, UpdateUser updateUser)
        {
            CreatePasswordHash(updateUser.Password, out byte[] hash, out byte[] salt);
            var user = await authContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.FullName = updateUser.FullName;
            user.Password = hash;
            user.Salt = salt;
            user.Email = updateUser.Email;
            user.Phone = updateUser.Phone;
            user.MSV = updateUser.MSV;
            user.Sex = updateUser.Sex;

            await authContext.SaveChangesAsync();
            return Ok(user);
        }

        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        [HttpDelete]
        [Route("delete/{id:guid}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            var user = await authContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            authContext.Remove(user);
            await authContext.SaveChangesAsync();
            return Ok(user);
        }
    }
}
