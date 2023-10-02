namespace NexusServer.Model
{
    public class UserModel
    {
        public long userId { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string passwordHash { get; set; }
        public string sessionToken { get; set; }
    }
}
