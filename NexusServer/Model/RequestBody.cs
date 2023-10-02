namespace NexusServer.Model
{
    public class SignUpRequest
    {
        public string name { get; set; }
        public string email { get; set; }
        public string pwd { get; set; }
    }

    public class SignInRequest
    {
        public string email { get; set; }
        public string pwd { get; set; }
    }

    public class CreateConversationRequest
    {
       public string title { get; set; }
    }

    public class ConversationUpdateRequest 
    { 
        public string title { get; set; }
    }

}
