using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Services.Chatting;

namespace Tompany.Services.Chatting
{
    public class ChatService : IChatService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> usersRepository;
        private readonly IDeletableEntityRepository<Group> groupsRepository;
        private readonly IDeletableEntityRepository<ChatMessage> chatMessageRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHubContext<ChatHub> hubContext;

        public ChatService(
            IDeletableEntityRepository<ApplicationUser> usersRepository,
            IDeletableEntityRepository<Group> groupsRepository,
            IDeletableEntityRepository<ChatMessage> chatMessageRepository,
            UserManager<ApplicationUser> userManager,
            IHubContext<ChatHub> hubContext)
        {
            this.usersRepository = usersRepository;
            this.userManager = userManager;
            this.hubContext = hubContext;
            this.groupsRepository = groupsRepository;
            this.chatMessageRepository = chatMessageRepository;
        }

        public async Task AddUserToGroup(string groupName, string toUsername, string fromUsername)
        {
            var toUser = this.usersRepository
                .All()
                .FirstOrDefault(x => x.UserName == toUsername);
            var toUserId = toUser.Id;

            var fromUser = this.usersRepository
                .All()
                .FirstOrDefault(x => x.UserName == fromUsername);
            var fromUserId = fromUser.Id;

            var targetGroup = this.groupsRepository
                .All()
                .FirstOrDefault(x => x.Name.ToLower() == groupName.ToLower());

            if (targetGroup == null)
            {
                targetGroup = new Group
                {
                    Name = groupName,
                };

                var targetToUser = new UserGroup
                {
                    ApplicationUser = toUser,
                    Group = targetGroup,
                };

                var targetFromUser = new UserGroup
                {
                    ApplicationUser = fromUser,
                    Group = targetGroup,
                };

                targetGroup.UsersGroups.Add(targetToUser);
                targetGroup.UsersGroups.Add(targetFromUser);

                await this.groupsRepository.AddAsync(targetGroup);
                await this.groupsRepository.SaveChangesAsync();
            }

            await this.hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", fromUsername, $"{fromUsername} has joined the group {groupName}.");
        }

        public async Task ReceiveNewMessage(string fromUsername, string message, string group)
        {
            var fromUser = this.usersRepository.
                All()
                .FirstOrDefault(x => x.UserName == fromUsername);

            var fromId = fromUser.Id;
            var fromImage = fromUser.ImageUrl;

            await this.chatMessageRepository.AddAsync(new ChatMessage
            {
                ApplicationUser = fromUser,
                Group = this.groupsRepository.All().FirstOrDefault(x => x.Name.ToLower() == group.ToLower()),
                SendedOn = DateTime.UtcNow,
                ReceiverUsername = fromUser.UserName,
                RecieverImageUrl = fromUser.ImageUrl,
                Content = message,
            });

            await this.chatMessageRepository.SaveChangesAsync();
            await this.hubContext.Clients.User(fromId).SendAsync("SendMessage", fromUsername, fromImage, message);
        }

        public async Task<string> SendMessageToUser(string fromUsername, string toUsername, string message, string group)
        {
            var toUser = this.usersRepository.All().FirstOrDefault(x => x.UserName == toUsername);
            var toId = toUser.Id;
            var toImage = toUser.ImageUrl;

            var fromUser = this.usersRepository.All().FirstOrDefault(x => x.UserName == fromUsername);
            var fromId = fromUser.Id;
            var fromImage = fromUser.ImageUrl;

            var newMessage = new ChatMessage
            {
                ApplicationUser = fromUser,
                Group = this.groupsRepository.All().FirstOrDefault(x => x.Name.ToLower() == group.ToLower()),
                SendedOn = DateTime.UtcNow,
                ReceiverUsername = toUser.UserName,
                RecieverImageUrl = toUser.ImageUrl,
                Content = message,
            };

            await this.chatMessageRepository.AddAsync(newMessage);
            await this.chatMessageRepository.SaveChangesAsync();
            await this.hubContext.Clients.User(toId).SendAsync("ReceiveMessage", fromUsername, fromImage, message);

            return toId;
        }

    }
}
