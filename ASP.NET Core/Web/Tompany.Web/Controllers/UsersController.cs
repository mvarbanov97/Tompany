using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Web.ViewModels.Users;

namespace Tompany.Web.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUsersService usersService;

        public UsersController(
            IUsersService usersService)
        {
            this.usersService = usersService;
        }

        public async Task<IActionResult> Index(string id)
        {
            var viewModel = this.usersService.GetById<UserViewModel>(id);

            return this.View();
        }
    }
}
