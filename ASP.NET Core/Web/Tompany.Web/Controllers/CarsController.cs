using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Web.ViewModels.Cars;

namespace Tompany.Web.Controllers
{
    public class CarsController : BaseController
    {
        private readonly ICarsService carsService;
        private readonly UserManager<ApplicationUser> userManager;


        public CarsController(
            ICarsService carsService,
            UserManager<ApplicationUser> userManager)
        {
            this.carsService = carsService;
            this.userManager = userManager;
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarCreateInputModel input)
        {
            var userId = this.userManager.GetUserId(this.HttpContext.User);

            await this.carsService.CreateAsync(userId, input);
            return this.RedirectToAction("Index", "Home");
        }
    }
}
