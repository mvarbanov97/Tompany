using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Web.Common;
using Tompany.Web.ViewModels.Cars;
using Tompany.Web.ViewModels.Destinations;
using Tompany.Web.ViewModels.Trips;

namespace Tompany.Web.Controllers
{
    public class TripsController : BaseController
    {
        private const int ItemsPerPage = 5;

        private readonly ITripsService tripsService;
        private readonly ICarsService carsService;
        private readonly IUsersService usersService;
        private readonly IDestinationService destinationsService;
        private readonly IViewService viewsService;
        private readonly ITripRequestsService tripRequestsService;
        private readonly UserManager<ApplicationUser> userManager;

        public TripsController(
            ITripsService tripsService,
            ICarsService carsService,
            IUsersService usersService,
            IDestinationService destinationsService,
            IViewService viewsService,
            ITripRequestsService tripRequestsService,
            UserManager<ApplicationUser> userManager)
        {
            this.tripsService = tripsService;
            this.carsService = carsService;
            this.usersService = usersService;
            this.destinationsService = destinationsService;
            this.viewsService = viewsService;
            this.tripRequestsService = tripRequestsService;
            this.userManager = userManager;
        }

        public IActionResult Index(TripSearchViewModel input, int page = 1)
        {
            var count = this.tripsService.GetTripsCount();
            var viewModel = new TripListViewModel()
            {
                Trips = this.tripsService.GetTripPosts<TripDetailsViewModel>(ItemsPerPage, (page - 1) * ItemsPerPage),
                PagesCount = (int)Math.Ceiling((double)count / ItemsPerPage),
                CurrentPage = page,
                SearchQuery = input,
            };

            return this.View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            var userId = this.userManager.GetUserId(this.User);

            var cars = this.carsService.GetCarByUserId<CarDropDownViewModel>(userId);

            var viewModel = new TripCreateInputModel
            {
                Cars = cars,
                Destinations = this.destinationsService.GetAllDestinationsAsync(),
                DateOfDeparture = DateTime.Now,
            };

            return this.View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(TripCreateInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return this.View(input);
            }

            var userId = this.userManager.GetUserId(this.User);
            var cars = this.usersService.GetUserCars(userId);
            await this.tripsService.CreateAsync(input, userId);

            return this.RedirectToAction("Index", "Trips");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Details(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var tripViewModel = this.tripsService.GetById<TripDetailsViewModel>(id);
            var tripRequests = this.tripRequestsService.GetAllTripRequestsByTripId(id);

            tripViewModel.TripRequests = tripRequests;

            if (tripViewModel == null)
            {
                return this.NotFound();
            }

            return this.View(tripViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TripEditViewModel tripToEditViewModel)
        {
            await this.tripsService.EditAsync(tripToEditViewModel);

            return this.RedirectToAction("Details", "Trips", new { area = "", id = tripToEditViewModel.Id});
        }

        public async Task<IActionResult> Edit(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var destinations = this.destinationsService.GetAllDestinationsAsync();

            var cars = this.carsService.GetCarByUserId<CarDropDownViewModel>(userId);
            var tripToEdit = this.tripsService.GetById<TripEditViewModel>(id);

            tripToEdit.Cars = cars;
            tripToEdit.Destinations = destinations;

            return this.View(tripToEdit);
        }

        public async Task<IActionResult> Delete(string id)
        {
            await this.tripsService.DeleteById(id);

            return this.RedirectToAction("UserListTrip", "Users");
        }

        public IActionResult Candidate(string tripId)
        {
            var user = this.User.Identity.Name;
            var trip = this.tripsService.GetById<TripDetailsViewModel>(tripId);
            var ownerId = trip.UserId;

            this.tripRequestsService.SendTripRequest(user, tripId, ownerId);

            return this.View("_TripRequestSendSuccessfully");
        }
    }
}
