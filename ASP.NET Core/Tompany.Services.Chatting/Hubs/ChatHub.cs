using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tompany.Services.Chatting
{
    public class ChatHub : Hub
    {
        private readonly IChatService chatService;

        public ChatHub(IChatService chatService)
        {
            this.chatService = chatService;
        }

        public async Task AddToGroup(string groupName, string toUsername, string fromUsername)
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, groupName);
            await this.chatService.AddUserToGroup(groupName, toUsername, fromUsername);
        }

        public async Task SendMessage(string fromUsername, string toUsername, string message, string group)
        {
            string toId =
                await this.chatService.SendMessageToUser(fromUsername, toUsername, message, group);
        }

        public async Task ReceiveMessage(string fromUsername, string message, string group)
        {
            await this.chatService.ReceiveNewMessage(fromUsername, message, group);
        }
    }
}
