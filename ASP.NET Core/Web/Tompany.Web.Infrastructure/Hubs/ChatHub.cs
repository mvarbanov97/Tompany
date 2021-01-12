using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tompany.Web.Infrastructure.Contracts;

namespace Tompany.Web.Infrastructure.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService chatService;
        private readonly IHubContext<NotificationHub> notificationHubContext;
        private readonly INotificationService notificationService;

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
            await this.chatService.SendMessageToUser(fromUsername, toUsername, message, group);
        }

        public async Task ReceiveMessage(string fromUsername, string message, string group)
        {
            await this.chatService.ReceiveNewMessage(fromUsername, message, group);
        }

        public async Task JoinGroup(string user, string group)
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, group);
            await this.chatService.UserJoinedGroupMessage(user, group);
            // Send notification to all the users in the chat
        }

        public async Task SendMessageToGroup(string fromUsername, string message, string group)
        {
            var senderContextId = this.Context.ConnectionId;
            await this.chatService.SendMessageToGroup(senderContextId, fromUsername, message, group);
        }

        public async Task GroupReceiveMessage(string fromUsername, string message, string group)
        {
            var senderContextId = this.Context.ConnectionId;
            await this.chatService.GroupReceiveNewMessage(senderContextId, fromUsername, message, group);

        }
    }
}
