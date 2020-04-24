using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Services.Mapping;
using Tompany.Web.ViewModels.Trips;
using Tompany.Data.Models.Enums;

namespace Tompany.Services.Data
{
    public class UsersService : IUsersService
    {
        private readonly IRepository<ApplicationUser> usersRepository;
        private readonly IRepository<Trip> tripsRepository;
        private readonly IRepository<TripRequest> tripRequestRepository;

        public UsersService(
            IRepository<ApplicationUser> usersRepository,
            IRepository<Trip> tripsRepository,
            IRepository<TripRequest> tripRequestRepository)
        {
            this.usersRepository = usersRepository;
            this.tripsRepository = tripsRepository;
            this.tripRequestRepository = tripRequestRepository;
        }

        public ApplicationUser GetUserById(string id)
        {
            var user = this.usersRepository.All().FirstOrDefault(x => x.Id == id);

            return user;
        }

        public async Task AcceptTripRequest(string senderId, string tripId, string userId)
        {
            var user = this.GetUserById(userId);
            var sender = this.GetUserById(senderId);

            var trip = this.tripsRepository
                .All()
                .Include(x => x.TripRequest)
                .Include(x => x.Car)
                .Include(x => x.Passengers)
                .Where(x => x.TripRequest.Any(x => x.SenderId == senderId && x.TripId == tripId && x.RequestStatus == RequestStatus.Pending))
                .FirstOrDefault();

            var tripRequest = trip.TripRequest.Where(x => x.TripId == trip.Id && x.SenderId == senderId).FirstOrDefault();
            
            tripRequest.RequestStatus = RequestStatus.Accepted;

            this.tripRequestRepository.Update(tripRequest);
            await this.tripRequestRepository.SaveChangesAsync();
        }

        public async Task DeclineRequest(string senderId, string tripId, string userId)
        {
            var trip = this.tripsRepository
                .All()
                .Include(x => x.TripRequest)
                .Include(x => x.Car)
                .Include(x => x.Passengers)
                .Where(x => x.TripRequest.Any(x => x.SenderId == senderId && x.TripId == tripId && x.RequestStatus == RequestStatus.Pending))
                .FirstOrDefault();

            var tripRequest = trip.TripRequest.Where(x => x.TripId == trip.Id && x.SenderId == senderId).FirstOrDefault();

            tripRequest.RequestStatus = RequestStatus.Declined;

            this.tripRequestRepository.Update(tripRequest);
            await this.tripRequestRepository.SaveChangesAsync();
        }

        public async Task AddPassengerToTrip(string tripId, string passengerId)
        {
            var trip = this.tripsRepository
                .All()
                .Include(x => x.TripRequest)
                .Include(x => x.Passengers)
                .Include(x => x.Car)
                .Where(x => x.TripRequest.Any(x => x.TripId == tripId && x.SenderId == passengerId))
                .FirstOrDefault();

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

        public async Task GetUserCars(string userId)
        {
            var cars = this.usersRepository.All().Where(x => x.Cars.Any(x => x.UserId == userId));
        }
    }
}
