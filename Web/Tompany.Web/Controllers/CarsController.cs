using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Common;
using Tompany.Data.Models;
using Tompany.Services;
using Tompany.Services.Data.Contracts;
using Tompany.Web.ViewModels.Cars.InputModels;
using Tompany.Web.ViewModels.Cars.ViewModels;

namespace Tompany.Web.Controllers
{
    public class CarsController : BaseController
    {
        private readonly ICarsService carsService;
        private readonly ICloudinaryService cloudinaryService;
        private readonly UserManager<ApplicationUser> userManager;

        public CarsController(
            ICarsService carsService,
            UserManager<ApplicationUser> userManager,
            ICloudinaryService cloudinaryService)
        {
            this.carsService = carsService;
            this.userManager = userManager;
            this.cloudinaryService = cloudinaryService;
        }

        [Authorize]
        public async Task<ActionResult> Index()
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);

            var userCars = this.carsService.GetAllUserCarsByUserId<CarViewModel>(currentUser.Id);

            var model = new CarListViewModel
            {
                UserUsername = currentUser.UserName,
                Cars = userCars,
            };

            return this.View(model);
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CarCreateInputModel input)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);

            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            await this.carsService.CreateAsync(currentUser.Id, input);

            return this.RedirectToAction("Index", "Cars");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);

            var isCarExist = this.carsService.IsCarExists(id, currentUser.Id);
            if (isCarExist)
            {
                CarEditIputModel model = await this.carsService.ExtractCar(id, currentUser.Id);
                return this.View(model);
            }
            else
            {
                return this.Unauthorized();
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(CarEditIputModel input)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);

            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            if (input.CarPicture != null)
            {
                string carImageUrl = await this.cloudinaryService.UploadImageAsync(
                                       input.CarPicture,
                                       string.Format(GlobalConstants.CloudinaryCarPictureName, currentUser.Id));
                input.CarImageUrl = carImageUrl;
            }

            await this.carsService.EditAsync(input, currentUser.Id);

            return this.RedirectToAction("Profile", "Users", new { username = currentUser.UserName, tab = "UserAllRegisteredVehicles" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);
            var isCarExist = this.carsService.IsCarExists(id, currentUser.Id);

            if (isCarExist)
            {
                await this.carsService.DeleteAsync(id, currentUser.Id);
                //TODO delete existing trips with removed car
            }
            else
            {
                return this.Unauthorized();
            }

            return this.RedirectToAction("Profile", "Users", new { username = currentUser.UserName, tab = "UserAllRegisteredVehicles" });
        }
    }
}
