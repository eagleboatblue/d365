namespace Nov.Caps.Int.D365.Messaging
{
    public class Settings
    {
        public string Url { get; }

        public string User { get; }

        public string Password { get; }

        public Settings(string url, string user, string password)
        {
            this.Url = url;
            this.User = user;
            this.Password = password;
        }
    }
}
