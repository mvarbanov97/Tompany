using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Data.Models.Enums;
using Tompany.Services.Data.Contracts;
using Tompany.Services.Mapping;
using Tompany.Web.Infrastructure;
using Tompany.Web.Infrastructure.Contracts;
using Tompany.Web.Infrastructure.Hubs;
using Tompany.Web.ViewModels.Trips.ViewModels;
using Tompany.Web.ViewModels.Users.ViewModels;

namespace Tompany.Web.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUsersService usersService;
        private readonly UserManager<ApplicationUser> userManager;

        public UsersController(
            IUsersService usersService,
            UserManager<ApplicationUser> userManager)
        {
            this.usersService = usersService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return this.NotFound();
        }

        [Route("Users/{username}/{tab?}/{page?}")]
        public async Task<IActionResult> Profile(string username, ProfileTab tab, int? page)
        {
            if (!await this.usersService.IsUserExists(username))
            {
                return this.NotFound();
            }

            var currentUser = await this.userManager.GetUserAsync(this.User);
            ApplicationUserViewModel user = await this.usersService.ExtractUserInfo(username, currentUser);

            var userThatRate = await this.userManager.GetUserAsync(this.User);
            var pageNumber = page ?? 1;

            var model = new UserProfileViewModel
            {
                ApplicationUser = user,
                CreatedPosts = await this.usersService.TakeCreatedTripPostsCountByUsername(username),
                RatingScore = this.usersService.ExtractUserRatingScore(username),
                LatestScore = await this.usersService.GetLatestScore(userThatRate, username),
            };

            model.ActiveTab = tab;
            model.Page = pageNumber;

            return this.View(model);
        }

        public async Task<IActionResult> SwitchToTabs(string username, string tab)
        {
            var user = await this.userManager.FindByNameAsync(username);

            var vm = tab switch
            {
                "UserAllRegisteredVehicles" => ProfileTab.UserAllRegisteredVehicles,
                "RegisteredPosts" => ProfileTab.RegisteredPosts,
                "UserAllTrips" => ProfileTab.UserAllTrips,
                _ => ProfileTab.RegisteredPosts,
            };

            return this.RedirectToAction("Profile", new { username = user.UserName, tab = vm });
        }

        [Route("/Profile/Users/{tab?}/{page?}/{search?}")]
        public IActionResult Users(AllUsersTab tab, int? page, string search)
        {
            var pageNumber = page ?? 1;

            if (search != null)
            {
                pageNumber = 1;
            }

            var model = new AllUsersViewModel
            {
                Search = search,
                ActiveTab = tab,
                Page = pageNumber,
            };

            return this.View();
        }

        public async Task<IActionResult> AcceptRequest(string senderId, string tripId)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);
            var sender = this.usersService.GetUserById(senderId);
            await this.usersService.AcceptTripRequest(senderId, currentUser, tripId);

            return this.RedirectToAction("Details", "Trips", new { id = tripId });
        }

        public async Task<IActionResult> DeclineRequest(string senderId, string tripId)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);
            var sender = this.usersService.GetUserById(senderId);
            await this.usersService.DeclineTripRequest(senderId, currentUser, tripId);

            return this.RedirectToAction("Details", "Trips", new { id = tripId });
        }

        [HttpPost]
        [Route("/RateUser")]
        public async Task<string> RateUser(string username, int rate)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);
            double rateUser = await this.usersService.RateUser(currentUser, username, rate);
            return $"{rateUser:F2}/5";
        }
    }
}
