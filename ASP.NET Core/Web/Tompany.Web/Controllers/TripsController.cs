using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Web.ViewModels.Travels;

namespace Tompany.Web.Controllers
{
    public class TripsController : BaseController
    {
        private readonly ITravelsService travelsService;
        private readonly UserManager<ApplicationUser> userManager;

        public TripsController(
            ITravelsService travelsService,
            UserManager<ApplicationUser> userManager)
        {
            this.travelsService = travelsService;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TravelCreateInputModel input)
        {
            var userId = this.userManager.GetUserId(this.User);
            await this.travelsService.CreateAsync(input);
            return this.RedirectToAction("Index", "Home");
        }
    }
}
