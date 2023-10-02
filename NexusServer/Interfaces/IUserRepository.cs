using NexusServer.Data;
namespace NexusServer.Interfaces
{
    public interface IUserRepository
    {
        public User GetUserByEmail(string email);
        public User GetUserBySessionToken(string sessionToken);
        public void UpdateNullSessionToken(string sessionToken);
        public void CreateConversation(Conversation conversation);
        public List<Conversation> GetAllConversations(string sessionToken);
        void CreateUser(User user);
        // Add other user-related methods here.
        public Task<User> AuthenticateUserAsync(string email, string password);
        public void SaveSessionToken(string email, string sessionToken);
        public Conversation GetConversationById(long id);
        public Conversation UpdateConversation(string title, long id);
        public Conversation DeleteConversation(long id);
        public List<Msg> GetMessageByCoversationId(long id);
        public Msg ReadingMessageByIndex(int index, long conversationId);
        public Msg DeleteMessage(long id, int index);
    }

}
