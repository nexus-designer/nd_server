﻿using NexusServer.Data;
namespace NexusServer.Interfaces
{
    public interface IUserRepository
    {
        public User GetUserByEmail(string email);
        public User GetUserBySessionToken(string sessionToken);
        public void UpdateNullSessionToken(string sessionToken);
        public void CreateConversation(Conversation conversation);
        public List<Conversation> GetAllConversations(long id);
        void CreateUser(User user);
        // Add other user-related methods here.
        public Task<User> AuthenticateUserAsync(string email, string password);
        public void SaveSessionToken(string email, string sessionToken);
        public Conversation GetConversationById(long id,long userId);
        public Conversation UpdateConversation(string title, long id,long userId);
        public Conversation DeleteConversation(long id,long userId);
        public List<Msg> GetMessageByCoversationId(long id);
        public Msg ReadingMessageByIndex(int index, long conversationId);
        public Msg DeleteMessage(long id, int index);
        public Msg AppendMessage(long id, string content);
        public Msg UpdateMessage(long id, int index, string content);
    }

}
