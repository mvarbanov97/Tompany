using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Data.Models.Enums;
using Tompany.Services.Mapping;
using Tompany.Web.Infrastructure.Contracts;
using Tompany.Web.ViewModels.Notifications.ViewModels;

namespace Tompany.Web.Infrastructure
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<ApplicationUser> usersRepository;
        private readonly IRepository<UserNotification> userNotificaitonsRepository;

        public NotificationService(
            IRepository<ApplicationUser> usersRepository,
            IRepository<UserNotification> userNotificaitonsRepository)
        {
            this.usersRepository = usersRepository;
            this.userNotificaitonsRepository = userNotificaitonsRepository;
        }

        public async Task<string> AddTripRequestNotification(string fromUsername, string toUsername, string message, string tripId)
        {
            var toUser = this.usersRepository.All().FirstOrDefault(x => x.UserName == toUsername);
            var toId = toUser.Id;
            var toImage = toUser.ImageUrl;

            var fromUser = this.usersRepository.All().FirstOrDefault(x => x.UserName == fromUsername);
            var fromId = fromUser.Id;
            var fromImage = fromUser.ImageUrl;

            var notification = new UserNotification
            {
                ApplicationUserId = fromUser.Id,
                CreatedOn = DateTime.UtcNow,
                Status = NotificationStatus.Unread,
                Text = message,
                TargetUsername = toUser.UserName,
                Link = $"/Trips/Details/{tripId}",
                NotificationType = NotificationType.TripRequest,
            };

            await this.userNotificaitonsRepository.AddAsync(notification);
            await this.userNotificaitonsRepository.SaveChangesAsync();
            return notification.Id;
        }

        public async Task<string> AddAcceptedTripRequestNotification(string fromUsername, string toUsername, string message, string tripId)
        {
            var toUser = this.usersRepository.All().FirstOrDefault(x => x.UserName == toUsername);
            var toId = toUser.Id;
            var toImage = toUser.ImageUrl;

            var fromUser = this.usersRepository.All().FirstOrDefault(x => x.UserName == fromUsername);
            var fromId = fromUser.Id;
            var fromImage = fromUser.ImageUrl;

            var notification = new UserNotification
            {
                ApplicationUserId = fromUser.Id,
                CreatedOn = DateTime.UtcNow,
                Status = NotificationStatus.Unread,
                Text = message,
                TargetUsername = toUser.UserName,
                Link = $"/Trips/Details/{tripId}",
                NotificationType = NotificationType.AcceptedTripRequest,
            };

            await this.userNotificaitonsRepository.AddAsync(notification);
            await this.userNotificaitonsRepository.SaveChangesAsync();
            return notification.Id;
        }

        public async Task<string> AddDeclinedTripRequestNotification(string fromUsername, string toUsername, string message, string tripId)
        {
            var toUser = this.usersRepository.All().FirstOrDefault(x => x.UserName == toUsername);
            var toId = toUser.Id;
            var toImage = toUser.ImageUrl;

            var fromUser = this.usersRepository.All().FirstOrDefault(x => x.UserName == fromUsername);
            var fromId = fromUser.Id;
            var fromImage = fromUser.ImageUrl;

            var notification = new UserNotification
            {
                ApplicationUserId = fromUser.Id,
                CreatedOn = DateTime.UtcNow,
                Status = NotificationStatus.Unread,
                Text = message,
                TargetUsername = toUser.UserName,
                Link = $"/Trips/Details/{tripId}",
                NotificationType = NotificationType.DeclinedTripRequest,
            };

            await this.userNotificaitonsRepository.AddAsync(notification);
            await this.userNotificaitonsRepository.SaveChangesAsync();
            return notification.Id;
        }


        public async Task<string> UserJoinedGroupChatNotification(string fromUsername, string toUsername, string message, string group)
        {
            var toUser = this.usersRepository.All().FirstOrDefault(x => x.UserName == toUsername);
            var toId = toUser.Id;
            var toImage = toUser.ImageUrl;

            var fromUser = this.usersRepository.All().FirstOrDefault(x => x.UserName == fromUsername);
            var fromId = fromUser.Id;
            var fromImage = fromUser.ImageUrl;

            var notification = new UserNotification
            {
                ApplicationUserId = fromUser.Id,
                CreatedOn = DateTime.UtcNow,
                Status = NotificationStatus.Unread,
                Text = message,
                TargetUsername = toUser.UserName,
                Link = $"/Chat/GropChat/{group}",
                NotificationType = NotificationType.JoinedGroupChat,
            };

            await this.userNotificaitonsRepository.AddAsync(notification);
            await this.userNotificaitonsRepository.SaveChangesAsync();
            return notification.Id;
        }

        public async Task<string> AddProfileRatingNotification(ApplicationUser user, ApplicationUser currentUser, int rate)
        {
            var notification = new UserNotification
            {
                ApplicationUserId = currentUser.Id,
                CreatedOn = DateTime.UtcNow,
                Status = NotificationStatus.Unread,
                Text = $"{currentUser.UserName.ToUpper()} rate your profile with {rate} stars",
                TargetUsername = user.UserName,
                Link = $"/Profile/{user.UserName}",
                NotificationType = NotificationType.RateProfile,
            };

            await this.userNotificaitonsRepository.AddAsync(notification);
            await this.userNotificaitonsRepository.SaveChangesAsync();
            return notification.Id;
        }

        public async Task<UserNotification> GetNotificationByIdAsync(string id)
        {
            var notification = await this.userNotificaitonsRepository.All().FirstOrDefaultAsync(x => x.Id == id);

            var user = await this.usersRepository.All().FirstOrDefaultAsync(x => x.Id == notification.ApplicationUserId);
            var targetUser = await this.usersRepository.All()
                .FirstOrDefaultAsync(x => x.UserName == notification.TargetUsername);

            return notification;
        }

        public async Task<List<NotificationViewModel>> GetUserNotifications(ApplicationUser currentUser)
        {
            //IQueryable<UserNotification> query = this.userNotificaitonsRepository
            //    .All()
            //    .Where(x => x.TargetUsername == currentUser.UserName)
            //    .OrderByDescending(x => x.CreatedOn);

            ////return query.To<T>().ToList();

            var result = new List<NotificationViewModel>();
            var notifications = this.userNotificaitonsRepository
                .All()
                .Where(x => x.TargetUsername == currentUser.UserName)
                .OrderByDescending(x => x.CreatedOn)
                .ToList();

            foreach (var notification in notifications)
            {
                var user = await this.usersRepository.All()
                           .FirstOrDefaultAsync(x => x.Id == notification.ApplicationUserId);
                var targetUser = await this.usersRepository.All()
                        .FirstOrDefaultAsync(x => x.UserName == notification.TargetUsername);

                NotificationViewModel item = this.ParseNotificationViewModel(notification, user, targetUser);
                result.Add(item);
            }

            var notificationsCount = await this.GetUserNotificationsCount(currentUser.UserName);

            return result;
        }

        public async Task<int> GetUserNotificationsCount(string userName)
        {
            var count = await this.userNotificaitonsRepository.All()
                    .CountAsync(x => x.TargetUsername == userName && x.Status == NotificationStatus.Unread);
            return count;
        }

        public async Task<bool> DeleteNotificationAsync(string username, string notificationId)
        {
            var notificaitonToDelete = await this.userNotificaitonsRepository
                .All()
                .FirstOrDefaultAsync(x =>
                x.Id == notificationId &&
                x.TargetUsername == username);

            if (notificaitonToDelete != null)
            {
                this.userNotificaitonsRepository.Delete(notificaitonToDelete);
                await this.userNotificaitonsRepository.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> EditStatusAsync(string notificationId, ApplicationUser currentUser, string newStatus)
        {
            var notification = await this.userNotificaitonsRepository
                .All()
                .Where(x =>
                x.Id == notificationId &&
                x.TargetUsername == currentUser.UserName)
                .FirstOrDefaultAsync();

            if (notification != null)
            {
                notification.Status = (NotificationStatus)Enum.Parse(typeof(NotificationStatus), newStatus);
                this.userNotificaitonsRepository.Update(notification);
                await this.userNotificaitonsRepository.SaveChangesAsync();
                return true;
            }

            return false;
        }

        private string GetNotificationHeading(NotificationType notificationType, ApplicationUser user, string link)
        {
            string message = string.Empty;

            switch (notificationType)
            {
                case NotificationType.AcceptedTripRequest:
                    message =
                        $"<a href=\"/Users/{user.UserName}\" style=\"text-decoration: underline\">{user.UserName}</a> aceppted your trip request for his <a href=\"{link}\" style=\"text-decoration: underline\">trip</a>.";
                    break;
                case NotificationType.DeclinedTripRequest:
                    message =
                        $"<a href=\"/Users/{user.UserName}\" style=\"text-decoration: underline\">{user.UserName}</a> declined your trip request for his <a href=\"{link}\" style=\"text-decoration: underline\">trip</a>.";
                    break;
                case NotificationType.RateProfile:
                    message =
                        $"<a href=\"/Users/{user.UserName}\" style=\"text-decoration: underline\">{user.UserName}</a> rate your <a href=\"{link}\" style=\"text-decoration: underline\">profile</a>.";
                    break;
                case NotificationType.TripRequest:
                    message =
                        $"<a href=\"/Users/{user.UserName}\" style=\"text-decoration: underline\">{user.UserName}</a> sent you a trip request for your <a href=\"{link}\" style=\"text-decoration: underline\">trip</a>.";
                    break;
                default:
                    break;
            }

            return message;
        }

        private NotificationViewModel ParseNotificationViewModel(UserNotification notification, ApplicationUser user, ApplicationUser targetUser)
        {
            var contentWithoutTags =
                Regex.Replace(notification.Text, "<.*?>", string.Empty);

            return new NotificationViewModel
            {
                Id = notification.Id,
                CreatedOn = notification.CreatedOn.ToLocalTime().ToString("dd-MMMM-yyyy HH:mm tt"),
                TargetFirstName = targetUser.FirstName,
                TargetLastName = targetUser.LastName,
                ImageUrl = user.ImageUrl,
                Heading = this.GetNotificationHeading(notification.NotificationType, user, notification.Link),
                Status = notification.Status,
                Text = contentWithoutTags.Length < 487 ?
                                contentWithoutTags :
                                $"{contentWithoutTags.Substring(0, 487)}...",
                TargetUsername = targetUser.UserName,
                AllStatuses = Enum.GetValues(typeof(NotificationStatus)).Cast<NotificationStatus>().Select(x => x.ToString()).ToList(),
            };
        }
    }
}
