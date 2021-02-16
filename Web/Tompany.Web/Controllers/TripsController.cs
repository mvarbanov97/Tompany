namespace Tompany.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Tompany.Common;
    using Tompany.Data.Models;
    using Tompany.Services.Data.Contracts;
    using Tompany.Web.Common;
    using Tompany.Web.Infrastructure;
    using Tompany.Web.ViewModels.Cars.ViewModels;
    using Tompany.Web.ViewModels.Destinations.ViewModels;
    using Tompany.Web.ViewModels.Trips.InputModels;
    using Tompany.Web.ViewModels.Trips.ViewModels;
    using X.PagedList;

    public class TripsController : BaseController
    {
        private readonly ITripsService tripsService;
        private readonly ICarsService carsService;
        private readonly IViewService viewsService;
        private readonly ITripRequestsService tripRequestsService;
        private readonly IDestinationService destinationsService;
        private readonly UserManager<ApplicationUser> userManager;

        public TripsController(
            ITripsService tripsService,
            ICarsService carsService,
            IViewService viewsService,
            ITripRequestsService tripRequestsService,
            IDestinationService destinationService,
            UserManager<ApplicationUser> userManager)
        {
            this.tripsService = tripsService;
            this.carsService = carsService;
            this.viewsService = viewsService;
            this.tripRequestsService = tripRequestsService;
            this.destinationsService = destinationService;
            this.userManager = userManager;
        }

        public IActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;
            this.ViewData["Destinations"] = SelectListGenerator.GetAllDestinations(this.destinationsService);
            var trips = this.tripsService.GetTripPosts<TripDetailsViewModel>();

            var viewModel = new TripListViewModel()
            {
                Trips = trips.ToPagedList(pageNumber, GlobalConstants.ItemsPerPage),
                SearchQuery = new TripSearchInputModel(),
            };

            return this.View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Search()
        {
            var fromDestination = int.Parse(this.Request.Query["FromDestinationId"]);
            var toDestination = int.Parse(this.Request.Query["ToDestinationId"]);
            var date = DateTime.TryParse(this.Request.Query["DateOfDeparture"], out DateTime dateOfDeparture);

            var count = this.tripsService.Count();
            this.ViewData["Destinations"] = SelectListGenerator.GetAllDestinations(this.destinationsService);

            TripSearchViewModel searchResultViewModel = await this.destinationsService.GetSearchResultAsync(fromDestination, toDestination, dateOfDeparture);
            return this.PartialView("_SearchResultPartial", searchResultViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);

            var cars = this.carsService.GetAllUserCarsByUserId<CarDropDownViewModel>(currentUser.Id);

            var viewModel = new TripCreateInputModel
            {
                ApplicationUser = currentUser,
                Cars = cars,
                Destinations = await this.tripsService.GetAllDestinationsAsync<DestinationViewModel>(),
                DateOfDeparture = DateTime.Now,
            };

            return this.View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(TripCreateInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            var user = await this.userManager.GetUserAsync(this.User);
            input.ApplicationUser = user;
            await this.tripsService.CreateAsync(input);

            return this.RedirectToAction("Index", "Trips");
        }

        [HttpGet]
        [Authorize]
        [Route("Trips/Details/{id}/{sendRequest?}")]
        public async Task<IActionResult> Details(string id, bool? sendRequest)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);
            await this.viewsService.AddViewAsync(id);
            var tripViewModel = this.tripsService.GetById<TripDetailsViewModel>(id);
            var tripRequests = this.tripRequestsService.GetAllTripRequestsByTripId(id);

            var isRequestAlreadySend = await this.tripRequestsService.IsRequesAlreadySend(currentUser.Id, id);
            {
                this.TempData["RequestMessage"] = GlobalConstants.SuccessfullySentTripRequest;
            }

            var sendRequestBoolean = sendRequest.HasValue ? sendRequest.Value : false;

            tripViewModel.TripRequests = tripRequests;
            tripViewModel.SendRequest = sendRequestBoolean;

            if (tripViewModel == null)
            {
                return this.NotFound();
            }

            return this.View(tripViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TripEditInputModel tripToEditViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(tripToEditViewModel);
            }

            await this.tripsService.EditAsync(tripToEditViewModel);

            return this.RedirectToAction("Details", "Trips", new { area = "", id = tripToEditViewModel.Id });
        }

        public async Task<IActionResult> Edit(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var destinations = await this.tripsService.GetAllDestinationsAsync<DestinationViewModel>();

            var cars = this.carsService.GetAllUserCarsByUserId<CarDropDownViewModel>(userId);
            var tripToEdit = this.tripsService.GetById<TripEditInputModel>(id);

            tripToEdit.Cars = cars;
            tripToEdit.Destinations = destinations;

            return this.View(tripToEdit);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);
            var tripExist = await this.tripsService.IsTripExist(id, currentUser.UserName);

            if (tripExist)
            {
                await this.tripsService.DeleteAsync(id);
            }
            else
            {
                return this.Unauthorized();
            }

            return this.RedirectToAction("Profile", "Users", new { username = currentUser.UserName, tab = "UserAllTrips" });
        }

        public async Task<IActionResult> Candidate(string tripId)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);
            var trip = this.tripsService.GetById(tripId);
            var ownerId = trip.UserId;

            var sendRequest = await this.tripRequestsService.SendTripRequest(currentUser.UserName, trip, ownerId);

            return this.RedirectToAction("Details", new { id = tripId, sendRequest = sendRequest });
        }
    }
}
