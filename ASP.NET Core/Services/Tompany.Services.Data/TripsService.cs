using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

namespace Tompany.Services.Data
{
    public class TripsService : ITripsService
    {
        private readonly IRepository<Trip> tripsRepository;
        private readonly IRepository<ApplicationUser> usersRepository;
        private readonly IRepository<UserTrip> userTripsRepository;
        private readonly IRepository<TripRequest> tripRequestRepository;
        private readonly IRepository<Destination> destinationsRepository;
        private readonly ILogger<TripsService> logger;

        public TripsService(
            IRepository<Trip> tripsRepository,
            IRepository<ApplicationUser> usersRepository,
            IRepository<UserTrip> userTripsRepository,
            IRepository<TripRequest> tripRequestRepository,
            IRepository<Destination> destinationsRepository,
            ILogger<TripsService> logger)
        {
            this.tripsRepository = tripsRepository;
            this.usersRepository = usersRepository;
            this.userTripsRepository = userTripsRepository;
            this.tripRequestRepository = tripRequestRepository;
            this.destinationsRepository = destinationsRepository;
            this.logger = logger;
        }

        public async Task CreateAsync(TripCreateInputModel input, string userId)
        {
            var user = this.usersRepository.All().FirstOrDefault(x => x.Id == userId);

            var trip = new Trip
            {
                FromDestinationName = input.FromDestinationName,
                ToDestinationName = input.ToDestinationName,
                UserId = userId,
                CarId = input.CarId,
                PricePerPassenger = input.PricePerPassenger,
                DateOfDeparture = input.DateOfDeparture,
                TimeOfDeparture = input.TimeOfDeparture,
                AdditionalInformation = input.AdditionalInformation,
            };

            var userTrip = new UserTrip
            {
                User = user,
                UserId = user.Id,
                Trip = trip,
                TripId = trip.Id,
            };

            user.UserTrips.Add(userTrip);
            this.usersRepository.Update(user);
            await this.tripsRepository.AddAsync(trip);
            await this.tripsRepository.SaveChangesAsync();
        }

        public T GetById<T>(string id)
        {
            var trip = this.tripsRepository.All().Include(x => x.TripRequest).Where(x => x.Id == id)
                .To<T>().FirstOrDefault();

            return trip;
        }

        public IEnumerable<T> GetAll<T>(int? count = null)
        {
            IQueryable<Trip> query = this.tripsRepository.All().Include(x => x.TripRequest).Where(x => x.Car.IsDeleted == false & x.IsDeleted == false);

            if (count.HasValue)
            {
                query = query.Take(count.Value);
            }

            return query.To<T>().ToList();
        }

        public IEnumerable<T> GetUserTrips<T>(string userId)
        {
            IQueryable<Trip> query = this.tripsRepository
                .All()
                .Where(x => x.UserId == userId);

            return query.To<T>().ToList();
        }

        public Trip GetTripByUserId(string userId)
        {
            var trip = this.tripsRepository.All().Include(x => x.TripRequest).Where(x => x.UserId == userId).FirstOrDefault();

            return trip;
        }

        public IEnumerable<T> GetTripPosts<T>(int? take = null, int skip = 0)
        {
            var query = this.tripsRepository
                .All()
                .Where(x => x.Car.IsDeleted == false & x.IsDeleted == false)
                .OrderByDescending(x => x.CreatedOn)
                .Skip(skip);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query.To<T>().ToList();
        }

        public T GetTripByUserId<T>(string userId, string id)
        {
            var trip = this.tripsRepository.All()
                .Where(x => x.UserId == userId)
                .To<T>()
                .FirstOrDefault();

            return trip;
        }

        public int GetTripsCount()
        {
            return this.tripsRepository.All().Count();
        }

        public async Task DeleteById(string id)
        {
            var tripToDelete = this.tripsRepository.All().FirstOrDefault(t => t.Id == id);

            if (tripToDelete == null)
            {
                throw new NullReferenceException($"Activity with id {id} not found.");
            }

            tripToDelete.IsDeleted = true;
            tripToDelete.DeletedOn = DateTime.Now;
            this.tripsRepository.Update(tripToDelete);
            await this.tripsRepository.SaveChangesAsync();
        }

        public async Task EditAsync(TripEditViewModel tripToEdit)
        {
            var trip = this.tripsRepository.All().FirstOrDefault(t => t.Id == tripToEdit.Id);

            if (trip == null)
            {
                throw new NullReferenceException($"Activity with id {tripToEdit.Id} not found");
            }

            trip.FromDestinationName = tripToEdit.FromDestinationName;
            trip.ToDestinationName = tripToEdit.ToDestinationName;
            trip.DateOfDeparture = tripToEdit.DateOfDeparture;
            trip.AdditionalInformation = tripToEdit.AdditionalInformation;

            this.tripsRepository.Update(trip);
            await this.tripsRepository.SaveChangesAsync();
        }

        public async Task SendTripRequest(string userName, string tripId, string ownerId)
        {
            ApplicationUser owner = null;
            owner = this.usersRepository.All().Include(x => x.UserTrips).FirstOrDefault(y => y.Id == ownerId);

            var trip = this.tripsRepository.All().FirstOrDefault(x => x.Id == tripId);

            ApplicationUser sender = null;
            sender = this.usersRepository.All().FirstOrDefault(x => x.UserName == userName);

            var tripRequest = new TripRequest
            {
                Trip = trip,
                TripId = tripId,
                Sender = sender,
                SenderId = sender.Id,
            };

            trip.TripRequest.Add(tripRequest);

            await this.tripRequestRepository.AddAsync(tripRequest);
            await this.tripRequestRepository.SaveChangesAsync();
            // await this.tripsRepository.SaveChangesAsync();

            // this.tripsRepository.Update(trip);

        }

        public IEnumerable<TripRequest> GetAllTripRequestInTrip(string tripId)
        {
            var tripRequests = this.tripRequestRepository
                .All()
                .Include(x => x.Sender)
                .Where(x => x.TripId == tripId)
                .ToList();

            return tripRequests;
        }

        public async Task<TripSearchViewModel> GetSearchResultAsync(int fromDestinationId, int toDestination, DateTime dateOfDeparture)
        {
            var destinations = this.destinationsRepository.All().Select(x => x.Name).ToList().FirstOrDefault();

            if (destinations == null)
            {
                throw new NullReferenceException($"Trip with id {destinations} not found.");
            }

            var searchViewModel = new TripSearchViewModel
            {
                FromDestinationName = destinations,
                ToDestinationName = destinations,
                DateOfDeparture = DateTime.UtcNow,
            };

            return searchViewModel;
        }
    }
}
