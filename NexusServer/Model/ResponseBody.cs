namespace NexusServer.Model
{
    public class SignUpResponse
    {
        public string message { get; set; }
    }
    public class SignInResponse
    {
        public long id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string sessionToken { get; set; }
    }
    public class ConversationResponse
    {
        public long id { get; set; }
        public long userId { get; set; }
        public string title { get; set; }
        public bool waiting { get; set; }
        public DateTimeOffset createdOn { get; set; }
    }

}
