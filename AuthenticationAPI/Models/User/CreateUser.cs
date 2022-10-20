namespace AuthenticationAPI.Models.User
{
    public class CreateUser
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; } = string.Empty;
        public long Phone { get; set; } = 0;
        public long MSV { get; set; } = 0;
        public bool Sex { get; set; } = true;
    }
}
