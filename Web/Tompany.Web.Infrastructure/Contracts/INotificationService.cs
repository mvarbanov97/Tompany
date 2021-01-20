using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Web.ViewModels.Notifications.ViewModels;

namespace Tompany.Web.Infrastructure.Contracts
{
    public interface INotificationService
    {
        Task<string> AddTripRequestNotification(string fromUsername, string toUsername, string message, string tripId);

        Task<string> AddAcceptedTripRequestNotification(string fromUsername, string toUsername, string message, string tripId);

        Task<string> AddDeclinedTripRequestNotification(string fromUsername, string toUsername, string message, string tripId);

        Task<string> UserJoinedGroupChatNotification(string fromUsername, string toUsername, string message, string group);

        Task<string> AddProfileRatingNotification(ApplicationUser user, ApplicationUser currentUser, int rate);

        Task<UserNotification> GetNotificationByIdAsync(string id);

        Task<List<NotificationViewModel>> GetUserNotifications(ApplicationUser currentUser);

        Task<bool> DeleteNotificationAsync(string username, string notificationId);

        Task<bool> EditStatusAsync(string notificationId, ApplicationUser currentUser, string newStatus);

        Task<int> GetUserNotificationsCount(string userName);
        //IEnumerable<T> GetUserNotifications<T>(ApplicationUser currentUser);
    }
}
