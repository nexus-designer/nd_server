using Microsoft.EntityFrameworkCore;
using NexusServer.Data;
using BCrypt.Net;
namespace NexusServer.Interfaces
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.email == email);
        }
        public User GetUserBySessionToken(string sessionToken)
        {
            var user = _context.Users.FirstOrDefault(u => u.sessionToken == sessionToken);
            return user;
        }
        public void UpdateNullSessionToken(string sessionToken)
        {
            var user = _context.Users.FirstOrDefault(u => u.sessionToken == sessionToken);
            user.sessionToken = null;
            _context.SaveChanges();
        }
        public void CreateUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        public async Task<User> AuthenticateUserAsync(string email, string password)
        {
            // Find the user by email in the "user" table
            var user = await _context.Users.Where(u => u.email != null && u.email == email).SingleOrDefaultAsync();

            if (user == null)
            {
                // User not found
                return null;
            }
            if (!BCrypt.Net.BCrypt.Verify(password, user.pwdHash))
            {
                return null;
            }
            // Authentication successful
            return user;
        }

        public void SaveSessionToken(string email, string sessionToken)
        {
            var user = _context.Users.SingleOrDefault(u => u.email == email);
            if (user != null)
            {
                // Update the session token
                user.sessionToken = sessionToken;

                // Save the changes to the database
                _context.SaveChanges();
            }
        }
        public void CreateConversation(Conversation conversation)
        {
            _context.Conversations.Add(conversation);
            _context.SaveChanges();
        }
        public List<Conversation> GetAllConversations(string sessionToken)
        {
            var user = _context.Users.FirstOrDefault(u => u.sessionToken == sessionToken);
            var conversations = _context.Conversations.Where(c => c.userId == user.id).ToList();
            return conversations;
        }
        public Conversation GetConversationById(long conversationId)
        {
            var conversation = _context.Conversations.FirstOrDefault(c => c.id == conversationId);
            return conversation;
        }
        public Conversation UpdateConversation(string title, long id)
        {
            var conversation = _context.Conversations.FirstOrDefault(c => c.id == id);
            conversation.title = title;
            _context.SaveChanges();
            return conversation;
        }
        public Conversation DeleteConversation(long id)
        {
            var conversation = _context.Conversations.FirstOrDefault(c => c.id == id);
            _context.Conversations.Remove(conversation);
            _context.SaveChanges();
            return conversation;
        }
        public List<Msg> GetMessageByCoversationId(long id)
        {
            var messages = _context.Msgs.Where(m => m.conversationId == id).ToList();
            return messages;
        }
        public Msg ReadingMessageByIndex(int index, long conversationId)
        {
            var message = _context.Msgs.FirstOrDefault(m => m.index == index && m.conversationId == conversationId);
            return message;
        }
        public Msg DeleteMessage(long id,int index)
        {
            var msg = _context.Msgs.FirstOrDefault(c => c.conversationId == id && c.index == index);
            _context.Msgs.Remove(msg);
            _context.SaveChanges();
            return msg;
        }
    }

}
