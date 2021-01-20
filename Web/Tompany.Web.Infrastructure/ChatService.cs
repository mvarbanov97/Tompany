using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Web.Infrastructure.Hubs;
using Microsoft.EntityFrameworkCore;

namespace Tompany.Web.Infrastructure
{
    public class ChatService : IChatService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> usersRepository;
        private readonly IDeletableEntityRepository<Group> groupsRepository;
        private readonly IDeletableEntityRepository<ChatMessage> chatMessageRepository;
        private readonly IHubContext<ChatHub> hubContext;

        public ChatService(
            IDeletableEntityRepository<ApplicationUser> usersRepository,
            IDeletableEntityRepository<Group> groupsRepository,
            IDeletableEntityRepository<ChatMessage> chatMessageRepository,
            IHubContext<ChatHub> hubContext)
        {
            this.usersRepository = usersRepository;
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
            var fromUsermage = fromUser.ImageUrl;
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

            await this.hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", fromUsername, fromUsermage, $"{fromUsername} has joined the chat.");
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

        public async Task<string> SendMessageToUser(string fromUsername, string toUsername, string message, string groupName)
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
                Group = this.groupsRepository.All().FirstOrDefault(x => x.Name.ToLower() == groupName.ToLower()),
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

        public async Task SendMessageToGroup(string senderContextId, string fromUsername, string message, string groupName)
        {
            var fromUser = await this.usersRepository.All().FirstOrDefaultAsync(x => x.UserName == fromUsername);
            var fromUserImage = fromUser.ImageUrl;
            var toGroup = await this.groupsRepository.All().FirstOrDefaultAsync(x => x.Name.ToLower() == groupName.ToLower());

            var newMessage = new ChatMessage
            {
                ApplicationUser = fromUser,
                Group = toGroup,
                SendedOn = DateTime.UtcNow,
                ReceiverUsername = toGroup.Name,
                RecieverImageUrl = fromUserImage,
                Content = message,
            };

            await this.chatMessageRepository.AddAsync(newMessage);
            await this.hubContext.Clients.GroupExcept(groupName, senderContextId).SendAsync("SendMessageToGroup", fromUsername, fromUserImage, message);
            await this.hubContext.Clients.User(fromUser.Id).SendAsync("ReceiveMessageSender", fromUsername, fromUserImage, message);

            await this.chatMessageRepository.SaveChangesAsync();
        }

        public async Task GroupReceiveNewMessage(string senderContextId, string fromUsername, string message, string group)
        {
            var fromUser = this.usersRepository.
                All()
                .FirstOrDefault(x => x.UserName == fromUsername);

            var fromId = fromUser.Id;
            var fromImage = fromUser.ImageUrl;

            var newMessage = new ChatMessage
            {
                ApplicationUser = fromUser,
                Group = this.groupsRepository.All().FirstOrDefault(x => x.Name.ToLower() == group.ToLower()),
                SendedOn = DateTime.UtcNow,
                Content = message,
            };

            await this.chatMessageRepository.AddAsync(newMessage);
            await this.hubContext.Clients.GroupExcept(group, senderContextId).SendAsync("SendMessageToGroup", fromUsername, fromImage, message);
            await this.hubContext.Clients.User(fromId).SendAsync("SenderReceiveMessage", fromUsername, fromImage, message);
            await this.chatMessageRepository.SaveChangesAsync();
        }

        public async Task UserJoinedGroupMessage(string username, string group)
        {
            var fromUser = this.usersRepository.
                All()
                .FirstOrDefault(x => x.UserName == username);
            var fromImage = fromUser.ImageUrl;
            var joinedGroupMessage = $"{username} has joined the group chat.";

            var dbGroup = await this.groupsRepository.All().FirstOrDefaultAsync(x => x.Name == group);

            if (dbGroup == null)
            {
                var targetGroup = new Group
                {
                    Name = group,
                };

                await this.groupsRepository.AddAsync(targetGroup);
                await this.groupsRepository.SaveChangesAsync();
            }

            var newMessage = new ChatMessage
            {
                ApplicationUser = fromUser,
                Group = this.groupsRepository.All().FirstOrDefault(x => x.Name.ToLower() == group.ToLower()),
                SendedOn = DateTime.UtcNow,
                Content = joinedGroupMessage,
            };

            await this.chatMessageRepository.AddAsync(newMessage);
            await this.hubContext.Clients.Group(group).SendAsync("JoinedGroupMessage", username, fromImage, joinedGroupMessage);
            await this.chatMessageRepository.SaveChangesAsync();
        }

        public async Task<ICollection<ChatMessage>> ExtractAllMessages(string group)
        {
            var targetGroup = await this.groupsRepository.All().FirstOrDefaultAsync(x => x.Name.ToLower() == group.ToLower());

            if (targetGroup != null)
            {
                var messages = this.chatMessageRepository
                    .All()
                    .Where(x => x.GroupId == targetGroup.Id)
                    .OrderBy(x => x.SendedOn)
                    .ToList();

                // Todo deleting old messages

                foreach (var message in messages)
                {
                    message.ApplicationUser = await this.usersRepository
                        .All()
                        .FirstOrDefaultAsync(x => x.Id == message.ApplicationUserId);
                }

                return messages;
            }

            return null;
        }
    }
}
