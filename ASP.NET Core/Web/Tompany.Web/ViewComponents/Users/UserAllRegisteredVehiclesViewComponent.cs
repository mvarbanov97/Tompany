using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Web.ViewModels.Cars.ViewModels;
using Tompany.Web.ViewModels.Users.ViewModels;

namespace Tompany.Web.ViewComponents.Users
{
    public class UserAllRegisteredVehiclesViewComponent : ViewComponent
    {
        private readonly ICarsService carsService;
        private readonly IUsersService usersService;
        private readonly UserManager<ApplicationUser> userManager;

        public UserAllRegisteredVehiclesViewComponent(
            ICarsService carsService,
            UserManager<ApplicationUser> userManager, IUsersService usersService)
        {
            this.carsService = carsService;
            this.userManager = userManager;
            this.usersService = usersService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string username, int page)
        {
            var allUserCars = this.carsService.GetAllUserCarsByUserUsername<CarViewModel>(username);
            var user = this.usersService.GetUserByUsername<ApplicationUserViewModel>(username);

            CarListViewModel model = new CarListViewModel
            {
                UserId = user.Id,
                UserUsername = username,
                Cars = allUserCars,
            };

            return this.View(model);
        }
    }
}
