namespace AuthenticationAPI.Models.User
{
    public class User
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public string Email { get; set; } = String.Empty;
        public long Phone { get; set; } = 0;
        public long MSV { get; set; } = 0;
        public bool Sex { get; set; } = true;
    }
}
