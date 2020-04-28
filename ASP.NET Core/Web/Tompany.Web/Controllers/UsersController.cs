using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Web.ViewModels.Trips;
using Tompany.Web.ViewModels.Users;

namespace Tompany.Web.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUsersService usersService;
        private readonly ITripsService tripsService;
        private readonly UserManager<ApplicationUser> userManager;

        public UsersController(
            IUsersService usersService,
            ITripsService tripsService,
            UserManager<ApplicationUser> userManager)
        {
            this.usersService = usersService;
            this.tripsService = tripsService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> UserTripList()
        {
            var userId = this.userManager.GetUserId(this.User);

            var viewModel = new UserTripListViewModel
            {
                Trips = this.tripsService.GetUserTrips<TripDetailsViewModel>(userId),
            };

            return this.View(viewModel);
        }

        public async Task<IActionResult> AcceptRequest(string senderId, string tripId)
        {
            var userId = this.userManager.GetUserId(this.User);
            var trip = this.tripsService.GetTripByUserId(userId);
            var result = this.usersService.AcceptTripRequest(senderId, trip.Id, userId);

            await this.usersService.AddPassengerToTrip(tripId, senderId);
            return this.RedirectToAction("UserTripList", "Users");
        }

        public async Task<IActionResult> DeclineRequest(string senderId, string tripId)
        {
            var userId = this.userManager.GetUserId(this.User);
            var trip = this.tripsService.GetTripByUserId(userId);
            await this.usersService.DeclineTripRequest(senderId, trip.Id, userId);

            return this.RedirectToAction("Details", "Trips", new { id = tripId });
        }

        public async Task<IActionResult> Details(string userId)
        {
            var viewModel = new UserDetailsViewModel();

            return this.View(viewModel);
        }
    }
}
