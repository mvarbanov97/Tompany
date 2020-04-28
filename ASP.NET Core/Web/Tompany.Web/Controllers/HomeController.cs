namespace Tompany.Web.Controllers
{
    using System.Diagnostics;

    using Tompany.Web.ViewModels;

    using Microsoft.AspNetCore.Mvc;
    using Tompany.Services.Data.Contracts;
    using Tompany.Web.ViewModels.Trips;
    using System.Threading.Tasks;
    using System;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using Tompany.Web.Common;

    public class HomeController : BaseController
    {
        private readonly IDestinationService destinationsService;
        private readonly ITripsService tripsService;

        public HomeController(
            IDestinationService destinationsService,
            ITripsService tripsService)
        {
            this.destinationsService = destinationsService;
            this.tripsService = tripsService;
        }

        public async Task<IActionResult> Index(TripSearchViewModel model)
        {

            this.ViewData["Destinations"] = SelectListGenerator.GetAllDestinations(this.destinationsService);
            return this.View();
        }

        [Route("[action]")]
        public async Task<IActionResult> Search()
        {
            // bool isAjaxCall = this.Request.Headers["x-requested-with"] == "XMLHttpRequest";
            var fromDestination = int.Parse(this.Request.Query["FromDestinationId"]);
            var toDestination = int.Parse(this.Request.Query["ToDestinationId"]);
            var dateOfDeparture = DateTime.Parse(this.Request.Query["DateOfDeparture"]);

            var searchResultViewModel = await this.destinationsService.GetSearchResultAsync(
                fromDestination,
                toDestination,
                dateOfDeparture);

            return this.PartialView("_SearchResultPartial", searchResultViewModel);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
