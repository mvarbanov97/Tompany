using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Services.Mapping;
using Tompany.Web.ViewModels.Trips.InputModels;

namespace Tompany.Services.Data
{
    public class TripsService : ITripsService
    {
        private readonly IRepository<Trip> tripsRepository;
        private readonly IRepository<Destination> destinationsRepository;
        private readonly IRepository<ApplicationUser> usersRepository;

        public TripsService(
            IRepository<Trip> tripsRepository,
            IRepository<Destination> destinationsRepository,
            IRepository<ApplicationUser> usersRepository)
        {
            this.tripsRepository = tripsRepository;
            this.destinationsRepository = destinationsRepository;
            this.usersRepository = usersRepository;
        }

        public async Task CreateAsync(TripCreateInputModel input)
        {
            var trip = new Trip
            {
                FromDestinationName = input.FromDestinationName,
                ToDestinationName = input.ToDestinationName,
                User = input.ApplicationUser,
                UserId = input.UserId,
                Car = input.Car,
                CarId = input.CarId,
                PricePerPassenger = input.PricePerPassenger,
                DateOfDeparture = input.DateOfDeparture,
                TimeOfDeparture = input.TimeOfDeparture,
                AdditionalInformation = input.AdditionalInformation,
                GroupName = Guid.NewGuid().ToString(),
            };

            var userTrip = new UserTrip
            {
                User = trip.User,
                UserId = trip.UserId,
                Trip = trip,
                TripId = trip.Id,
            };

            var user = input.ApplicationUser;
            user.UserTrips.Add(userTrip);
            this.usersRepository.Update(user);
            await this.usersRepository.SaveChangesAsync();
        }

        public async Task EditAsync(TripEditInputModel tripToEdit)
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
            trip.CarId = tripToEdit.CarId;

            this.tripsRepository.Update(trip);
            await this.tripsRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
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

        public async Task<bool> IsTripExist(string id)
        {
            var trip = await this.tripsRepository.All().AnyAsync(x => x.Id == id);

            return trip;
        }

        public T GetById<T>(string id)
        {
            var trip = this.tripsRepository
                .All()
                .Include(x => x.TripRequest)
                .Include(x => x.Car)
                .Include(x => x.Views)
                .Include(x => x.Passengers)
                .Where(x => x.Id == id)
                .To<T>()
                .FirstOrDefault();

            return trip;
        }

        public Trip GetById(string id)
        {
            var trip = this.tripsRepository
                .All()
                .Include(x => x.Car)
                .Include(x => x.Passengers)
                .Include(x => x.TripRequest)
                .Include(x => x.Views)
                .Where(x => x.Id == id).FirstOrDefault();

            return trip;
        }

        public IEnumerable<T> GetUserTripsWithUsername<T>(string username)
        {
            var cars = this.tripsRepository
                .All()
                .Include(x => x.Passengers)
                .Include(x => x.Car)
                .Include(x => x.UserTrips)
                .Where(x => x.User.UserName == username)
                .To<T>()
                .ToList();

            return cars;
        }

        public IEnumerable<T> GetTripPosts<T>(int? take = null, int skip = 0)
        {
            var tripPosts = this.tripsRepository
                .All()
                .Where(x => x.Car.IsDeleted == false & x.IsDeleted == false)
                .OrderByDescending(x => x.CreatedOn)
                .Skip(skip)
                .To<T>()
                .ToList();

            if (take.HasValue)
            {
                tripPosts = tripPosts.Take(take.Value).ToList();
            }

            return tripPosts;
        }

        public int Count()
        {
            return this.tripsRepository.All().Count();
        }

        public async Task<IEnumerable<T>> GetAllDestinationsAsync<T>()
        {
            IQueryable<Destination> query = this.destinationsRepository.All();

            return await query.To<T>().ToListAsync();
        }
    }
}
