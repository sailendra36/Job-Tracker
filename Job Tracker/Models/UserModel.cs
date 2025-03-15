namespace Job_Tracker.Models
{
    public class UserModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
