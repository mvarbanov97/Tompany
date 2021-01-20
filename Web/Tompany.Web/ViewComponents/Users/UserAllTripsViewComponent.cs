using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Web.ViewModels.Trips.ViewModels;

namespace Tompany.Web.ViewComponents.Users
{
    public class UserAllTripsViewComponent : ViewComponent
    {
        private readonly ITripsService tripsService;

        public UserAllTripsViewComponent(ITripsService tripsService)
        {
            this.tripsService = tripsService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string username, int page)
        {
            var allUserTrips = this.tripsService.GetUserTripsWithUsername<TripDetailsViewModel>(username);

            TripListViewModel model = new TripListViewModel
            {
                Trips = allUserTrips,
            };

            return this.View(model);
        }
    }
}
