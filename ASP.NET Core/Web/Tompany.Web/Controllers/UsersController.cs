using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Web.ViewModels.Trips;

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

            var viewModel = new TripListViewModel
            {
                Trips = this.tripsService.GetUserTrips<TripDetailsViewModel>(userId),
            };

            return this.View(viewModel);
        }

        public async Task<IActionResult> AcceptRequest(string senderId, string tripId)
        {
            var userId = this.userManager.GetUserId(this.User);
            var trip = this.tripsService.GetTripByUserId(userId);
            var accept = this.usersService.AcceptTripRequest(senderId, trip.Id, userId);
            
            return this.View();
        }
    }
}
