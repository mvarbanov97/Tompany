using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tompany.Services.Chatting
{
    public interface IChatService
    {
        Task AddUserToGroup(string groupName, string toUsername, string fromName);

        Task<string> SendMessageToUser(string fromUsername, string toUsername, string message, string group);

        Task ReceiveNewMessage(string fromUsername, string message, string group);
    }
}
