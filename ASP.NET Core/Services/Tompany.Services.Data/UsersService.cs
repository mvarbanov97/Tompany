﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Data.Models.Enums;
using Tompany.Services.Data.Contracts;
using Tompany.Services.Mapping;

namespace Tompany.Services.Data
{
    public class UsersService : IUsersService
    {
        private readonly IRepository<ApplicationUser> usersRepository;
        private readonly IRepository<Trip> tripsRepository;
        private readonly IRepository<TripRequest> tripRequestRepository;
        private readonly ITripsService tripsService;
        private readonly ICarsService carsService;

        public UsersService(
            IRepository<ApplicationUser> usersRepository,
            IRepository<Trip> tripsRepository,
            IRepository<TripRequest> tripRequestRepository,
            ITripsService tripsService,
            ICarsService carsService)
        {
            this.usersRepository = usersRepository;
            this.tripsRepository = tripsRepository;
            this.tripRequestRepository = tripRequestRepository;
            this.tripsService = tripsService;
            this.carsService = carsService;
        }

        public ApplicationUser GetUserById(string id)
        {
            var user = this.usersRepository.All().FirstOrDefault(x => x.Id == id);

            return user;
        }

        public T GetUserByUsername<T>(string username)
        {
            var user = this.usersRepository.All().Where(x => x.UserName == username).To<T>().FirstOrDefault();

            return user;
        }

        public async Task GetUserCars(string userId)
        {
            var cars = this.usersRepository.All().Where(x => x.Cars.Any(x => x.UserId == userId));
        }

        public async Task AcceptTripRequest(string passengerId, Trip trip, string userId)
        {
            ApplicationUser user = this.GetUserById(userId);
            if (user == null)
            {
                throw new NullReferenceException($"There is no user with Id {userId}");
            }

            ApplicationUser passenger = this.GetUserById(passengerId);
            if (passenger == null)
            {
                throw new NullReferenceException($"There is no passenger with Id {passengerId}");
            }

            if (trip.Passengers.Count() >= trip.Car.Seats)
            {
                return;
            }

            var tripRequest = trip.TripRequest.Where(x => x.TripId == trip.Id && x.SenderId == passengerId).FirstOrDefault();
            tripRequest.RequestStatus = RequestStatus.Accepted;

            this.tripRequestRepository.Update(tripRequest);
            await this.tripRequestRepository.SaveChangesAsync();

        }

        public async Task DeclineTripRequest(string senderId, string tripId, string userId)
        {
            var tripRequest = this.tripRequestRepository
                .All()
                .Where(x => x.TripId == tripId && x.SenderId == senderId)
                .FirstOrDefault();

            tripRequest.RequestStatus = RequestStatus.Declined;

            this.tripRequestRepository.Update(tripRequest);
            await this.tripRequestRepository.SaveChangesAsync();
        }

        public async Task AddPassengerToTrip(string tripId, string passengerId)
        {
            var trip = this.tripsService.GetById(tripId);

            var passenger = this.GetUserById(passengerId);

            var tripRequest = trip.TripRequest.Where(x => x.SenderId == passengerId && x.TripId == trip.Id).FirstOrDefault();

            var seats = trip.Car.Seats;

            if (trip.Passengers.Count >= seats)
            {
                throw new InvalidOperationException($"There are no free seats left for this trip.");
            }
            else
            {
                if (tripRequest.RequestStatus == RequestStatus.Accepted)
                {
                    trip.Passengers.Add(passenger);
                }
            }

            this.tripsRepository.Update(trip);
            await this.tripsRepository.SaveChangesAsync();
        }

        public bool IsRequestAlreadySent(string senderId, string tripId)
        {
            return this.tripRequestRepository.All().Any(x => x.SenderId == senderId && x.TripId == tripId);
        }
    }
}
