using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Services.Data.Contracts;
using Tompany.Web.ViewModels.Cars;

namespace Tompany.Web.Controllers
{
    public class CarsController : BaseController
    {
        private readonly ICarsService carsService;

        public CarsController(
            ICarsService carsService)
        {
            this.carsService = carsService;
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarCreateInputModel input)
        {
            await this.carsService.CreateAsync(input);
            return this.RedirectToAction("Index", "Home");
        }
    }
}
