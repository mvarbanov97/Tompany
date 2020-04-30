using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Web.ViewModels.WatchListTrips;

namespace Tompany.Web.Controllers
{
    [Authorize]
    public class WatchListsController : BaseController
    {
        private readonly IWatchListsService watchListsService;

        public WatchListsController(IWatchListsService watchListsService)
        {
            this.watchListsService = watchListsService;
        }

        public async Task<IActionResult> All()
        {
            IEnumerable<WatchListTripViewModel> watchListTrips = this.watchListsService.All(this.User.Identity.Name);

            return View(watchListTrips);
        }

        public async Task<IActionResult> Add(string tripId)
        {
            await this.watchListsService.AddAsync(tripId, this.User.Identity.Name);

            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Delete(string tripId)
        {
            await this.watchListsService.Delete(tripId, this.User.Identity.Name);

            return this.RedirectToAction(nameof(All));
        }
    }
}
