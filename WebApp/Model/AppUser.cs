namespace WebApp.Model
{
    public class AppUser
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public List<string> Claims { get; set; } = new();
    }
}