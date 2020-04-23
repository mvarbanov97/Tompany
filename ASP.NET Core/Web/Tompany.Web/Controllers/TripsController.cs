﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Web.ViewModels.Cars;
using Tompany.Web.ViewModels.Trips;

namespace Tompany.Web.Controllers
{
    public class TripsController : BaseController
    {
        private const int ItemsPerPage = 5;

        private readonly ITripsService tripsService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICarsService carsService;
        private readonly IUsersService usersService;

        public TripsController(
            ITripsService tripsService,
            UserManager<ApplicationUser> userManager,
            ICarsService carsService,
            IUsersService usersService)
        {
            this.tripsService = tripsService;
            this.userManager = userManager;
            this.carsService = carsService;
            this.usersService = usersService;
        }

        public IActionResult Index(int page = 1)
        {
            var count = this.tripsService.GetTripsCount();
            var viewModel = new TripListViewModel()
            {
                Trips = this.tripsService.GetTripPosts<TripDetailsViewModel>(ItemsPerPage, (page - 1) * ItemsPerPage),
                PagesCount = (int)Math.Ceiling((double)count / ItemsPerPage),
                CurrentPage = page,
            };

            return this.View(viewModel);
        }

        [Authorize]
        public IActionResult Create()
        {
            var userId = this.userManager.GetUserId(this.User);

            var cars = this.carsService.GetCarByUserId<CarDropDownViewModel>(userId);

            var viewModel = new TripCreateInputModel
            {
                Cars = cars,
            };

            return this.View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(TripCreateInputModel input)
        {
            var userId = this.userManager.GetUserId(this.User);
            await this.tripsService.CreateAsync(input, userId);
            return this.RedirectToAction("Index", "Trips");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Details(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var tripViewModel = this.tripsService.GetById<TripDetailsViewModel>(id);
            var car = this.carsService.GetById<CarViewModel>(tripViewModel.CarId);

            tripViewModel.Car = car;
            tripViewModel.Views.Add(new View { UserId = userId });

            if (tripViewModel == null)
            {
                return this.NotFound();
            }

            return this.View(tripViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TripEditViewModel tripToEditViewModel)
        {
            await this.tripsService.EditAsync(tripToEditViewModel);
            return this.RedirectToAction("Details", "Trips", new { area = "", id = tripToEditViewModel.Id});
        }

        public async Task<IActionResult> Edit(string id)
        {
            var tripToEdit = this.tripsService.GetById<TripEditViewModel>(id);
            return this.View(tripToEdit);
        }

        public async Task<IActionResult> Delete(string id)
        {
            await this.tripsService.DeleteById(id);

            return this.RedirectToAction("UserListTrip", "Users");
        }

        public IActionResult Candidate(string tripId)
        {
            var user = this.User.Identity.Name;
            var trip = this.tripsService.GetById<TripDetailsViewModel>(tripId);
            var ownerId = trip.UserId;

            this.tripsService.SendTripRequest(user, tripId, ownerId);

            return this.View("_TripRequestSendSuccessfully");
        }
    }
}
