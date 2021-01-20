using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Data.Models.Enums;

namespace Tompany.Web.Infrastructure.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly IRepository<UserNotification> userNotificationsRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public NotificationHub(
            IRepository<UserNotification> userNotificationsRepository,
            UserManager<ApplicationUser> userManager)
        {
            this.userNotificationsRepository = userNotificationsRepository;
            this.userManager = userManager;
        }

        public async Task GetUserNotificationCount()
        {
            var username = this.Context.User.Identity.Name;
            var targetUser = await this.userManager.GetUserAsync(this.Context.User);

            var notificationCount = await this.userNotificationsRepository.All().CountAsync(x => x.TargetUsername == username && x.Status == NotificationStatus.Unread);

            await this.Clients.User(targetUser.Id).SendAsync("ReceiveNotification", notificationCount);
        }
    }
}
