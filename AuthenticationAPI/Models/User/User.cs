namespace AuthenticationAPI.Models.User
{
    public class User
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public string Email { get; set; }
        public long Phone { get; set; }
        public long MSV { get; set; }
        public bool Sex { get; set; } = true;
    }
}
