using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Web.ViewModels.Trips;
using Tompany.Web.ViewModels.Users.ViewModels;

namespace Tompany.Services.Data.Contracts
{
    public interface IUsersService
    {
        Task<ApplicationUserViewModel> ExtractUserInfo(string username, ApplicationUser currentUser);

        T GetUserByUsername<T>(string username);

        ApplicationUserViewModel GetUserByUsername(string username);

        ApplicationUser GetUserById(string id);

        Task AcceptTripRequest(string senderId, ApplicationUser user, string tripId);

        Task DeclineTripRequest(string senderId, ApplicationUser user, string tripId);

        Task GetUserCars(string userId);

        Task<bool> IsUserExists(string username);

        Task<int> TakeCreatedTripPostsCountByUsername(string username);

        double ExtractUserRatingScore(string username);

        Task<int> GetLatestScore(ApplicationUser currentUser, string username);

        bool IsRequestAlreadySent(string senderId, string tripId);

        Task<double> RateUser(ApplicationUser currentUser, string username, int rate);
    }
}
