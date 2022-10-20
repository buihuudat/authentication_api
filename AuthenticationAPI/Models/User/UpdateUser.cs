namespace AuthenticationAPI.Models.User
{
    public class UpdateUser
    {
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public long Phone { get; set; }
        public long MSV { get; set; }
        public bool Sex { get; set; }
    }
}
