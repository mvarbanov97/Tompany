using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Web.ViewModels.Cars;
using Tompany.Web.ViewModels.Travels;

namespace Tompany.Web.Controllers
{
    public class TripsController : BaseController
    {
        private readonly ITripsService travelsService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICarsService carsService;

        public TripsController(
            ITripsService travelsService,
            UserManager<ApplicationUser> userManager,
            ICarsService carsService)
        {
            this.travelsService = travelsService;
            this.userManager = userManager;
            this.carsService = carsService;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Create()
        {
            var cars = this.carsService.GetAll<CarDropDownViewModel>();

            var viewModel = new TravelCreateInputModel
            {
                Cars = cars,
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TravelCreateInputModel input)
        {
            var userId = this.userManager.GetUserId(this.User);
            await this.travelsService.CreateAsync(input, userId);
            return this.RedirectToAction("Index", "Home");
        }
    }
}
