using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Models;

namespace Tompany.Web.Infrastructure
{
    public interface IChatService
    {
        Task AddUserToGroup(string groupName, string toUsername, string fromName);

        Task<string> SendMessageToUser(string fromUsername, string toUsername, string message, string group);

        Task SendMessageToGroup(string senderContextId, string fromUsername, string message, string group);

        Task ReceiveNewMessage(string fromUsername, string message, string group);

        Task UserJoinedGroupMessage(string username, string group);

        Task GroupReceiveNewMessage(string senderContetId, string fromUsername, string message, string group);

        Task<ICollection<ChatMessage>> ExtractAllMessages(string group);
    }
}
