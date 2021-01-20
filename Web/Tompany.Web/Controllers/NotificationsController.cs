using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Tompany.Data.Models;
using Tompany.Web.Infrastructure.Contracts;
using Tompany.Web.Infrastructure.Hubs;
using Tompany.Web.ViewModels.Notifications.ViewModels;

namespace Tompany.Web.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly INotificationService notificationService;
        private readonly IHubContext<NotificationHub> hubContext;

        public NotificationsController(
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService,
            IHubContext<NotificationHub> hubContext)
        {
            this.userManager = userManager;
            this.notificationService = notificationService;
            this.hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);
            var result = await this.notificationService.GetUserNotifications(currentUser);

            return this.View(result);
        }

        [HttpPost]
        public async Task<bool> EditStatus(string newStatus, string notificationId)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);
            bool isNotificationEdited = await this.notificationService.EditStatusAsync(notificationId, currentUser, newStatus);
            await this.ChangeNotificationCounter(isNotificationEdited, currentUser);

            return isNotificationEdited;
        }

        [HttpPost]
        public async Task<bool> DeleteNotification(string notificationId)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);
            bool isNotificationDeleted = await this.notificationService.DeleteNotificationAsync(currentUser.UserName, notificationId);
            await this.ChangeNotificationCounter(isNotificationDeleted, currentUser);

            return isNotificationDeleted;
        }

        private async Task ChangeNotificationCounter(bool isForChange, ApplicationUser user)
        {
            if (isForChange)
            {
                int count = await this.notificationService.GetUserNotificationsCount(user.UserName);
                await this.hubContext.Clients.User(user.Id).SendAsync("ReceiveNotification", count, false);
            }
        }
    }
}
