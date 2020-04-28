using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Web.ViewModels.Trips;

namespace Tompany.Services.Data.Contracts
{
    public interface IUsersService
    {
        ApplicationUser GetUserById(string id);

        Task AcceptTripRequest(string senderId, string tripId, string userId);

        Task DeclineTripRequest(string senderId, string tripId, string userId);

        Task AddPassengerToTrip(string tripId, string passengerId);

        Task GetUserCars(string userId);
    }
}
