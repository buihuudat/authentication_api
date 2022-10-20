using AuthenticationAPI.Models.User;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationAPI.Data
{
    public class AuthenticationApiDbContext : DbContext
    {
        public AuthenticationApiDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
